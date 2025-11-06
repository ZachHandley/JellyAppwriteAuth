using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Appwrite;
using Appwrite.Services;
using Jellyfin.Plugin.Template.Appwrite;
using Jellyfin.Plugin.Template.Configuration;
using MediaBrowser.Controller.Authentication;
using MediaBrowser.Controller.Library;
using Jellyfin.Data.Entities;
using Microsoft.Extensions.Logging;
// Use Jellyfin's AuthenticationException to signal auth failures to core.
using JfAuthException = MediaBrowser.Controller.Authentication.AuthenticationException;

namespace Jellyfin.Plugin.Template.Authentication;

/// <summary>
/// Lightweight Appwrite-backed authentication provider.
/// Keeps Jellyfin URLs unchanged; validates credentials via Appwrite.
/// Creates/updates a local Jellyfin user whose username equals the Appwrite $id.
/// </summary>
public class AppwriteAuthenticationProvider : IAuthenticationProvider
{
    private readonly ILogger<AppwriteAuthenticationProvider> _logger;
    private readonly IUserManager _userManager;

    public AppwriteAuthenticationProvider(ILogger<AppwriteAuthenticationProvider> logger, IUserManager userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public string Name => "AppwriteAuth";

    // Jellyfin 10.9.x expects Authenticate(username, password). We keep a CancellationToken-capable helper below.
    public async Task<ProviderAuthenticationResult> Authenticate(string username, string password)
        => await AuthenticateCore(username, password, CancellationToken.None).ConfigureAwait(false);

    public bool IsEnabled => true;

    public bool HasPassword(User user) => true;

    public Task ChangePassword(User user, string newPassword)
    {
        // Password changes are handled via Appwrite; no-op here.
        return Task.CompletedTask;
    }

    private async Task<ProviderAuthenticationResult> AuthenticateCore(string username, string password, CancellationToken cancellationToken)
    {
        var cfg = Plugin.Instance?.Configuration ?? new PluginConfiguration();

        if (string.IsNullOrWhiteSpace(cfg.AppwriteEndpoint))
        {
            throw new JfAuthException("Appwrite endpoint not configured in plugin settings.");
        }

        if (string.IsNullOrWhiteSpace(cfg.AppwriteProjectId))
        {
            throw new JfAuthException("Appwrite project id not configured in plugin settings.");
        }

        // We prefer API key to resolve the user id reliably.
        var client = AppwriteClientFactory.Create(cfg);
        Users? usersSvc = null;
        if (!string.IsNullOrWhiteSpace(cfg.AppwriteApiKey))
        {
            usersSvc = new Users(client);
        }

        string? email = null;
        string? appwriteUserId = null;

        try
        {
            // Resolve Appwrite user by email or id using Admin API (requires API key).
            var apiKey = cfg.AppwriteApiKey;
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                var isEmailLogin = username.IndexOf('@') >= 0;
                if (isEmailLogin && usersSvc != null)
                {
                    var list = await usersSvc.List(search: username).ConfigureAwait(false);
                    var appUser = list.Users.FirstOrDefault(u => string.Equals(u.Email, username, StringComparison.OrdinalIgnoreCase));
                    if (appUser == null)
                    {
                        throw new JfAuthException("User not found in Appwrite.");
                    }
                    appwriteUserId = appUser.Id;
                    email = appUser.Email;
                }
                else
                {
                    // Try direct get by id
                    if (usersSvc != null)
                    {
                        try
                        {
                            var appUser = await usersSvc.Get(username).ConfigureAwait(false);
                            appwriteUserId = appUser.Id;
                            email = appUser.Email;
                        }
                        catch
                        {
                            // Fallback: treat as email
                            var list = await usersSvc.List(search: username).ConfigureAwait(false);
                            var appUser = list.Users.FirstOrDefault(u => string.Equals(u.Email, username, StringComparison.OrdinalIgnoreCase));
                            if (appUser == null)
                            {
                                throw new JfAuthException("User not found in Appwrite.");
                            }
                            appwriteUserId = appUser.Id;
                            email = appUser.Email;
                        }
                    }
                }
            }
            else
            {
                // Without API key, we can only attempt login with provided username as email
                email = username;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new JfAuthException("Unable to resolve Appwrite email for login.");
            }

            // Validate credentials via Appwrite SDK.
            var authService = new Jellyfin.Plugin.Template.Appwrite.AppwriteAuthService(cfg);
            var ok = await authService.ValidateCredentialsAsync(email!, password, cancellationToken).ConfigureAwait(false);
            if (!ok)
            {
                throw new JfAuthException("Invalid credentials.");
            }

            // Resolve Appwrite user id if we don't already have it (fallback via Account)
            if (string.IsNullOrWhiteSpace(appwriteUserId))
            {
                try
                {
                    var account = new Account(client);
                    var self = await account.Get().ConfigureAwait(false);
                    appwriteUserId = self.Id;
                }
                catch
                {
                    // Keep going; we can use email as a last-resort username if needed
                }
            }

            // Ensure Jellyfin user exists with username == Appwrite $id (preferred), otherwise fallback to email.
            // Name field rule: if they logged in with a username (not email), use that; otherwise use email
            var jfUsername = username.IndexOf('@') >= 0 ? email! : username;
            var jfUser = _userManager.GetUserByName(jfUsername);
            if (jfUser == null)
            {
                jfUser = await _userManager.CreateUserAsync(jfUsername).ConfigureAwait(false);
            }

            // Mark Appwrite email verification on successful login if enabled
            if (!string.IsNullOrWhiteSpace(appwriteUserId) && cfg.MarkEmailVerifiedOnLogin && usersSvc != null)
            {
                try
                {
                    await usersSvc.UpdateEmailVerification(appwriteUserId!, true).ConfigureAwait(false);
                }
                catch (Exception verEx)
                {
                    _logger.LogDebug(verEx, "Failed to mark email verified for {UserId}", appwriteUserId);
                }
            }

            return new ProviderAuthenticationResult
            {
                Username = jfUsername,
                DisplayName = jfUsername
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during Appwrite authentication for {User}", username);
            throw;
        }
    }

    public ValueTask CreateOrUpdateUser(string username, string password, CancellationToken cancellationToken)
    {
        // Jellyfin may call this for provider-managed user provisioning; we create users on the fly in Authenticate.
        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteUser(string username, CancellationToken cancellationToken)
    {
        // Optional: remove local Jellyfin user on external deletion. Leaving as no-op.
        return ValueTask.CompletedTask;
    }

    // No team → role mapping in this build.
}

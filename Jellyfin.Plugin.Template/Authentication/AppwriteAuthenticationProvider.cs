using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Appwrite;
using Appwrite.Services;
using MediaBrowser.Controller.Entities;
using Jellyfin.Plugin.Template.Appwrite;
using Jellyfin.Plugin.Template.Configuration;
using MediaBrowser.Controller.Authentication;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Logging;
// Use Jellyfin's AuthenticationException to signal auth failures to core.
using JfAuthException = MediaBrowser.Controller.Authentication.AuthenticationException;

namespace Jellyfin.Plugin.Template.Authentication;

/// <summary>
/// Lightweight Appwrite-backed authentication provider.
/// Keeps Jellyfin URLs unchanged; validates credentials via Appwrite.
/// Maps username/email to Jellyfin user name per login input.
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

    /// <summary>
    /// Gets the provider name.
    /// </summary>
    public string Name => "AppwriteAuth";

    /// <summary>
    /// Gets a value indicating whether the provider is enabled.
    /// </summary>
    public bool IsEnabled => true;

    /// <summary>
    /// Returns true to advertise password capability.
    /// </summary>
    /// <summary>
    /// Determines whether the user is considered to have a password.
    /// </summary>
    /// <param name="user">The user to evaluate.</param>
    /// <returns>Always true for this provider.</returns>
    public bool HasPassword(User user) => true;

    /// <summary>
    /// No-op; password changes should be managed via Appwrite.
    /// </summary>
    /// <summary>
    /// Changes the user's password (no-op; handled by Appwrite).
    /// </summary>
    /// <param name="user">The user to change.</param>
    /// <param name="newPassword">The new password.</param>
    /// <returns>A completed task.</returns>
    public Task ChangePassword(User user, string newPassword)
    {
        // Password changes are handled via Appwrite; no-op here.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Authenticate the user using Appwrite as the identity provider.
    /// </summary>
    /// <param name="username">The username or email supplied by the client.</param>
    /// <param name="password">The password supplied by the client.</param>
    /// <returns>A provider authentication result indicating success.</returns>
    public async Task<ProviderAuthenticationResult> Authenticate(string username, string password)
        => await AuthenticateCore(username, password, CancellationToken.None).ConfigureAwait(false);

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
                var isEmailLogin = username.Contains('@', StringComparison.Ordinal);
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
            var jfUsername = username.Contains('@', StringComparison.Ordinal) ? email! : username;
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

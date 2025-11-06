using System;
using System.Threading;
using System.Threading.Tasks;
using Appwrite;
using Appwrite.Services;
using Jellyfin.Plugin.AppwriteAuth.Configuration;

namespace Jellyfin.Plugin.AppwriteAuth.Appwrite;

/// <summary>
/// Minimal Appwrite account auth helper used to validate credentials.
/// </summary>
public class AppwriteAuthService
{
    private readonly Client _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppwriteAuthService"/> class.
    /// </summary>
    /// <param name="config">Plugin configuration.</param>
    public AppwriteAuthService(PluginConfiguration config)
    {
        _client = AppwriteClientFactory.Create(config);
    }

    /// <summary>
    /// Validates user credentials against Appwrite by attempting to create a session.
    /// Does not persist or return the Appwrite session; Jellyfin will maintain its own session.
    /// </summary>
    /// <param name="usernameOrEmail">The user's email or username.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True when credentials are valid; otherwise false.</returns>
    public async Task<bool> ValidateCredentialsAsync(string usernameOrEmail, string password, CancellationToken cancellationToken)
    {
        var account = new Account(_client);
        try
        {
            var session = await account.CreateEmailPasswordSession(usernameOrEmail, password).ConfigureAwait(false);
            try
            {
                await account.DeleteSession(session.Id).ConfigureAwait(false);
            }
            catch
            {
                // no-op
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    // Placeholder for team membership retrieval if needed for role mapping.
    // In practice you may want to use the Appwrite Admin (API key) client to inspect memberships
    // and map to Jellyfin roles.
}

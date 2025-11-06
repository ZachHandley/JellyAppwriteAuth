using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Appwrite;
using Appwrite.Services;
using Jellyfin.Plugin.AppwriteAuth.Appwrite;
using Jellyfin.Plugin.AppwriteAuth.Configuration;
using Jellyfin.Plugin.AppwriteAuth.Messaging;

namespace Jellyfin.Plugin.AppwriteAuth.Services;

public sealed class InvitationService
{
    private readonly PluginConfiguration _config;
    private readonly IEmailSender _emailSender;

    public InvitationService(PluginConfiguration config)
    {
        _config = config;
        _emailSender = config.EmailProvider == EmailProviderType.AppwriteMessaging
            ? new MessagingEmailSender(config)
            : new SmtpEmailSender(config);
    }

    public async Task InviteAsync(string email, CancellationToken cancellationToken)
    {
        var tempPassword = GenerateTempPassword();

        // Optional: Create user in Appwrite with temp password (requires Appwrite admin API key)
        if (!string.IsNullOrWhiteSpace(_config.AppwriteApiKey))
        {
            try
            {
                var client = AppwriteClientFactory.Create(_config);
                var users = new Users(client);
                // Create user with email + password; ignore if user already exists
                await users.CreatePHPassUser(ID.Unique(), email, tempPassword, email).ConfigureAwait(false);
            }
            catch
            {
                // Ignore creation errors (user might already exist). Admin can reset password.
            }
        }

        var preferInlineCid = _emailSender is not MessagingEmailSender; // CID for SMTP, data URL for Messaging
        var (html, inlineLogo) = EmailTemplateRenderer.RenderInvite(_config, email, tempPassword, preferInlineCid);
        await _emailSender.SendAsync(email, _config.InviteEmailSubject, html, inlineLogo, cancellationToken).ConfigureAwait(false);
    }

    public async Task ResetPasswordAsync(string email, CancellationToken cancellationToken)
    {
        var tempPassword = GenerateTempPassword();

        // Optional: Update user password in Appwrite (requires admin API key)
        if (!string.IsNullOrWhiteSpace(_config.AppwriteApiKey))
        {
            try
            {
                var client = AppwriteClientFactory.Create(_config);
                var users = new Users(client);
                // Find user and update password
                var list = await users.List(search: email).ConfigureAwait(false);
                foreach (var u in list.Users)
                {
                    if (string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase))
                    {
                        await users.UpdatePassword(u.Id, tempPassword).ConfigureAwait(false);
                        break;
                    }
                }
            }
            catch
            {
                // Ignore failure; still send email so admin can resolve manually.
            }
        }

        var preferInlineCid = _emailSender is not MessagingEmailSender;
        var (html, inlineLogo) = EmailTemplateRenderer.RenderReset(_config, email, tempPassword, preferInlineCid);
        await _emailSender.SendAsync(email, _config.ResetEmailSubject, html, inlineLogo, cancellationToken).ConfigureAwait(false);
    }

    private static string GenerateTempPassword()
    {
        // 16 bytes random -> base64url without padding
        var bytes = RandomNumberGenerator.GetBytes(16);
        var b64 = Convert.ToBase64String(bytes);
        return b64.Replace("+", "-", StringComparison.Ordinal).Replace("/", "_", StringComparison.Ordinal).TrimEnd('=');
    }
}

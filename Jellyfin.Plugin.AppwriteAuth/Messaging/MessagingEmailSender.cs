using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Appwrite;
using Appwrite.Enums;
using Appwrite.Services;
using Jellyfin.Plugin.AppwriteAuth.Appwrite;
using Jellyfin.Plugin.AppwriteAuth.Configuration;

namespace Jellyfin.Plugin.AppwriteAuth.Messaging;

/// <summary>
/// Sends emails via Appwrite Messaging. Ensures an SMTP provider exists in Appwrite
/// based on plugin SMTP settings. Falls back to SMTP sender if Messaging API is unavailable.
/// </summary>
public sealed class MessagingEmailSender : IEmailSender
{
    private const string ProviderId = "jellyfin_smtp";
    private readonly PluginConfiguration _config;
    private readonly SmtpEmailSender _fallback;

    public MessagingEmailSender(PluginConfiguration config)
    {
        _config = config;
        _fallback = new SmtpEmailSender(config);
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody, EmailAttachment? inlineLogo, CancellationToken cancellationToken)
    {
        try
        {
            var client = AppwriteClientFactory.Create(_config);
            var messaging = new global::Appwrite.Services.Messaging(client);
            var usersSvc = new global::Appwrite.Services.Users(client);

            await EnsureSmtpProviderAsync(messaging, cancellationToken).ConfigureAwait(false);

            var userId = await EnsureUserForEmailAsync(usersSvc, toEmail, cancellationToken).ConfigureAwait(false);
            var toUsers = new List<string> { userId };
            var messageId = ID.Unique();

            // Prefer HTML content
            await messaging.CreateEmail(
                messageId,
                subject,
                htmlBody,
                topics: null,
                users: toUsers,
                targets: null,
                cc: null,
                bcc: null,
                attachments: null,
                draft: null,
                html: true,
                scheduledAt: null).ConfigureAwait(false);

            return;
        }
        catch
        {
            await _fallback.SendAsync(toEmail, subject, htmlBody, inlineLogo, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task EnsureSmtpProviderAsync(global::Appwrite.Services.Messaging messaging, System.Threading.CancellationToken cancellationToken)
    {
        try
        {
            _ = await messaging.GetProvider(ProviderId).ConfigureAwait(false);
            return;
        }
        catch
        {
            var encryption = _config.SmtpEnableSsl ? SmtpEncryption.Ssl : SmtpEncryption.None;
            var fromName = string.IsNullOrWhiteSpace(_config.BrandName) ? "Jellyfin" : _config.BrandName;
#pragma warning disable CS0618
            await messaging.CreateSmtpProvider(
                providerId: ProviderId,
                name: "Jellyfin SMTP",
                host: _config.SmtpHost,
                port: _config.SmtpPort,
                username: _config.SmtpUsername,
                password: _config.SmtpPassword,
                encryption: encryption,
                autoTLS: true,
                mailer: string.Empty,
                fromName: fromName,
                fromEmail: _config.SmtpFrom,
                replyToName: string.Empty,
                replyToEmail: string.Empty,
                enabled: true).ConfigureAwait(false);
#pragma warning restore CS0618
        }
    }

    private static string GeneratePassword()
    {
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(16);
        var b64 = Convert.ToBase64String(bytes).TrimEnd('=');
        return b64.Replace('+', '-').Replace('/', '_');
    }

    private async Task<string> EnsureUserForEmailAsync(global::Appwrite.Services.Users usersSvc, string email, CancellationToken cancellationToken)
    {
        try
        {
            var list = await usersSvc.List(search: email).ConfigureAwait(false);
            var existing = list.Users?.Find(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                return existing.Id;
            }
        }
        catch
        {
            // ignore and create
        }

        var uid = ID.Unique();
        var pwd = GeneratePassword();
        var created = await usersSvc.CreatePHPassUser(uid, email, pwd, email).ConfigureAwait(false);
        return created.Id;
    }
}

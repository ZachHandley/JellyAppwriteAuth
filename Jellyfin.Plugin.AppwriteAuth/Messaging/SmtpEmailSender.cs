using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.AppwriteAuth.Configuration;

namespace Jellyfin.Plugin.AppwriteAuth.Messaging;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly PluginConfiguration _config;

    public SmtpEmailSender(PluginConfiguration config)
    {
        _config = config;
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody, EmailAttachment? inlineLogo, CancellationToken cancellationToken)
    {
        using var message = new MailMessage();
        message.From = new MailAddress(_config.SmtpFrom);
        message.To.Add(new MailAddress(toEmail));
        message.Subject = subject;
        message.IsBodyHtml = true;

        if (inlineLogo == null)
        {
            message.Body = htmlBody;
        }
        else
        {
            var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            var logoRes = new LinkedResource(new MemoryStream(inlineLogo.Bytes.ToArray()), inlineLogo.MediaType)
            {
                ContentId = inlineLogo.ContentId
            };
            htmlView.LinkedResources.Add(logoRes);
            message.AlternateViews.Add(htmlView);
        }

        using var client = new SmtpClient(_config.SmtpHost, _config.SmtpPort)
        {
            EnableSsl = _config.SmtpEnableSsl
        };

        if (!string.IsNullOrWhiteSpace(_config.SmtpUsername))
        {
            client.Credentials = new NetworkCredential(_config.SmtpUsername, _config.SmtpPassword);
        }

        // SmtpClient has no async send prior to newer APIs; wrap in Task.Run for plugin use.
        await Task.Run(() => client.Send(message), cancellationToken).ConfigureAwait(false);
    }
}

using System.Threading;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.Template.Messaging;

public interface IEmailSender
{
    Task SendAsync(string toEmail, string subject, string htmlBody, EmailAttachment? inlineLogo, CancellationToken cancellationToken);
}

public sealed class EmailAttachment
{
    public EmailAttachment(string contentId, string mediaType, byte[] bytes)
    {
        ContentId = contentId;
        MediaType = mediaType;
        Bytes = new System.ReadOnlyMemory<byte>(bytes);
    }

    public string ContentId { get; }

    public string MediaType { get; }

    public System.ReadOnlyMemory<byte> Bytes { get; }
}

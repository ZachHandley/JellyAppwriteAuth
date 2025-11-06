using System;
using System.IO;
using System.Text.RegularExpressions;
using Jellyfin.Plugin.Template.Configuration;

namespace Jellyfin.Plugin.Template.Messaging;

public static class EmailTemplateRenderer
{
    public static (string Html, EmailAttachment? InlineLogo) RenderInvite(PluginConfiguration cfg, string email, string tempPassword, bool preferInlineCid = true)
        => Render(cfg, cfg.InviteEmailHtml, email, tempPassword, preferInlineCid);

    public static (string Html, EmailAttachment? InlineLogo) RenderReset(PluginConfiguration cfg, string email, string tempPassword, bool preferInlineCid = true)
        => Render(cfg, cfg.ResetEmailHtml, email, tempPassword, preferInlineCid);

    private static (string Html, EmailAttachment? InlineLogo) Render(PluginConfiguration cfg, string template, string email, string tempPassword, bool preferInlineCid)
    {
        var primaryColor = string.IsNullOrWhiteSpace(cfg.BrandPrimaryColor) ? "#6C63FF" : cfg.BrandPrimaryColor.Trim();
        var brandName = string.IsNullOrWhiteSpace(cfg.BrandName) ? "Jellyfin" : cfg.BrandName.Trim();
        var loginUrl = string.IsNullOrWhiteSpace(cfg.LoginUrl) ? string.Empty : cfg.LoginUrl.Trim();

        string logoRef = cfg.LogoUrlOrPath?.Trim() ?? string.Empty;
        EmailAttachment? inlineLogo = null;

        if (!string.IsNullOrWhiteSpace(logoRef) && File.Exists(logoRef))
        {
            var bytes = File.ReadAllBytes(cfg.LogoUrlOrPath!);
            var mediaType = GuessMediaType(cfg.LogoUrlOrPath!);
            if (preferInlineCid)
            {
                // Embed inline via CID (SMTP-friendly)
                var contentId = "logoImage";
                logoRef = $"cid:{contentId}";
                inlineLogo = new EmailAttachment(contentId, mediaType, bytes);
            }
            else
            {
                // Embed as data URL (works with most HTML email renderers)
                var b64 = Convert.ToBase64String(bytes);
                logoRef = $"data:{mediaType};base64,{b64}";
            }
        }

        var html = template
            .Replace("{{brandName}}", Escape(brandName), StringComparison.Ordinal)
            .Replace("{{primaryColor}}", Escape(primaryColor), StringComparison.Ordinal)
            .Replace("{{loginUrl}}", Escape(loginUrl), StringComparison.Ordinal)
            .Replace("{{email}}", Escape(email), StringComparison.Ordinal)
            .Replace("{{password}}", Escape(tempPassword), StringComparison.Ordinal)
            .Replace("{{logo}}", Escape(logoRef), StringComparison.Ordinal);

        return (html, inlineLogo);
    }

    private static string Escape(string value)
    {
        // Minimal HTML attribute/content escape
        return value
            .Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal)
            .Replace("\"", "&quot;", StringComparison.Ordinal);
    }

    private static string GuessMediaType(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream"
        };
    }
}

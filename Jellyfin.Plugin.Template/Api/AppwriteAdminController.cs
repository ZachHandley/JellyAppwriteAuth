using System;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Template.Api.Models;
using Jellyfin.Plugin.Template.Configuration;
using Jellyfin.Plugin.Template.Messaging;
using Jellyfin.Plugin.Template.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.Template.Api;

/// <summary>
/// Admin-only endpoints for invite/reset/test and helper operations.
/// </summary>
[ApiController]
[Route("Plugin/AppwriteAuth")]
public class AppwriteAdminController : ControllerBase
{
    private bool IsAdmin() => User?.IsInRole("Administrator") == true;

    /// <summary>
    /// Sends an invite email to the specified address (and creates the Appwrite user if needed).
    /// </summary>
    /// <param name="req">Email payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP 200 if sent.</returns>
    [HttpPost("invite")]
    [Authorize]
    public async Task<IActionResult> Invite([FromBody] Models.EmailRequest req, CancellationToken cancellationToken)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        if (req == null || string.IsNullOrWhiteSpace(req.Email))
        {
            return BadRequest("Email is required.");
        }

        var cfg = Plugin.Instance?.Configuration;
        if (cfg == null)
        {
            return StatusCode(500, "Plugin configuration not available.");
        }

        var svc = new InvitationService(cfg);
        await svc.InviteAsync(req.Email!, cancellationToken).ConfigureAwait(false);
        return Ok(new { status = "ok" });
    }

    /// <summary>
    /// Resets the user's password and sends a reset email.
    /// </summary>
    /// <param name="req">Email payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP 200 if sent.</returns>
    [HttpPost("reset")]
    [Authorize]
    public async Task<IActionResult> Reset([FromBody] Models.EmailRequest req, CancellationToken cancellationToken)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        if (req == null || string.IsNullOrWhiteSpace(req.Email))
        {
            return BadRequest("Email is required.");
        }

        var cfg = Plugin.Instance?.Configuration;
        if (cfg == null)
        {
            return StatusCode(500, "Plugin configuration not available.");
        }

        var svc = new InvitationService(cfg);
        await svc.ResetPasswordAsync(req.Email!, cancellationToken).ConfigureAwait(false);
        return Ok(new { status = "ok" });
    }

    /// <summary>
    /// Sends a test email to validate configuration.
    /// </summary>
    /// <param name="req">Email payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP 200 if sent.</returns>
    [HttpPost("test")]
    [Authorize]
    public async Task<IActionResult> Test([FromBody] Models.EmailRequest req, CancellationToken cancellationToken)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        if (req == null || string.IsNullOrWhiteSpace(req.Email))
        {
            return BadRequest("Email is required.");
        }

        var cfg = Plugin.Instance?.Configuration;
        if (cfg == null)
        {
            return StatusCode(500, "Plugin configuration not available.");
        }

        var sender = cfg.EmailProvider == EmailProviderType.AppwriteMessaging
            ? (IEmailSender)new MessagingEmailSender(cfg)
            : new SmtpEmailSender(cfg);

        var preferInlineCid = sender is not MessagingEmailSender;
        var (html, inlineLogo) = Messaging.EmailTemplateRenderer.RenderInvite(cfg, req.Email!, "test-temp-pass", preferInlineCid);
        await sender.SendAsync(req.Email!, cfg.InviteEmailSubject, html, inlineLogo, cancellationToken).ConfigureAwait(false);
        return Ok(new { status = "ok" });
    }

    /// <summary>
    /// Resolves Appwrite email for a given Jellyfin user name (assumes Jellyfin user name = Appwrite id or email).
    /// </summary>
    /// <param name="userName">The Jellyfin user name (Appwrite id or email).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP 200 with email mapping or null.</returns>
    [HttpGet("resolve-email")]
    [Authorize]
    public async Task<IActionResult> ResolveEmail([FromQuery] string userName, CancellationToken cancellationToken)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            return BadRequest("userName is required");
        }

        var cfg = Plugin.Instance?.Configuration;
        if (cfg == null)
        {
            return StatusCode(500, "Plugin configuration not available.");
        }

        try
        {
            var client = Jellyfin.Plugin.Template.Appwrite.AppwriteClientFactory.Create(cfg);
            var users = new global::Appwrite.Services.Users(client);
            var u = await users.Get(userName).ConfigureAwait(false);
            return Ok(new { email = u.Email });
        }
        catch
        {
            return Ok(new { email = (string?)null });
        }
    }

    // EmailRequest moved to Api/Models/EmailRequest.cs for clarity.
}

using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.AppwriteAuth.Configuration;

/// <summary>
/// Email sender provider type.
/// </summary>
public enum EmailProviderType
{
    Smtp,
    AppwriteMessaging
}

/// <summary>
/// Plugin configuration for AppwriteAuth.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        AppwriteEndpoint = string.Empty;
        AppwriteProjectId = string.Empty;
        AppwriteApiKey = string.Empty;

        EmailProvider = EmailProviderType.AppwriteMessaging;
        SmtpHost = string.Empty;
        SmtpPort = 587;
        SmtpUsername = string.Empty;
        SmtpPassword = string.Empty;
        SmtpFrom = string.Empty;
        SmtpEnableSsl = true;

        BrandName = "Jellyfin";
        LogoUrlOrPath = "https://appwrite.io/logo.svg";
        BrandPrimaryColor = "#6C63FF";
        LoginUrl = string.Empty;

        InviteEmailSubject = "You're invited to Jellyfin";
        InviteEmailHtml = DefaultInviteHtml;

        ResetEmailSubject = "Your Jellyfin password has been reset";
        ResetEmailHtml = DefaultResetHtml;

        MarkEmailVerifiedOnLogin = true;
    }

    /// <summary>
    /// Gets or sets the Appwrite API endpoint, e.g. https://cloud.appwrite.io/v1.
    /// </summary>
    public string AppwriteEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the Appwrite Project ID.
    /// </summary>
    public string AppwriteProjectId { get; set; }

    /// <summary>
    /// Gets or sets the optional Appwrite API key for server-side access.
    /// </summary>
    public string? AppwriteApiKey { get; set; }

    /// <summary>
    /// Gets or sets which email provider to use for sending invites and reset emails.
    /// </summary>
    public EmailProviderType EmailProvider { get; set; }

    /// <summary>
    /// Gets or sets the SMTP host for email sending (used when <see cref="EmailProvider"/> is <see cref="EmailProviderType.Smtp"/>).
    /// </summary>
    public string SmtpHost { get; set; }

    /// <summary>
    /// Gets or sets the SMTP port for email sending (default 587).
    /// </summary>
    public int SmtpPort { get; set; }

    /// <summary>
    /// Gets or sets the SMTP username.
    /// </summary>
    public string SmtpUsername { get; set; }

    /// <summary>
    /// Gets or sets the SMTP password.
    /// </summary>
    public string SmtpPassword { get; set; }

    /// <summary>
    /// Gets or sets the from address for emails.
    /// </summary>
    public string SmtpFrom { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether SSL/TLS is enabled for SMTP.
    /// </summary>
    public bool SmtpEnableSsl { get; set; }

    /// <summary>
    /// Gets or sets the brand name used in email templates.
    /// </summary>
    public string BrandName { get; set; }

    /// <summary>
    /// Gets or sets the logo URL or server path used in email templates.
    /// </summary>
    public string LogoUrlOrPath { get; set; }

    /// <summary>
    /// Gets or sets the primary brand color in HEX (e.g. #6C63FF).
    /// </summary>
    public string BrandPrimaryColor { get; set; }

    /// <summary>
    /// Gets or sets the login URL used in email templates.
    /// </summary>
    public string LoginUrl { get; set; }

    /// <summary>
    /// Gets or sets the invite email subject.
    /// </summary>
    public string InviteEmailSubject { get; set; }

    /// <summary>
    /// Gets or sets the invite email HTML template.
    /// Tokens: {{brandName}}, {{email}}, {{password}}, {{loginUrl}}, {{logo}}.
    /// </summary>
    public string InviteEmailHtml { get; set; }

    /// <summary>
    /// Gets or sets the reset email subject.
    /// </summary>
    public string ResetEmailSubject { get; set; }

    /// <summary>
    /// Gets or sets the reset email HTML template.
    /// Tokens: {{brandName}}, {{email}}, {{password}}, {{loginUrl}}, {{logo}}.
    /// </summary>
    public string ResetEmailHtml { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether on successful login the plugin should set the user's Appwrite emailVerification to true.
    /// Requires Appwrite API key.
    /// </summary>
    public bool MarkEmailVerifiedOnLogin { get; set; }

    #pragma warning disable SA1201
    private const string DefaultInviteHtml = @"<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1'>
  <title>Invite</title>
  <style>
    body { background:#f6f7fb; margin:0; padding:0; font-family:Segoe UI, Roboto, Helvetica, Arial, sans-serif; color:#1f2937; }
    .container { max-width:560px; margin:24px auto; background:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 6px 24px rgba(0,0,0,0.08); }
    .header { padding:20px 24px; background:{{primaryColor}}; color:#fff; display:flex; align-items:center; gap:12px; }
    .header img { height:28px; }
    .brand { font-size:18px; font-weight:600; letter-spacing:.2px; }
    .content { padding:24px; }
    h1 { margin:0 0 12px; font-size:20px; color:#111827; }
    p { margin:0 0 12px; line-height:1.6; }
    .kbd { display:inline-block; padding:8px 10px; background:#111827; color:#fff; border-radius:8px; font-family:ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace; font-size:14px; }
    .button { display:inline-block; padding:12px 18px; background:{{primaryColor}}; color:#fff; text-decoration:none; border-radius:10px; font-weight:600; letter-spacing:.2px; }
    .footer { padding:14px 24px 22px; color:#6b7280; font-size:12px; }
  </style>
  <!-- logo: {{logo}} -->
  <!-- brand: {{brandName}} -->
  <!-- primary: {{primaryColor}} -->
  <!-- loginUrl: {{loginUrl}} -->
  <!-- email: {{email}} -->
  <!-- password: {{password}} -->
  <!-- If logo is a local path, it will be embedded inline. -->
</head>
<body>
  <div class='container'>
    <div class='header'>
      <img src='{{logo}}' alt='{{brandName}} logo' />
      <div class='brand'>{{brandName}}</div>
    </div>
    <div class='content'>
      <h1>Welcome to {{brandName}}</h1>
      <p>You have been invited to access our Jellyfin server.</p>
      <p>Use these temporary credentials to log in and change your password after first sign-in:</p>
      <p><span class='kbd'>{{email}}</span></p>
      <p><span class='kbd'>{{password}}</span></p>
      <p style='margin-top:18px'>
        <a class='button' href='{{loginUrl}}' target='_blank' rel='noopener'>Go to Login</a>
      </p>
      <p>If the button doesn’t work, copy and paste this link:<br />{{loginUrl}}</p>
    </div>
    <div class='footer'>
      You’re receiving this because an administrator invited you. If you weren’t expecting this, you can safely ignore this email.
    </div>
  </div>
</body>
</html>";

    private const string DefaultResetHtml = @"<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1'>
  <title>Password Reset</title>
  <style>
    body { background:#f6f7fb; margin:0; padding:0; font-family:Segoe UI, Roboto, Helvetica, Arial, sans-serif; color:#1f2937; }
    .container { max-width:560px; margin:24px auto; background:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 6px 24px rgba(0,0,0,0.08); }
    .header { padding:20px 24px; background:{{primaryColor}}; color:#fff; display:flex; align-items:center; gap:12px; }
    .header img { height:28px; }
    .brand { font-size:18px; font-weight:600; letter-spacing:.2px; }
    .content { padding:24px; }
    h1 { margin:0 0 12px; font-size:20px; color:#111827; }
    p { margin:0 0 12px; line-height:1.6; }
    .kbd { display:inline-block; padding:8px 10px; background:#111827; color:#fff; border-radius:8px; font-family:ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace; font-size:14px; }
    .button { display:inline-block; padding:12px 18px; background:{{primaryColor}}; color:#fff; text-decoration:none; border-radius:10px; font-weight:600; letter-spacing:.2px; }
    .footer { padding:14px 24px 22px; color:#6b7280; font-size:12px; }
  </style>
  <!-- logo: {{logo}} -->
  <!-- brand: {{brandName}} -->
  <!-- primary: {{primaryColor}} -->
  <!-- loginUrl: {{loginUrl}} -->
  <!-- email: {{email}} -->
  <!-- password: {{password}} -->
  <!-- If logo is a local path, it will be embedded inline. -->
</head>
<body>
  <div class='container'>
    <div class='header'>
      <img src='{{logo}}' alt='{{brandName}} logo' />
      <div class='brand'>{{brandName}}</div>
    </div>
    <div class='content'>
      <h1>Password Reset</h1>
      <p>Your password has been reset by an administrator. Use the temporary credentials below to log in and change your password immediately:</p>
      <p><span class='kbd'>{{email}}</span></p>
      <p><span class='kbd'>{{password}}</span></p>
      <p style='margin-top:18px'>
        <a class='button' href='{{loginUrl}}' target='_blank' rel='noopener'>Go to Login</a>
      </p>
      <p>If the button doesn’t work, copy and paste this link:<br />{{loginUrl}}</p>
    </div>
    <div class='footer'>
      If you didn’t request this change, contact an administrator.
    </div>
  </div>
</body>
    </html>";
    #pragma warning restore SA1201
}

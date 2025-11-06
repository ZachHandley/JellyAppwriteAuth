/**
 * Plugin configuration types matching C# PluginConfiguration model
 */

/**
 * Email provider type enum
 */
export enum EmailProviderType {
  Smtp = 0,
  AppwriteMessaging = 1,
}

/**
 * Plugin configuration interface
 */
export interface PluginConfiguration {
  /** Appwrite API endpoint, e.g. https://cloud.appwrite.io/v1 */
  AppwriteEndpoint: string;

  /** Appwrite Project ID */
  AppwriteProjectId: string;

  /** Appwrite API key for server-side access (optional) */
  AppwriteApiKey?: string;

  /** Email provider to use for sending invites and resets */
  EmailProvider: EmailProviderType;

  /** SMTP host (used when EmailProvider is SMTP) */
  SmtpHost: string;

  /** SMTP port (default 587) */
  SmtpPort: number;

  /** SMTP username */
  SmtpUsername: string;

  /** SMTP password */
  SmtpPassword: string;

  /** From address for emails */
  SmtpFrom: string;

  /** Whether SSL/TLS is enabled for SMTP */
  SmtpEnableSsl: boolean;

  /** Brand name used in email templates */
  BrandName: string;

  /** Logo URL or server path used in email templates */
  LogoUrlOrPath: string;

  /** Primary brand color in HEX (e.g. #6C63FF) */
  BrandPrimaryColor: string;

  /** Login URL used in email templates */
  LoginUrl: string;

  /** Invite email subject */
  InviteEmailSubject: string;

  /** Invite email HTML template. Tokens: {{brandName}}, {{email}}, {{password}}, {{loginUrl}}, {{logo}} */
  InviteEmailHtml: string;

  /** Reset email subject */
  ResetEmailSubject: string;

  /** Reset email HTML template. Tokens: {{brandName}}, {{email}}, {{password}}, {{loginUrl}}, {{logo}} */
  ResetEmailHtml: string;

  /** Whether to mark email as verified on successful login (requires Appwrite API key) */
  MarkEmailVerifiedOnLogin: boolean;
}

/**
 * Default configuration values
 */
export const DEFAULT_CONFIG: Partial<PluginConfiguration> = {
  EmailProvider: EmailProviderType.AppwriteMessaging,
  SmtpPort: 587,
  SmtpEnableSsl: true,
  BrandName: 'Jellyfin',
  LogoUrlOrPath: 'https://appwrite.io/logo.svg',
  BrandPrimaryColor: '#6C63FF',
  InviteEmailSubject: "You're invited to Jellyfin",
  ResetEmailSubject: 'Your Jellyfin password has been reset',
  MarkEmailVerifiedOnLogin: true,
};

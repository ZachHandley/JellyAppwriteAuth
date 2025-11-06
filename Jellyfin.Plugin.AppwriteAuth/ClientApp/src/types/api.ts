/**
 * API request and response types
 */

/**
 * Email request for invite/reset/test operations
 */
export interface EmailRequest {
  email: string;
}

/**
 * Response from resolve-email endpoint
 */
export interface ResolveEmailResponse {
  email: string | null;
}

/**
 * Jellyfin user model
 */
export interface JellyfinUser {
  Name: string;
  Id: string;
  ServerId?: string;
  HasPassword?: boolean;
  HasConfiguredPassword?: boolean;
  HasConfiguredEasyPassword?: boolean;
  EnableAutoLogin?: boolean;
  LastLoginDate?: string;
  LastActivityDate?: string;
  Configuration?: {
    AudioLanguagePreference?: string;
    PlayDefaultAudioTrack?: boolean;
    SubtitleLanguagePreference?: string;
    DisplayMissingEpisodes?: boolean;
    GroupedFolders?: string[];
    SubtitleMode?: string;
    DisplayCollectionsView?: boolean;
    EnableLocalPassword?: boolean;
    OrderedViews?: string[];
    LatestItemsExcludes?: string[];
    MyMediaExcludes?: string[];
    HidePlayedInLatest?: boolean;
    RememberAudioSelections?: boolean;
    RememberSubtitleSelections?: boolean;
    EnableNextEpisodeAutoPlay?: boolean;
  };
  Policy?: {
    IsAdministrator?: boolean;
    IsHidden?: boolean;
    IsDisabled?: boolean;
    MaxParentalRating?: number;
    BlockedTags?: string[];
    EnableUserPreferenceAccess?: boolean;
    AccessSchedules?: unknown[];
    BlockUnratedItems?: unknown[];
    EnableRemoteControlOfOtherUsers?: boolean;
    EnableSharedDeviceControl?: boolean;
    EnableRemoteAccess?: boolean;
    EnableLiveTvManagement?: boolean;
    EnableLiveTvAccess?: boolean;
    EnableMediaPlayback?: boolean;
    EnableAudioPlaybackTranscoding?: boolean;
    EnableVideoPlaybackTranscoding?: boolean;
    EnablePlaybackRemuxing?: boolean;
    ForceRemoteSourceTranscoding?: boolean;
    EnableContentDeletion?: boolean;
    EnableContentDeletionFromFolders?: string[];
    EnableContentDownloading?: boolean;
    EnableSyncTranscoding?: boolean;
    EnableMediaConversion?: boolean;
    EnabledDevices?: string[];
    EnableAllDevices?: boolean;
    EnabledChannels?: string[];
    EnableAllChannels?: boolean;
    EnabledFolders?: string[];
    EnableAllFolders?: boolean;
    InvalidLoginAttemptCount?: number;
    LoginAttemptsBeforeLockout?: number;
    MaxActiveSessions?: number;
    EnablePublicSharing?: boolean;
    BlockedMediaFolders?: string[];
    BlockedChannels?: string[];
    RemoteClientBitrateLimit?: number;
    AuthenticationProviderId?: string;
    PasswordResetProviderId?: string;
    SyncPlayAccess?: string;
  };
}

/**
 * API error response
 */
export interface ApiError {
  message: string;
  statusCode?: number;
}

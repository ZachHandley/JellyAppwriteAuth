/**
 * Jellyfin global types for window.ApiClient integration
 */

declare global {
  interface Window {
    ApiClient: {
      /**
       * Get the current access token (newer Jellyfin versions)
       */
      accessToken?: () => string;
      /**
       * Direct access to token (older Jellyfin versions)
       */
      _accessToken?: string;
      /**
       * Construct full URL from relative path
       */
      getUrl: (path: string) => string;
      /**
       * Get current server address
       */
      serverAddress?: () => string;
    };
    Dashboard: {
      /**
       * Show loading indicator
       */
      showLoadingMsg: () => void;
      /**
       * Hide loading indicator
       */
      hideLoadingMsg: () => void;
    };
  }
}

export {};

/**
 * Data-controller wrapper for Jellyfin plugin system
 * This file implements the pattern Jellyfin expects: export default function(view)
 * It loads the Vue UMD bundle and manually mounts the app
 */
export default function(view) {
    console.log('[AppwriteAuth] Data-controller initializing...', view);

    /**
     * Initialize the Vue app once the bundle is loaded
     */
    const initVueApp = () => {
        console.log('[AppwriteAuth] Attempting to mount Vue app...');

        // Check if the Vue bundle has loaded and exposed the API
        if (window.AppwriteAuthUI && typeof window.AppwriteAuthUI.mount === 'function') {
            // Find the mount target within the controller's view
            const mountTarget = view.querySelector('#appwrite-auth-root');

            if (mountTarget) {
                console.log('[AppwriteAuth] Mount target found, mounting Vue app...');
                window.AppwriteAuthUI.mount('#appwrite-auth-root');
            } else {
                console.error('[AppwriteAuth] Mount target #appwrite-auth-root not found in view');
            }
        } else {
            console.error('[AppwriteAuth] Vue bundle not loaded or AppwriteAuthUI not available');
        }
    };

    /**
     * Load the Vue UMD bundle dynamically
     */
    const loadVueBundle = () => {
        // Check if already loaded
        if (window.AppwriteAuthUI) {
            console.log('[AppwriteAuth] Vue bundle already loaded');
            initVueApp();
            return;
        }

        console.log('[AppwriteAuth] Loading Vue bundle...');

        const script = document.createElement('script');
        script.type = 'text/javascript';
        script.src = 'appwriteauthbundle'; // References the PluginPageInfo Name

        script.onload = () => {
            console.log('[AppwriteAuth] Vue bundle loaded successfully');
            initVueApp();
        };

        script.onerror = (error) => {
            console.error('[AppwriteAuth] Failed to load Vue bundle:', error);
        };

        document.head.appendChild(script);
    };

    // Start loading
    loadVueBundle();

    /**
     * Cleanup when page is hidden/navigated away
     */
    const cleanup = () => {
        console.log('[AppwriteAuth] Cleaning up Vue app...');
        if (window.AppwriteAuthUI && typeof window.AppwriteAuthUI.unmount === 'function') {
            window.AppwriteAuthUI.unmount();
        }
    };

    // Register cleanup handlers
    window.addEventListener('pagehide', cleanup);

    // Also cleanup on beforeunload for better compatibility
    window.addEventListener('beforeunload', cleanup);
}

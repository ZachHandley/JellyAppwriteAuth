import { createApp } from 'vue';
import type { App as VueApp } from 'vue';
import App from './App.vue';
import './assets/styles/main.css';

let appInstance: VueApp | null = null;

/**
 * Mount the Vue app to a specific DOM selector
 * @param selector - CSS selector for the mount target
 */
export function mountApp(selector: string): void {
  const el = document.querySelector(selector);
  if (el) {
    appInstance = createApp(App);
    appInstance.mount(el);
  } else {
    console.error(`[AppwriteAuth] Mount target "${selector}" not found`);
  }
}

/**
 * Unmount the Vue app if it's currently mounted
 */
export function unmountApp(): void {
  if (appInstance) {
    appInstance.unmount();
    appInstance = null;
  }
}

// Auto-mount when loaded in Jellyfin (for Custom Tabs or direct injection)
if (typeof window !== 'undefined') {
  // Wait for DOM to be ready
  const initApp = () => {
    const target = document.querySelector('#appwrite-auth-root');
    if (target) {
      mountApp('#appwrite-auth-root');
    }
  };

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initApp);
  } else {
    // DOM already loaded
    initApp();
  }
}

// Expose to window for manual mounting if needed
declare global {
  interface Window {
    AppwriteAuthUI?: {
      mount: typeof mountApp;
      unmount: typeof unmountApp;
    };
  }
}

if (typeof window !== 'undefined') {
  window.AppwriteAuthUI = {
    mount: mountApp,
    unmount: unmountApp,
  };
}

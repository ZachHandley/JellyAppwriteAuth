# AppwriteAuth Custom Tab Setup

This guide explains how to set up the AppwriteAuth Vue 3 SPA with the required Custom Tabs plugin dependency.

## ⚠️ Important: Required Dependencies

AppwriteAuth requires the following plugins to be installed **in this order**:

1. **File Transformation** (v2.2.1.0 or newer)
2. **Custom Tabs** (v0.2.2.0 or newer)
3. **AppwriteAuth** (this plugin)

**Why?** AppwriteAuth uses Custom Tabs to add an "Auth Settings" navigation item to your Jellyfin sidebar. Custom Tabs itself requires File Transformation to modify Jellyfin's web interface.

---

## Installation Guide

### Step 1: Add Plugin Repositories

1. Navigate to **Dashboard → Plugins → Repositories**
2. Click **Add Repository**
3. Add the Custom Tabs repository:
   ```
   Repository Name: IAmParadox27 Repository
   Repository URL: https://www.iamparadox.dev/jellyfin/plugins/manifest.json
   ```
4. Click **Save**

### Step 2: Install Dependencies (In Order!)

#### 2a. Install File Transformation Plugin

1. Navigate to **Dashboard → Plugins → Catalog**
2. Search for "File Transformation"
3. Click **Install**
4. Restart Jellyfin when prompted

#### 2b. Install Custom Tabs Plugin

1. Navigate to **Dashboard → Plugins → Catalog**
2. Search for "Custom Tabs"
3. Click **Install**
4. Restart Jellyfin when prompted

#### 2c. Install AppwriteAuth Plugin

1. Navigate to **Dashboard → Plugins → Catalog** (or install manually)
2. Search for "AppwriteAuth"
3. Click **Install**
4. Restart Jellyfin when prompted

**Note**: If AppwriteAuth doesn't appear in the catalog, you may need to add your repository URL or install manually from the DLL.

### Step 3: Configure Custom Tab

1. Navigate to **Dashboard → Plugins → Custom Tabs → Settings**
2. Click **Add New Tab**
3. Fill in the following details:

   **Tab Configuration:**
   ```
   Tab Name: Auth Settings
   Tab Icon: security (or any Material Design icon name)
   Tab Order: 100 (adjust as needed for positioning)
   ```

4. In the **Tab HTML Content** field, paste this code:

   ```html
   <div id="appwrite-auth-root" style="min-height: 100vh; width: 100%;"></div>
   <script src="/Plugin/AppwriteAuth/js/appwrite-auth.js"></script>
   ```

5. Click **Save**

### Step 4: Verify Installation

1. Check that all three plugins are installed and active:
   - **Dashboard → Plugins → My Plugins**
   - You should see: File Transformation, Custom Tabs, and AppwriteAuth

2. Refresh your Jellyfin dashboard (hard refresh: Ctrl+F5 or Cmd+Shift+R)

3. Look for the **"Auth Settings"** tab in your navigation bar

4. Click the tab to access the AppwriteAuth management interface

**Troubleshooting**: If the tab doesn't appear:
- Verify all three plugins show as "Active" in My Plugins
- Check Jellyfin logs for any plugin loading errors
- Ensure Custom Tabs configuration was saved properly
- Try restarting Jellyfin again

---

## Alternative: Plugin Configuration Page (Fallback)

If you experience issues with Custom Tabs or prefer the traditional approach, you can still access the Vue SPA:

1. Navigate to **Dashboard → Plugins**
2. Find **AppwriteAuth** in the list
3. Click the three-dot menu → **Configure**
4. The Vue SPA will load in the configuration page

**Note**: This method still works without Custom Tabs, but you won't have the dedicated navigation tab.

---

## Manual Integration (Advanced)

If you want to embed the AppwriteAuth UI in a custom page or script, you can use the JavaScript API:

```html
<!-- Create a mount point -->
<div id="my-auth-container"></div>

<!-- Load the bundle -->
<script src="/Plugin/AppwriteAuth/js/appwrite-auth.js"></script>

<!-- Manually mount the app -->
<script>
  // Auto-mount to #appwrite-auth-root
  // OR manually mount:
  window.AppwriteAuthUI.mount('#my-auth-container');

  // To unmount:
  // window.AppwriteAuthUI.unmount();
</script>
```

---

## Troubleshooting

### Tab doesn't appear after configuration

1. Hard refresh your browser (Ctrl+F5 or Cmd+Shift+R)
2. Clear browser cache
3. Verify Custom Tabs plugin is installed and active
4. Check Jellyfin logs for errors

### JavaScript bundle not loading (404 error)

1. Ensure AppwriteAuth plugin is properly installed
2. Restart Jellyfin server
3. Check that `/Plugin/AppwriteAuth/js/appwrite-auth.js` is accessible
4. Verify the plugin DLL includes the embedded resource

### Vue app shows loading spinner indefinitely

1. Check browser console for JavaScript errors
2. Verify Jellyfin API is accessible
3. Ensure you're logged in as an administrator
4. Check network tab for failed API requests

### Dark mode doesn't match Jellyfin theme

The Vue SPA automatically detects Jellyfin's dark mode class. If it's not matching:

1. Ensure Jellyfin's theme is set correctly
2. Try manually toggling Jellyfin theme
3. Check browser console for CSS loading issues

---

## Features

The AppwriteAuth Vue SPA provides:

- **Appwrite Configuration**: Set endpoint, project ID, and API key
- **Email Provider Settings**: Configure SMTP or Appwrite Messaging
- **Email Templates**: Customize invite and reset email HTML/subject
- **Branding**: Set logo, primary color, and brand name
- **Admin Actions**:
  - Send invite emails to new users
  - Reset user passwords
  - Test email configuration
  - Search Jellyfin users
  - Resolve user emails

---

## Development

### Local Development

To work on the Vue SPA locally:

```bash
cd Jellyfin.Plugin.AppwriteAuth/ClientApp
pnpm install
pnpm run dev
```

The dev server will start at `http://localhost:5173` with:
- Hot Module Replacement (HMR)
- TypeScript type checking
- Tailwind CSS compilation
- API proxy to Jellyfin (http://localhost:8096)

### Building

```bash
cd Jellyfin.Plugin.AppwriteAuth/ClientApp
pnpm run build
```

This generates `wwwroot/appwrite-auth.js` which is automatically embedded in the plugin DLL.

### Production Build

The MSBuild process automatically builds the Vue SPA before compiling the C# plugin:

```bash
dotnet build --configuration Release
```

---

## Architecture

### Bundle Structure

- **Single JavaScript File**: All Vue components, TypeScript code, and CSS are bundled into one file
- **UMD Format**: Compatible with script tag loading
- **Embedded Resource**: Served from the plugin DLL, no external files needed
- **Cache Headers**: 1-hour client-side cache for performance

### API Integration

The Vue SPA communicates with Jellyfin via:

1. **Authentication**: Extracts `X-Emby-Token` from `window.ApiClient`
2. **Plugin Config**: `GET/POST /PluginConfiguration/{id}`
3. **Admin Actions**: `POST /Plugin/AppwriteAuth/{invite,reset,test}`
4. **User Search**: `GET /Users`
5. **Email Resolution**: `GET /Plugin/AppwriteAuth/resolve-email`

### Security

- All API calls require Jellyfin authentication
- Admin actions require administrator role
- No Appwrite credentials exposed to frontend
- All Appwrite operations proxied through C# backend
- XSS protection via Vue template escaping

---

## Support

If you encounter issues:

1. Check Jellyfin logs: **Dashboard → Logs**
2. Check browser console for JavaScript errors
3. Verify plugin version matches documentation
4. Create an issue on GitHub with logs and error details

---

## License

This plugin is open source. See LICENSE file for details.

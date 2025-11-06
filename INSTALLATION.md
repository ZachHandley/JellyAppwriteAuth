# AppwriteAuth Installation Guide

## Quick Installation Checklist

Follow these steps in order to install AppwriteAuth with all required dependencies.

### ✅ Prerequisites

- [ ] Jellyfin 10.11.0 or newer
- [ ] Administrator access to Jellyfin
- [ ] Appwrite instance (cloud or self-hosted)

### ✅ Step 1: Add Plugin Repository (Optional)

If installing from a custom repository:

```
Dashboard → Plugins → Repositories → Add Repository

Repository Name: IAmParadox27 Repository
Repository URL: https://www.iamparadox.dev/jellyfin/plugins/manifest.json
```

### ✅ Step 2: Install Required Plugins (In Order!)

Install these plugins in the specified order:

#### 1. File Transformation (v2.2.1.0+)
```
Dashboard → Plugins → Catalog → Search "File Transformation" → Install → Restart
```

#### 2. Custom Tabs (v0.2.2.0+)
```
Dashboard → Plugins → Catalog → Search "Custom Tabs" → Install → Restart
```

#### 3. AppwriteAuth (this plugin)
```
Dashboard → Plugins → Catalog → Search "AppwriteAuth" → Install → Restart
```

**Manual Installation**: If installing from DLL:
```bash
# Copy DLL to Jellyfin plugins directory
cp Jellyfin.Plugin.AppwriteAuth.dll /var/lib/jellyfin/plugins/

# Restart Jellyfin
sudo systemctl restart jellyfin
```

### ✅ Step 3: Configure Custom Tab

```
Dashboard → Plugins → Custom Tabs → Settings → Add New Tab

Tab Name: Auth Settings
Tab Icon: security
Tab Order: 100
HTML Content:
<div id="appwrite-auth-root" style="min-height: 100vh; width: 100%;"></div>
<script src="/Plugin/AppwriteAuth/js/appwrite-auth.js"></script>
```

Click **Save** and hard refresh your browser (Ctrl+F5).

### ✅ Step 4: Configure Appwrite

1. Click the new **"Auth Settings"** tab in navigation
2. Fill in Appwrite connection details:
   - **Endpoint**: `https://cloud.appwrite.io/v1` (or your self-hosted URL)
   - **Project ID**: Your Appwrite project ID
   - **API Key**: (Optional) For server-side operations
3. Configure email provider (SMTP or Appwrite Messaging)
4. Customize branding if desired
5. Click **Save Configuration**

### ✅ Step 5: Test Email Configuration

1. In Auth Settings → Admin Actions
2. Enter your email address
3. Click **Send Test Email**
4. Verify you receive the test email

---

## Dependency Chain

```
AppwriteAuth
    └── Custom Tabs (v0.2.2.0+)
        └── File Transformation (v2.2.1.0+)
```

**Important**: Jellyfin does NOT automatically install dependencies. You must install File Transformation and Custom Tabs manually before AppwriteAuth will work.

---

## Verification

After installation, verify everything is working:

1. **Check Plugins Are Active**:
   ```
   Dashboard → Plugins → My Plugins
   ```
   You should see all three plugins marked as "Active"

2. **Check JavaScript Bundle Loads**:
   ```
   Open browser console (F12)
   Navigate to: http://your-jellyfin:8096/Plugin/AppwriteAuth/js/appwrite-auth.js
   Should return JavaScript content (not 404)
   ```

3. **Check Auth Settings Tab**:
   - Navigation sidebar should have "Auth Settings" item
   - Clicking it should load the Vue SPA
   - Browser console should have no errors

4. **Check API Access**:
   ```javascript
   // In browser console:
   console.log(window.AppwriteAuthUI);
   // Should output: { mount: ƒ, unmount: ƒ }
   ```

---

## Troubleshooting

### "Auth Settings" tab doesn't appear

**Possible causes**:
- Custom Tabs not configured correctly
- Browser cache (try hard refresh: Ctrl+F5)
- Custom Tabs plugin not active

**Solutions**:
1. Go to **Dashboard → Plugins → Custom Tabs → Settings**
2. Verify the tab configuration is saved
3. Check that HTML content matches exactly:
   ```html
   <div id="appwrite-auth-root" style="min-height: 100vh; width: 100%;"></div>
   <script src="/Plugin/AppwriteAuth/js/appwrite-auth.js"></script>
   ```
4. Restart Jellyfin: `sudo systemctl restart jellyfin`
5. Clear browser cache and hard refresh

### AppwriteAuth plugin won't load

**Error**: "Plugin failed to load" or missing from plugin list

**Possible causes**:
- Missing dependencies (File Transformation or Custom Tabs)
- Incompatible Jellyfin version
- Corrupted DLL

**Solutions**:
1. Check Jellyfin logs: `/var/lib/jellyfin/log/log_*.txt`
2. Verify dependencies are installed:
   ```
   Dashboard → Plugins → My Plugins
   ```
   Should show: File Transformation + Custom Tabs
3. Ensure Jellyfin 10.11.0+ is installed
4. Re-download and reinstall the plugin DLL

### JavaScript bundle 404 error

**Error**: `/Plugin/AppwriteAuth/js/appwrite-auth.js` returns 404

**Possible causes**:
- Vue SPA not built before plugin compilation
- DLL doesn't include embedded resource
- Plugin not loaded properly

**Solutions**:
1. Rebuild plugin:
   ```bash
   cd ClientApp && pnpm run build
   cd .. && dotnet build --configuration Release
   ```
2. Verify embedded resource:
   ```bash
   # Check that wwwroot/appwrite-auth.js exists
   ls -lh Jellyfin.Plugin.AppwriteAuth/wwwroot/
   ```
3. Restart Jellyfin

### Vue SPA shows blank page

**Possible causes**:
- JavaScript errors (check browser console)
- Missing `#appwrite-auth-root` element
- API authentication issues

**Solutions**:
1. Open browser console (F12) and check for errors
2. Verify you're logged in as administrator
3. Check `window.ApiClient` exists:
   ```javascript
   console.log(window.ApiClient);
   ```
4. Try manual mount:
   ```javascript
   window.AppwriteAuthUI.mount('#appwrite-auth-root');
   ```

### Configuration not saving

**Error**: "Failed to save" toast or API errors

**Possible causes**:
- Not logged in as administrator
- Backend API errors
- Network issues

**Solutions**:
1. Verify administrator access
2. Check browser Network tab for failed requests
3. Check Jellyfin logs for API errors
4. Verify plugin GUID matches: `1f51419e-8bc3-4fb6-868e-6e8a094d9707`

---

## Uninstallation

To completely remove AppwriteAuth and its dependencies:

1. **Remove Custom Tab**:
   ```
   Dashboard → Plugins → Custom Tabs → Settings → Delete "Auth Settings" tab
   ```

2. **Uninstall Plugins** (in reverse order):
   ```
   Dashboard → Plugins → My Plugins
   - Uninstall AppwriteAuth
   - Uninstall Custom Tabs (if no other plugins use it)
   - Uninstall File Transformation (if no other plugins use it)
   ```

3. **Restart Jellyfin**

4. **Optional - Clean Data**:
   ```bash
   # Remove plugin configuration
   rm /var/lib/jellyfin/plugins/configurations/Jellyfin.Plugin.AppwriteAuth.xml

   # Remove plugin DLL
   rm /var/lib/jellyfin/plugins/Jellyfin.Plugin.AppwriteAuth.dll
   ```

---

## Support

- **Documentation**: See `/CUSTOM_TABS_SETUP.md` for detailed Custom Tabs configuration
- **Issues**: Report bugs at https://github.com/ZachHandley/JellyAppwriteAuth/issues
- **Logs**: Check `/var/lib/jellyfin/log/log_*.txt` for error details

---

## What's Installed

After successful installation:

### Plugins
- ✅ File Transformation (2.2.1.0+)
- ✅ Custom Tabs (0.2.2.0+)
- ✅ AppwriteAuth (1.0.0.0)

### UI Elements
- ✅ "Auth Settings" navigation tab
- ✅ Vue 3 admin interface (155 KB bundle)
- ✅ Toast notification system
- ✅ Dark mode support

### Features Available
- ✅ Appwrite authentication configuration
- ✅ Email provider setup (SMTP/Appwrite Messaging)
- ✅ Branding customization
- ✅ Email template editor
- ✅ Admin actions (invite, reset, test)
- ✅ User search and management

---

**Installation Complete!** 🎉

Navigate to **Auth Settings** in your Jellyfin sidebar to start configuring Appwrite authentication.

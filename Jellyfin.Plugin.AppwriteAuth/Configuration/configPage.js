const AppwriteAuthConfig = {
    pluginUniqueId: '1f51419e-8bc3-4fb6-868e-6e8a094d9707',

    loadConfiguration: function (page) {
        Dashboard.showLoadingMsg();

        ApiClient.getPluginConfiguration(this.pluginUniqueId)
            .then(function(config) {
                // Appwrite settings
                page.querySelector('#appwriteEndpoint').value = config.AppwriteEndpoint || '';
                page.querySelector('#appwriteProjectId').value = config.AppwriteProjectId || '';
                page.querySelector('#appwriteApiKey').value = config.AppwriteApiKey || '';
                page.querySelector('#markEmailVerified').checked = config.MarkEmailVerifiedOnLogin || false;

                // Email provider
                page.querySelector('#emailProvider').value = config.EmailProvider || 0;

                // SMTP settings
                page.querySelector('#smtpHost').value = config.SmtpHost || '';
                page.querySelector('#smtpPort').value = config.SmtpPort || 587;
                page.querySelector('#smtpUsername').value = config.SmtpUsername || '';
                page.querySelector('#smtpPassword').value = config.SmtpPassword || '';
                page.querySelector('#smtpFrom').value = config.SmtpFrom || '';
                page.querySelector('#smtpEnableSsl').checked = config.SmtpEnableSsl !== false;

                // Branding
                page.querySelector('#brandName').value = config.BrandName || 'Jellyfin';
                page.querySelector('#loginUrl').value = config.LoginUrl || '';

                // Show/hide SMTP settings based on provider
                AppwriteAuthConfig.updateSmtpVisibility(page, config.EmailProvider || 0);
            })
            .finally(function() {
                Dashboard.hideLoadingMsg();
            });
    },

    updateSmtpVisibility: function(page, provider) {
        const smtpSettings = page.querySelector('#smtpSettings');
        if (smtpSettings) {
            smtpSettings.style.display = (provider == 0) ? 'block' : 'none';
        }
    },

    save: function(page) {
        Dashboard.showLoadingMsg();

        return new Promise((resolve) => {
            ApiClient.getPluginConfiguration(this.pluginUniqueId)
                .then(function(config) {
                    // Appwrite settings
                    config.AppwriteEndpoint = page.querySelector('#appwriteEndpoint').value;
                    config.AppwriteProjectId = page.querySelector('#appwriteProjectId').value;
                    config.AppwriteApiKey = page.querySelector('#appwriteApiKey').value;
                    config.MarkEmailVerifiedOnLogin = page.querySelector('#markEmailVerified').checked;

                    // Email provider
                    config.EmailProvider = parseInt(page.querySelector('#emailProvider').value) || 0;

                    // SMTP settings
                    config.SmtpHost = page.querySelector('#smtpHost').value;
                    config.SmtpPort = parseInt(page.querySelector('#smtpPort').value) || 587;
                    config.SmtpUsername = page.querySelector('#smtpUsername').value;
                    config.SmtpPassword = page.querySelector('#smtpPassword').value;
                    config.SmtpFrom = page.querySelector('#smtpFrom').value;
                    config.SmtpEnableSsl = page.querySelector('#smtpEnableSsl').checked;

                    // Branding
                    config.BrandName = page.querySelector('#brandName').value;
                    config.LoginUrl = page.querySelector('#loginUrl').value;

                    ApiClient.updatePluginConfiguration(AppwriteAuthConfig.pluginUniqueId, config)
                        .then(Dashboard.processPluginConfigurationUpdateResult)
                        .then(resolve);
                })
                .catch(function(error) {
                    Dashboard.hideLoadingMsg();
                    Dashboard.alert('Error saving configuration: ' + error);
                });
        });
    }
};

export default function(view) {
    // Handle form submission
    view.querySelector('#appwriteAuthForm').addEventListener('submit', function(e) {
        e.preventDefault();
        AppwriteAuthConfig.save(view);
        return false;
    });

    // Handle email provider change
    view.querySelector('#emailProvider').addEventListener('change', function(e) {
        AppwriteAuthConfig.updateSmtpVisibility(view, parseInt(e.target.value));
    });

    // Load configuration when page shows
    window.addEventListener('pageshow', function() {
        AppwriteAuthConfig.loadConfiguration(view);
    });
}

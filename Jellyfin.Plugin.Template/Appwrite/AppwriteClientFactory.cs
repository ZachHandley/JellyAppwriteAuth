using System;
using Appwrite;
using Jellyfin.Plugin.Template.Configuration;

namespace Jellyfin.Plugin.Template.Appwrite;

public static class AppwriteClientFactory
{
    public static Client Create(PluginConfiguration config)
    {
        var endpoint = config.AppwriteEndpoint;
        var projectId = config.AppwriteProjectId;
        var apiKey = config.AppwriteApiKey ?? string.Empty;

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new InvalidOperationException("Appwrite endpoint is not configured. Set it in the plugin settings.");
        }

        if (string.IsNullOrWhiteSpace(projectId))
        {
            throw new InvalidOperationException("Appwrite project ID is not configured. Set it in the plugin settings.");
        }

        var client = new Client()
            .SetEndpoint(endpoint)
            .SetProject(projectId);

        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            client.SetKey(apiKey);
        }

        return client;
    }
}

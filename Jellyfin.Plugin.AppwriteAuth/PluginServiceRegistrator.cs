using Jellyfin.Plugin.AppwriteAuth.Authentication;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Authentication;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.AppwriteAuth;

/// <summary>
/// Registers plugin services with Jellyfin's dependency injection container.
/// </summary>
public class PluginServiceRegistrator : IPluginServiceRegistrator
{
    /// <inheritdoc />
    public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
    {
        // Register the Appwrite authentication provider
        serviceCollection.AddSingleton<IAuthenticationProvider, AppwriteAuthenticationProvider>();
    }
}

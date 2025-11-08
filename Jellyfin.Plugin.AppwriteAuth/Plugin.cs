using System;
using System.Collections.Generic;
using System.Globalization;
using Jellyfin.Plugin.AppwriteAuth.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.AppwriteAuth;

/// <summary>
/// The main plugin.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
    /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    /// <inheritdoc />
    public override string Name => "AppwriteAuth";

    /// <inheritdoc />
    public override Guid Id => Guid.Parse("1f51419e-8bc3-4fb6-868e-6e8a094d9707");

    /// <inheritdoc />
    public override string Description => "Appwrite-backed authentication with admin invite and reset tools";

    /// <summary>
    /// Gets the current plugin instance.
    /// </summary>
    public static Plugin? Instance { get; private set; }

    /// <inheritdoc />
    public IEnumerable<PluginPageInfo> GetPages()
    {
        var resourcePath = $"{GetType().Namespace}.Configuration.configPage.html";

        // Log to verify this method is being called
        Console.WriteLine($"[AppwriteAuth] GetPages() called");
        Console.WriteLine($"[AppwriteAuth] Resource path: {resourcePath}");
        Console.WriteLine($"[AppwriteAuth] Plugin name: {this.Name}");

        return new[]
        {
            new PluginPageInfo
            {
                Name = this.Name,
                DisplayName = "Appwrite Auth",
                EmbeddedResourcePath = resourcePath,
                EnableInMainMenu = true
            }
        };
    }
}

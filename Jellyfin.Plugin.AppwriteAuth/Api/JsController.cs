using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.AppwriteAuth.Api;

/// <summary>
/// Controller for serving the Vue SPA JavaScript bundle.
/// </summary>
[ApiController]
[Route("Plugin/AppwriteAuth/js")]
public class JsController : ControllerBase
{
    /// <summary>
    /// Serves the compiled Vue 3 SPA bundle as a single JavaScript file.
    /// </summary>
    /// <returns>The JavaScript bundle with application/javascript content type.</returns>
    [HttpGet("appwrite-auth.js")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client)]
    public IActionResult GetBundleJs()
    {
        var assembly = typeof(JsController).Assembly;
        var resourcePath = "Jellyfin.Plugin.AppwriteAuth.wwwroot.appwrite-auth.js";

        using var stream = assembly.GetManifestResourceStream(resourcePath);

        if (stream == null)
        {
            return NotFound(new { error = "JavaScript bundle not found. Ensure the Vue app has been built." });
        }

        // Read the stream into a byte array
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        var bytes = memoryStream.ToArray();

        return File(bytes, "application/javascript");
    }
}

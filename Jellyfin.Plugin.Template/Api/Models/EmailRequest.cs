using System.Diagnostics.CodeAnalysis;

namespace Jellyfin.Plugin.Template.Api.Models;

/// <summary>
/// Simple email payload model for admin endpoints.
/// </summary>
public sealed class EmailRequest
{
    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    [DisallowNull]
    public string? Email { get; set; }
}

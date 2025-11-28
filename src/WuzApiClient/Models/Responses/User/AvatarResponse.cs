using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.User;

/// <summary>
/// Response containing user avatar.
/// </summary>
public sealed class AvatarResponse
{
    /// <summary>
    /// Gets or sets the avatar URL.
    /// </summary>
    [JsonPropertyName("URL")]
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the avatar ID.
    /// </summary>
    [JsonPropertyName("ID")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the type (preview or full).
    /// </summary>
    [JsonPropertyName("Type")]
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the direct path to the avatar.
    /// </summary>
    [JsonPropertyName("DirectPath")]
    public string? DirectPath { get; set; }
}

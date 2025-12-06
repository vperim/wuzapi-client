using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;

namespace WuzApiClient.Models.Requests.Chat;

/// <summary>
/// Request to set presence state.
/// </summary>
public sealed class SetPresenceRequest
{
    /// <summary>
    /// Gets or sets the chat phone number or group ID.
    /// </summary>
    [JsonPropertyName("Phone")]
    public Phone Phone { get; set; }

    /// <summary>
    /// Gets or sets the presence state. Valid values: "composing", "paused", "recording", etc.
    /// </summary>
    [JsonPropertyName("State")]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the media type for the presence state.
    /// </summary>
    [JsonPropertyName("Media")]
    public string? Media { get; set; }
}

using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Requests.Group;

/// <summary>
/// Request to set group announcement-only mode.
/// </summary>
public sealed class SetGroupAnnounceRequest
{
    /// <summary>
    /// Gets or sets the group ID (JID).
    /// </summary>
    [JsonPropertyName("groupId")]
    public string GroupId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether announce mode is enabled.
    /// </summary>
    [JsonPropertyName("announce")]
    public bool Announce { get; set; }
}

using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Requests.Group;

/// <summary>
/// Request to set disappearing messages timer.
/// </summary>
public sealed class SetDisappearingMessagesRequest
{
    /// <summary>
    /// Gets or sets the group ID (JID).
    /// </summary>
    [JsonPropertyName("groupId")]
    public string GroupId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timer in seconds.
    /// Valid values: 0 (off), 86400 (24h), 604800 (7d), 7776000 (90d).
    /// </summary>
    [JsonPropertyName("timer")]
    public int Timer { get; set; }
}

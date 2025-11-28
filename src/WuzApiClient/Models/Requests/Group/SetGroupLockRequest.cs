using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Requests.Group;

/// <summary>
/// Request to set group lock status.
/// </summary>
public sealed class SetGroupLockRequest
{
    /// <summary>
    /// Gets or sets the group ID (JID).
    /// </summary>
    [JsonPropertyName("groupId")]
    public string GroupId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the group is locked.
    /// </summary>
    [JsonPropertyName("locked")]
    public bool Locked { get; set; }
}

using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Requests.Group;

/// <summary>
/// Request to update group name.
/// </summary>
public sealed class UpdateGroupNameRequest
{
    /// <summary>
    /// Gets or sets the group ID (JID).
    /// </summary>
    [JsonPropertyName("groupId")]
    public string GroupId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new group name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

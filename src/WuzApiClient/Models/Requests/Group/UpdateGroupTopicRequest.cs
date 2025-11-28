using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Requests.Group;

/// <summary>
/// Request to update group topic/description.
/// </summary>
public sealed class UpdateGroupTopicRequest
{
    /// <summary>
    /// Gets or sets the group ID (JID).
    /// </summary>
    [JsonPropertyName("groupId")]
    public string GroupId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new group topic.
    /// </summary>
    [JsonPropertyName("topic")]
    public string Topic { get; set; } = string.Empty;
}

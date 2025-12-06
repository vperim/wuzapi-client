using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

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
    [JsonConverter(typeof(JidConverter))]
    public Jid? GroupId { get; set; }

    /// <summary>
    /// Gets or sets the new group topic.
    /// </summary>
    [JsonPropertyName("topic")]
    public string Topic { get; set; } = string.Empty;
}

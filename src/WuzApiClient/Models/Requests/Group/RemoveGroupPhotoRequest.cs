using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.Models.Requests.Group;

/// <summary>
/// Request to remove group photo.
/// </summary>
public sealed class RemoveGroupPhotoRequest
{
    /// <summary>
    /// Gets or sets the group ID (JID).
    /// </summary>
    [JsonPropertyName("groupId")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? GroupId { get; set; }
}

using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

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
    [JsonConverter(typeof(JidConverter))]
    public Jid? GroupId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether announce mode is enabled.
    /// </summary>
    [JsonPropertyName("announce")]
    public bool Announce { get; set; }
}

using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

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
    [JsonConverter(typeof(JidConverter))]
    public Jid? GroupId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the group is locked.
    /// </summary>
    [JsonPropertyName("locked")]
    public bool Locked { get; set; }
}

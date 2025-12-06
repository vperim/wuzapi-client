using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Models.Requests.Group;

/// <summary>
/// Request to manage group participants.
/// </summary>
public sealed class ManageParticipantsRequest
{
    /// <summary>
    /// Gets or sets the group ID (JID).
    /// </summary>
    [JsonPropertyName("groupId")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? GroupId { get; set; }

    /// <summary>
    /// Gets or sets the participants to manage.
    /// </summary>
    [JsonPropertyName("participants")]
    [JsonConverter(typeof(JidArrayConverter))]
    public Jid[]? Participants { get; set; }

    /// <summary>
    /// Gets or sets the action to perform.
    /// </summary>
    [JsonPropertyName("action")]
    public ParticipantAction Action { get; set; }
}

using System.Text.Json.Serialization;
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
    public string GroupId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the participants to manage.
    /// </summary>
    [JsonPropertyName("participants")]
    public Phone[] Participants { get; set; } = [];

    /// <summary>
    /// Gets or sets the action to perform.
    /// </summary>
    [JsonPropertyName("action")]
    public ParticipantAction Action { get; set; }
}

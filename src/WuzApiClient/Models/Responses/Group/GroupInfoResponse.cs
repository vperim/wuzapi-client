using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.Group;

/// <summary>
/// Response containing detailed group information.
/// </summary>
public sealed class GroupInfoResponse
{
    /// <summary>
    /// Gets or sets the group JID.
    /// </summary>
    [JsonPropertyName("JID")]
    public string? Jid { get; set; }

    /// <summary>
    /// Gets or sets the group name.
    /// </summary>
    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the group topic.
    /// </summary>
    [JsonPropertyName("Topic")]
    public string? Topic { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp (ISO 8601 format).
    /// </summary>
    [JsonPropertyName("GroupCreated")]
    public string? GroupCreated { get; set; }

    /// <summary>
    /// Gets or sets the group owner JID.
    /// </summary>
    [JsonPropertyName("OwnerJID")]
    public string? OwnerJid { get; set; }

    /// <summary>
    /// Gets or sets the participants.
    /// </summary>
    [JsonPropertyName("Participants")]
    public GroupParticipant[]? Participants { get; set; }
}

/// <summary>
/// Group participant information.
/// </summary>
public sealed class GroupParticipant
{
    /// <summary>
    /// Gets or sets the participant JID.
    /// </summary>
    [JsonPropertyName("JID")]
    public string? Jid { get; set; }

    /// <summary>
    /// Gets or sets whether participant is admin.
    /// </summary>
    [JsonPropertyName("IsAdmin")]
    public bool IsAdmin { get; set; }

    /// <summary>
    /// Gets or sets whether participant is super admin.
    /// </summary>
    [JsonPropertyName("IsSuperAdmin")]
    public bool IsSuperAdmin { get; set; }
}

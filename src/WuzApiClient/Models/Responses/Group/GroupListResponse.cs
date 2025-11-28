using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.Group;

/// <summary>
/// Response containing list of groups.
/// </summary>
public sealed class GroupListResponse
{
    /// <summary>
    /// Gets or sets the groups.
    /// </summary>
    [JsonPropertyName("Groups")]
    public GroupSummary[]? Groups { get; set; }
}

/// <summary>
/// Summary information about a group.
/// </summary>
public sealed class GroupSummary
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
    /// Gets or sets the owner JID.
    /// </summary>
    [JsonPropertyName("OwnerJID")]
    public string? OwnerJid { get; set; }

    /// <summary>
    /// Gets or sets the participants.
    /// </summary>
    [JsonPropertyName("Participants")]
    public GroupParticipantSummary[]? Participants { get; set; }
}

/// <summary>
/// Participant summary in group list.
/// </summary>
public sealed class GroupParticipantSummary
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

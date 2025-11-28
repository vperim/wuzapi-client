using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.Group;

/// <summary>
/// Response containing invite link information.
/// </summary>
public sealed class GroupInviteInfoResponse
{
    /// <summary>
    /// Gets or sets the group JID.
    /// </summary>
    [JsonPropertyName("groupJid")]
    public string? GroupJid { get; set; }

    /// <summary>
    /// Gets or sets the group name.
    /// </summary>
    [JsonPropertyName("groupName")]
    public string? GroupName { get; set; }

    /// <summary>
    /// Gets or sets the number of participants.
    /// </summary>
    [JsonPropertyName("participants")]
    public int Participants { get; set; }

    /// <summary>
    /// Gets or sets the group creator JID.
    /// </summary>
    [JsonPropertyName("creator")]
    public string? Creator { get; set; }
}

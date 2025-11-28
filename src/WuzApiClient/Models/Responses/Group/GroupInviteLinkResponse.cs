using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.Group;

/// <summary>
/// Response containing group invite link.
/// </summary>
public sealed class GroupInviteLinkResponse
{
    /// <summary>
    /// Gets or sets the invite link.
    /// </summary>
    [JsonPropertyName("InviteLink")]
    public string Link { get; set; } = string.Empty;
}

using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Requests.Group;

/// <summary>
/// Request to get invite link information.
/// </summary>
public sealed class GetInviteInfoRequest
{
    /// <summary>
    /// Gets or sets the invite link.
    /// </summary>
    [JsonPropertyName("inviteLink")]
    public string InviteLink { get; set; } = string.Empty;
}

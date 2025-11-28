using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Requests.Group;

/// <summary>
/// Request to join a group via invite link.
/// </summary>
public sealed class JoinGroupRequest
{
    /// <summary>
    /// Gets or sets the invite code extracted from the invite link.
    /// Extract the code from the invite link URL. For example, extract 'ABC123' from 'https://chat.whatsapp.com/ABC123'.
    /// </summary>
    [JsonPropertyName("Code")]
    public string Code { get; set; } = string.Empty;
}

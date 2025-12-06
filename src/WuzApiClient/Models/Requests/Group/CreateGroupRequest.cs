using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.Models.Requests.Group;

/// <summary>
/// Request to create a new group.
/// </summary>
public sealed class CreateGroupRequest
{
    /// <summary>
    /// Gets or sets the group name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the initial participants.
    /// </summary>
    [JsonPropertyName("participants")]
    [JsonConverter(typeof(JidArrayConverter))]
    public Jid[]? Participants { get; set; }
}

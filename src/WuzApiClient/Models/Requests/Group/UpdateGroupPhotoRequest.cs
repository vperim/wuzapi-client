using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Requests.Group;

/// <summary>
/// Request to update group photo.
/// </summary>
public sealed class UpdateGroupPhotoRequest
{
    /// <summary>
    /// Gets or sets the group ID (JID).
    /// </summary>
    [JsonPropertyName("groupId")]
    public string GroupId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base64-encoded photo data.
    /// </summary>
    [JsonPropertyName("photo")]
    public string Photo { get; set; } = string.Empty;
}

using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;

namespace WuzApiClient.Models.Requests.User;

/// <summary>
/// Request body for fetching a user's profile picture (POST /user/avatar).
/// </summary>
public sealed class GetAvatarRequest
{
    /// <summary>
    /// Gets or sets the phone number to fetch the avatar for.
    /// </summary>
    [JsonPropertyName("Phone")]
    public Phone Phone { get; set; }

    /// <summary>
    /// Gets or sets whether to return the low-resolution preview (true) or the
    /// full-resolution picture (false).
    /// </summary>
    [JsonPropertyName("Preview")]
    public bool Preview { get; set; }
}

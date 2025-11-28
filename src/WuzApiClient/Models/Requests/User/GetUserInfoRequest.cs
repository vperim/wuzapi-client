using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Requests.User;

/// <summary>
/// Request to get user information.
/// Note: This endpoint requires phone numbers in JID format (e.g., "5511999999999@s.whatsapp.net").
/// </summary>
public sealed class GetUserInfoRequest
{
    /// <summary>
    /// Gets or sets the phone numbers in JID format to get info for.
    /// </summary>
    [JsonPropertyName("Phone")]
    public string[] Phones { get; set; } = [];
}

using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.Models.Requests.User;

/// <summary>
/// Request to get user information.
/// Note: This endpoint requires phone numbers in JID format (e.g., "5511999999999@s.whatsapp.net").
/// </summary>
public sealed class GetUserInfoRequest
{
    /// <summary>
    /// Gets or sets the JIDs to get info for.
    /// </summary>
    [JsonPropertyName("Phone")]
    [JsonConverter(typeof(JidArrayConverter))]
    public Jid[] Phones { get; set; } = [];
}

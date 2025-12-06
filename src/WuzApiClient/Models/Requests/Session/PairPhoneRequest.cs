using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;

namespace WuzApiClient.Models.Requests.Session;

/// <summary>
/// Request to pair via phone number.
/// </summary>
public sealed class PairPhoneRequest
{
    /// <summary>
    /// Gets or sets the phone number to pair with.
    /// </summary>
    [JsonPropertyName("phone")]
    public Phone Phone { get; set; }
}

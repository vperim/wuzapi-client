using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.Session;

/// <summary>
/// Response containing pairing code.
/// </summary>
public sealed class PairPhoneResponse
{
    /// <summary>
    /// Gets or sets the pairing code to enter on the phone.
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}

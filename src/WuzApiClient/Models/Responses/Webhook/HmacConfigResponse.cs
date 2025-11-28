using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.Webhook;

/// <summary>
/// Response containing HMAC configuration.
/// </summary>
public sealed class HmacConfigResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether HMAC is enabled.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the HMAC key (may be masked).
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }
}

using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Requests.Webhook;

/// <summary>
/// Request to set HMAC signing key.
/// </summary>
public sealed class SetHmacKeyRequest
{
    /// <summary>
    /// Gets or sets the HMAC key.
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;
}

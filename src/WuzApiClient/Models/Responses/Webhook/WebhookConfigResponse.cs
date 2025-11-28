using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.Webhook;

/// <summary>
/// Response containing webhook configuration.
/// </summary>
public sealed class WebhookConfigResponse
{
    /// <summary>
    /// Gets or sets the webhook URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the subscribed events.
    /// </summary>
    [JsonPropertyName("events")]
    public string[]? Events { get; set; }
}

using System.Text.Json.Serialization;
using WuzApiClient.Common.Enums;
using WuzApiClient.Json;

namespace WuzApiClient.Models.Requests.Webhook;

/// <summary>
/// Request to set webhook configuration.
/// </summary>
public sealed class SetWebhookRequest
{
    /// <summary>
    /// Gets or sets the webhook URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the events to subscribe to.
    /// </summary>
    [JsonPropertyName("events")]
    [JsonConverter(typeof(SubscribableEventArrayConverter))]
    public WhatsAppEventType[]? Events { get; set; }
}

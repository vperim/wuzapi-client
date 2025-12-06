using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when newsletter mute status is changed.
/// Maps to whatsmeow events.NewsletterMuteChange.
/// </summary>
public sealed record NewsletterMuteChangeEventEnvelope : WhatsAppEventEnvelope<NewsletterMuteChangeEventData>
{
    [JsonPropertyName("event")]
    public override required NewsletterMuteChangeEventData Event { get; init; }
}

/// <summary>
/// Event when newsletter mute status is changed.
/// Maps to whatsmeow events.NewsletterMuteChange.
/// </summary>
public sealed record NewsletterMuteChangeEventData
{
    /// <summary>
    /// Gets the newsletter JID.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? Id { get; init; }

    /// <summary>
    /// Gets the mute state ("on" or "off").
    /// </summary>
    [JsonPropertyName("mute")]
    public string? Mute { get; init; }
}

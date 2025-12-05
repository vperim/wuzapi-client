using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Data for the event when the client version is outdated.
/// Corresponds to whatsmeow events.ClientOutdated.
/// </summary>
public sealed record ClientOutdatedEventData;

/// <summary>
/// Envelope for the event when the client version is outdated.
/// Corresponds to whatsmeow events.ClientOutdated.
/// </summary>
public sealed record ClientOutdatedEventEnvelope : WhatsAppEventEnvelope<ClientOutdatedEventData>
{
    [JsonPropertyName("event")]
    public override required ClientOutdatedEventData Event { get; init; }
}

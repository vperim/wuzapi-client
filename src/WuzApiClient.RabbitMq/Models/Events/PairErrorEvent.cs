using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when device pairing fails.
/// Corresponds to whatsmeow events.PairError.
/// </summary>
public sealed record PairErrorEventData;

/// <summary>
/// Envelope for pair error event.
/// </summary>
public sealed record PairErrorEventEnvelope : WhatsAppEventEnvelope<PairErrorEventData>
{
    [JsonPropertyName("event")]
    public override required PairErrorEventData Event { get; init; }
}

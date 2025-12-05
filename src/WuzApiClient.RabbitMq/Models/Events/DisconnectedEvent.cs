using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Data for the event when the client disconnects from WhatsApp.
/// Corresponds to whatsmeow events.Disconnected.
/// </summary>
public sealed record DisconnectedEventData;

/// <summary>
/// Envelope for the event when the client disconnects from WhatsApp.
/// Corresponds to whatsmeow events.Disconnected.
/// </summary>
public sealed record DisconnectedEventEnvelope : WhatsAppEventEnvelope<DisconnectedEventData>
{
    [JsonPropertyName("event")]
    public override required DisconnectedEventData Event { get; init; }
}

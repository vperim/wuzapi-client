using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Data for the event when the client successfully connects to WhatsApp.
/// Corresponds to whatsmeow events.Connected.
/// </summary>
public sealed record ConnectedEventData;

/// <summary>
/// Envelope for the event when the client successfully connects to WhatsApp.
/// Corresponds to whatsmeow events.Connected.
/// </summary>
public sealed record ConnectedEventEnvelope : WhatsAppEventEnvelope<ConnectedEventData>
{
    [JsonPropertyName("event")]
    public override required ConnectedEventData Event { get; init; }
}

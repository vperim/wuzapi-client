using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when the QR code pairing times out.
/// Corresponds to whatsmeow events.QRTimeout.
/// </summary>
public sealed record QrTimeoutEventData;

/// <summary>
/// Envelope for QR timeout event.
/// </summary>
public sealed record QrTimeoutEventEnvelope : WhatsAppEventEnvelope<QrTimeoutEventData>
{
    [JsonPropertyName("event")]
    public override required QrTimeoutEventData Event { get; init; }
}

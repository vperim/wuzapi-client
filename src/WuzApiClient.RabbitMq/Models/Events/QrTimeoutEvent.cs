using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Envelope for QR timeout event.
/// The event field is a plain string ("timeout") from whatsmeow, not a structured object.
/// </summary>
public sealed record QrTimeoutEventEnvelope : WhatsAppEventEnvelope<string>
{
    [JsonPropertyName("event")]
    public override required string Event { get; init; }
}

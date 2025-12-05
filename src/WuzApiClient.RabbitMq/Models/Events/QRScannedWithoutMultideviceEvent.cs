using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when QR code is scanned but multidevice is not enabled on the phone.
/// User must enable multidevice in WhatsApp settings.
/// Corresponds to whatsmeow events.QRScannedWithoutMultidevice.
/// </summary>
public sealed record QrScannedWithoutMultideviceEventData;

/// <summary>
/// Envelope for QR scanned without multidevice event.
/// </summary>
public sealed record QrScannedWithoutMultideviceEventEnvelope : WhatsAppEventEnvelope<QrScannedWithoutMultideviceEventData>
{
    [JsonPropertyName("event")]
    public override required QrScannedWithoutMultideviceEventData Event { get; init; }
}

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when QR code is scanned but multidevice is not enabled on the phone.
/// User must enable multidevice in WhatsApp settings.
/// Corresponds to whatsmeow events.QRScannedWithoutMultidevice.
/// </summary>
public sealed record QrScannedWithoutMultideviceEvent;

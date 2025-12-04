namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when the QR code pairing times out.
/// Corresponds to whatsmeow events.QRTimeout.
/// </summary>
public sealed record QrTimeoutEvent;

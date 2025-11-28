namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when the QR code pairing times out.
/// </summary>
public sealed record QrTimeoutEvent : WuzEvent;

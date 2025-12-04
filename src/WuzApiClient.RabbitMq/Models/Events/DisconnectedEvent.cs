namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when the client disconnects from WhatsApp.
/// Corresponds to whatsmeow events.Disconnected.
/// </summary>
public sealed record DisconnectedEvent;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when the client disconnects from WhatsApp.
/// </summary>
public sealed record DisconnectedEvent : WuzEvent;

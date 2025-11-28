namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when the client successfully connects to WhatsApp.
/// </summary>
public sealed record ConnectedEvent : WuzEvent;

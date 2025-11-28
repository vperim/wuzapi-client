namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when connection to WhatsApp fails.
/// </summary>
public sealed record ConnectFailureEvent : WuzEvent;

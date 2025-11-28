namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when the stream is replaced by a new connection.
/// </summary>
public sealed record StreamReplacedEvent : WuzEvent;

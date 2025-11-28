namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event for keep-alive timeout.
/// </summary>
public sealed record KeepAliveTimeoutEvent : WuzEvent
{
}

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event for keep-alive connection restored.
/// </summary>
public sealed record KeepAliveRestoredEvent : WuzEvent
{
}

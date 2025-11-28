namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event for offline synchronization completion.
/// </summary>
public sealed record OfflineSyncCompletedEvent : WuzEvent
{
}

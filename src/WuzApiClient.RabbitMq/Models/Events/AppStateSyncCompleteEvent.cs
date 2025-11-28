namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event for application state synchronization completion.
/// </summary>
public sealed record AppStateSyncCompleteEvent : WuzEvent
{
}

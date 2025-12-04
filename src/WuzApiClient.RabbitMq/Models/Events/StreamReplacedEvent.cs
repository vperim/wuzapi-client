namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event emitted when the client is disconnected by another client connecting with the same keys.
/// Corresponds to whatsmeow events.StreamReplaced.
/// This event has no additional properties.
/// </summary>
public sealed record StreamReplacedEvent
{
}

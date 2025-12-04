namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event emitted if the keepalive pings start working again after some KeepAliveTimeout events.
/// Corresponds to whatsmeow events.KeepAliveRestored.
/// This event has no additional properties.
/// </summary>
public sealed record KeepAliveRestoredEvent
{
}

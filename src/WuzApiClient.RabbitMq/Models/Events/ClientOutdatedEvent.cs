namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when the client version is outdated.
/// Corresponds to whatsmeow events.ClientOutdated.
/// </summary>
public sealed record ClientOutdatedEvent;

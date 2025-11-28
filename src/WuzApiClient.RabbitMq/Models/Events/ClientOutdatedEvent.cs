namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when the client version is outdated.
/// </summary>
public sealed record ClientOutdatedEvent : WuzEvent;

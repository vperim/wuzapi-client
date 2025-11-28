namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when blocklist is modified.
/// </summary>
public sealed record BlocklistChangeEvent : WuzEvent;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when the account receives a temporary ban.
/// </summary>
public sealed record TemporaryBanEvent : WuzEvent;

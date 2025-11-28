namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when device pairing fails.
/// </summary>
public sealed record PairErrorEvent : WuzEvent;

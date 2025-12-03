namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when a Client Access Token (CAT) refresh operation fails.
/// </summary>
public sealed record CATRefreshErrorEvent : WuzEvent;

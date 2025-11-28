namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event for messages that could not be decrypted.
/// </summary>
public sealed record UndecryptableMessageEvent : WuzEvent
{
}

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when the client successfully connects to WhatsApp.
/// Corresponds to whatsmeow events.Connected.
/// </summary>
public sealed record ConnectedEvent;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when newsletter mute status is changed.
/// </summary>
public sealed record NewsletterMuteChangeEvent : WuzEvent;

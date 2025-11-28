namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when a newsletter receives a live update.
/// </summary>
public sealed record NewsletterLiveUpdateEvent : WuzEvent;

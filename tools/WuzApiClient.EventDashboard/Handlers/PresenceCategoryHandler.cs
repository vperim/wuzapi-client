using WuzApiClient.EventDashboard.Models;
using WuzApiClient.EventDashboard.Models.Metadata;
using WuzApiClient.EventDashboard.Services;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models;
using WuzApiClient.RabbitMq.Models.Events;

namespace WuzApiClient.EventDashboard.Handlers;

/// <summary>
/// Handles presence events (Presence, ChatPresence).
/// </summary>
public sealed class PresenceCategoryHandler :
    IEventHandler<PresenceEvent>,
    IEventHandler<ChatPresenceEvent>
{
    private readonly IEventStreamService eventStream;

    public PresenceCategoryHandler(IEventStreamService eventStream)
    {
        this.eventStream = eventStream;
    }

    public Task HandleAsync(WuzEventEnvelope<PresenceEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new PresenceMetadata
        {
            Category = EventCategory.Presence,
            UserJid = evt.From ?? string.Empty,
            State = evt.State ?? "unknown",
            IsAvailable = !evt.Unavailable,
            LastSeen = evt.LastSeen,
            Media = null,
            IsChatPresence = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<ChatPresenceEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new PresenceMetadata
        {
            Category = EventCategory.Presence,
            UserJid = evt.Sender ?? string.Empty,
            State = evt.State ?? "unknown",
            IsAvailable = true,
            LastSeen = null,
            Media = evt.Media,
            IsChatPresence = true
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }
}

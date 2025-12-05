using WuzApiClient.EventDashboard.Models;
using WuzApiClient.EventDashboard.Models.Metadata;
using WuzApiClient.EventDashboard.Services;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models.Events;
using WuzApiClient.RabbitMq.Models.Wuz;

namespace WuzApiClient.EventDashboard.Handlers;

/// <summary>
/// Handles presence events (Presence, ChatPresence).
/// </summary>
public sealed class PresenceCategoryHandler :
    IEventHandler<PresenceEventEnvelope>,
    IEventHandler<ChatPresenceEventEnvelope>
{
    private readonly IEventStreamService eventStream;

    public PresenceCategoryHandler(IEventStreamService eventStream)
    {
        this.eventStream = eventStream;
    }

    public Task HandleAsync(IWuzEventEnvelope<PresenceEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
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

    public Task HandleAsync(IWuzEventEnvelope<ChatPresenceEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
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

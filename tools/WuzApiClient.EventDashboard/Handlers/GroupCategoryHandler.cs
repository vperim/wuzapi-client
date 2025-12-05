using WuzApiClient.EventDashboard.Models;
using WuzApiClient.EventDashboard.Models.Metadata;
using WuzApiClient.EventDashboard.Services;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models.Events;
using WuzApiClient.RabbitMq.Models.Wuz;

namespace WuzApiClient.EventDashboard.Handlers;

/// <summary>
/// Handles group events (GroupInfo, JoinedGroup, Picture).
/// </summary>
public sealed class GroupCategoryHandler :
    IEventHandler<GroupInfoEventEnvelope>,
    IEventHandler<JoinedGroupEventEnvelope>,
    IEventHandler<PictureEventEnvelope>
{
    private readonly IEventStreamService eventStream;

    public GroupCategoryHandler(IEventStreamService eventStream)
    {
        this.eventStream = eventStream;
    }

    public Task HandleAsync(IWuzEventEnvelope<GroupInfoEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
        var metadata = new GroupMetadata
        {
            Category = EventCategory.Group,
            GroupJid = evt.Jid ?? string.Empty,
            GroupName = evt.Name,
            Topic = evt.Topic,
            IsLocked = evt.Locked,
            IsAnnounce = evt.Announce,
            IsPictureRemoved = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(IWuzEventEnvelope<JoinedGroupEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
        var metadata = new GroupMetadata
        {
            Category = EventCategory.Group,
            GroupJid = evt.Jid ?? string.Empty,
            JoinReason = evt.Reason,
            CreateTime = evt.CreateTime,
            IsPictureRemoved = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(IWuzEventEnvelope<PictureEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
        var metadata = new GroupMetadata
        {
            Category = EventCategory.Group,
            GroupJid = evt.Jid ?? string.Empty,
            PictureAuthor = evt.Author,
            IsPictureRemoved = evt.Remove
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }
}

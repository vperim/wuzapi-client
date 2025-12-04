using WuzApiClient.EventDashboard.Models;
using WuzApiClient.EventDashboard.Models.Metadata;
using WuzApiClient.EventDashboard.Services;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models;
using WuzApiClient.RabbitMq.Models.Events;

namespace WuzApiClient.EventDashboard.Handlers;

/// <summary>
/// Handles group events (GroupInfo, JoinedGroup, Picture).
/// </summary>
public sealed class GroupCategoryHandler :
    IEventHandler<GroupInfoEvent>,
    IEventHandler<JoinedGroupEvent>,
    IEventHandler<PictureEvent>
{
    private readonly IEventStreamService eventStream;

    public GroupCategoryHandler(IEventStreamService eventStream)
    {
        this.eventStream = eventStream;
    }

    public Task HandleAsync(WuzEventEnvelope<GroupInfoEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
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

    public Task HandleAsync(WuzEventEnvelope<JoinedGroupEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
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

    public Task HandleAsync(WuzEventEnvelope<PictureEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
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

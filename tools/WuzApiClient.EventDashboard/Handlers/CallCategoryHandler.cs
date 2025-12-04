using WuzApiClient.EventDashboard.Models;
using WuzApiClient.EventDashboard.Models.Metadata;
using WuzApiClient.EventDashboard.Services;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models;
using WuzApiClient.RabbitMq.Models.Events;

namespace WuzApiClient.EventDashboard.Handlers;

/// <summary>
/// Handles call events (CallOffer, CallAccept, CallTerminate, etc.).
/// </summary>
public sealed class CallCategoryHandler :
    IEventHandler<CallOfferEvent>,
    IEventHandler<CallAcceptEvent>,
    IEventHandler<CallTerminateEvent>,
    IEventHandler<CallOfferNoticeEvent>,
    IEventHandler<CallRelayLatencyEvent>
{
    private readonly IEventStreamService eventStream;

    public CallCategoryHandler(IEventStreamService eventStream)
    {
        this.eventStream = eventStream;
    }

    public Task HandleAsync(WuzEventEnvelope<CallOfferEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new CallMetadata
        {
            Category = EventCategory.Call,
            CallEvent = "CallOffer",
            CallId = evt.CallId,
            FromJid = evt.From,
            RemotePlatform = evt.RemotePlatform,
            RemoteVersion = evt.RemoteVersion
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<CallAcceptEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new CallMetadata
        {
            Category = EventCategory.Call,
            CallEvent = "CallAccept",
            CallId = evt.CallId,
            FromJid = evt.From,
            RemotePlatform = evt.RemotePlatform,
            RemoteVersion = evt.RemoteVersion
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<CallTerminateEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new CallMetadata
        {
            Category = EventCategory.Call,
            CallEvent = "CallTerminate",
            CallId = evt.CallId,
            FromJid = evt.From,
            TerminateReason = evt.Reason
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<CallOfferNoticeEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new CallMetadata
        {
            Category = EventCategory.Call,
            CallEvent = "CallOfferNotice",
            CallId = evt.CallId,
            FromJid = evt.From,
            MediaType = evt.Media,
            CallType = evt.Type
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<CallRelayLatencyEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new CallMetadata
        {
            Category = EventCategory.Call,
            CallEvent = "CallRelayLatency",
            CallId = evt.CallId,
            FromJid = evt.From
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }
}

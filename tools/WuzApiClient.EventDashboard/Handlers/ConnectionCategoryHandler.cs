using WuzApiClient.EventDashboard.Models;
using WuzApiClient.EventDashboard.Models.Metadata;
using WuzApiClient.EventDashboard.Services;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models;
using WuzApiClient.RabbitMq.Models.Events;

namespace WuzApiClient.EventDashboard.Handlers;

/// <summary>
/// Handles connection lifecycle events (Connected, Disconnected, QR codes, pairing, etc.).
/// </summary>
public sealed class ConnectionCategoryHandler :
    IEventHandler<ConnectedEvent>,
    IEventHandler<DisconnectedEvent>,
    IEventHandler<LoggedOutEvent>,
    IEventHandler<ConnectFailureEvent>,
    IEventHandler<QrCodeEvent>,
    IEventHandler<QrTimeoutEvent>,
    IEventHandler<QrScannedWithoutMultideviceEvent>,
    IEventHandler<PairSuccessEvent>,
    IEventHandler<PairErrorEvent>,
    IEventHandler<ClientOutdatedEvent>,
    IEventHandler<TemporaryBanEvent>,
    IEventHandler<StreamErrorEvent>,
    IEventHandler<StreamReplacedEvent>
{
    private readonly IEventStreamService eventStream;

    public ConnectionCategoryHandler(IEventStreamService eventStream)
    {
        this.eventStream = eventStream;
    }

    public Task HandleAsync(WuzEventEnvelope<ConnectedEvent> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "Connected"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<DisconnectedEvent> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "Disconnected"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<LoggedOutEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "LoggedOut",
            Reason = evt.Reason
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<ConnectFailureEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "ConnectFailure",
            Reason = evt.Reason,
            ErrorMessage = evt.Error,
            RetryAttempts = evt.Attempts
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<QrCodeEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "QR",
            QrCodeBase64 = evt.QrCodeBase64
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<QrTimeoutEvent> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "QRTimeout"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<QrScannedWithoutMultideviceEvent> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "QRScannedWithoutMultidevice"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<PairSuccessEvent> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "PairSuccess"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<PairErrorEvent> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "PairError"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<ClientOutdatedEvent> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "ClientOutdated"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<TemporaryBanEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "TemporaryBan",
            ErrorMessage = $"Code: {evt.Code}, Expires: {evt.Expire}"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<StreamErrorEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "StreamError",
            ErrorMessage = $"Code: {evt.Code}"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<StreamReplacedEvent> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "StreamReplaced"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }
}

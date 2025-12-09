using WuzApiClient.EventDashboard.Models;
using WuzApiClient.EventDashboard.Models.Metadata;
using WuzApiClient.EventDashboard.Services;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models.Events;
using WuzApiClient.RabbitMq.Models.Wuz;

namespace WuzApiClient.EventDashboard.Handlers;

/// <summary>
/// Handles connection lifecycle events (Connected, Disconnected, QR codes, pairing, etc.).
/// </summary>
public sealed class ConnectionCategoryHandler :
    IEventHandler<ConnectedEventEnvelope>,
    IEventHandler<DisconnectedEventEnvelope>,
    IEventHandler<LoggedOutEventEnvelope>,
    IEventHandler<ConnectFailureEventEnvelope>,
    IEventHandler<QrCodeEventEnvelope>,
    IEventHandler<QrTimeoutEventEnvelope>,
    IEventHandler<QrScannedWithoutMultideviceEventEnvelope>,
    IEventHandler<PairSuccessEventEnvelope>,
    IEventHandler<PairErrorEventEnvelope>,
    IEventHandler<ClientOutdatedEventEnvelope>,
    IEventHandler<TemporaryBanEventEnvelope>,
    IEventHandler<StreamErrorEventEnvelope>,
    IEventHandler<StreamReplacedEventEnvelope>
{
    private readonly IEventStreamService eventStream;

    public ConnectionCategoryHandler(IEventStreamService eventStream)
    {
        this.eventStream = eventStream;
    }

    public Task HandleAsync(IWuzEventEnvelope<ConnectedEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "Connected"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(IWuzEventEnvelope<DisconnectedEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "Disconnected"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(IWuzEventEnvelope<LoggedOutEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "LoggedOut",
            Reason = evt.Reason.ToString()
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(IWuzEventEnvelope<ConnectFailureEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
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

    public Task HandleAsync(IWuzEventEnvelope<QrCodeEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload;
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "QR",
            QrCodeBase64 = evt.QrCode?.ToString()
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(IWuzEventEnvelope<QrTimeoutEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "QRTimeout"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(IWuzEventEnvelope<QrScannedWithoutMultideviceEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "QRScannedWithoutMultidevice"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(IWuzEventEnvelope<PairSuccessEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "PairSuccess"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(IWuzEventEnvelope<PairErrorEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "PairError"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(IWuzEventEnvelope<ClientOutdatedEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "ClientOutdated"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(IWuzEventEnvelope<TemporaryBanEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "TemporaryBan",
            ErrorMessage = $"Code: {evt.Code}, Expires: {evt.Expire}"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(IWuzEventEnvelope<StreamErrorEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
        var metadata = new ConnectionMetadata
        {
            Category = EventCategory.Connection,
            ConnectionEvent = "StreamError",
            ErrorMessage = $"Code: {evt.Code}"
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(IWuzEventEnvelope<StreamReplacedEventEnvelope> envelope, CancellationToken cancellationToken = default)
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

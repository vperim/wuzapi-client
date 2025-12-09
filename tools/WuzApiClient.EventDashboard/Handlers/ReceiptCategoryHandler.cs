using WuzApiClient.EventDashboard.Models;
using WuzApiClient.EventDashboard.Models.Metadata;
using WuzApiClient.EventDashboard.Services;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models.Events;
using WuzApiClient.RabbitMq.Models.Wuz;

namespace WuzApiClient.EventDashboard.Handlers;

/// <summary>
/// Handles receipt events (read, delivery, played receipts).
/// </summary>
public sealed class ReceiptCategoryHandler : IEventHandler<ReceiptEventEnvelope>
{
    private readonly IEventStreamService eventStream;

    public ReceiptCategoryHandler(IEventStreamService eventStream)
    {
        this.eventStream = eventStream;
    }

    public Task HandleAsync(IWuzEventEnvelope<ReceiptEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
        var metadata = new ReceiptMetadata
        {
            Category = EventCategory.Receipt,
            ChatJid = evt.Chat ?? string.Empty,
            SenderJid = evt.Sender ?? string.Empty,
            MessageIds = evt.MessageIDs ?? [],
            ReceiptTimestamp = evt.Timestamp,
            ReceiptType = evt.ReceiptType.ToString(),
            State = envelope.Payload.State.ToString(),
            IsGroup = evt.IsGroup
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }
}

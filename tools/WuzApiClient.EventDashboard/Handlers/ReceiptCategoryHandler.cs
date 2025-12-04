using WuzApiClient.EventDashboard.Models;
using WuzApiClient.EventDashboard.Models.Metadata;
using WuzApiClient.EventDashboard.Services;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models;
using WuzApiClient.RabbitMq.Models.Events;

namespace WuzApiClient.EventDashboard.Handlers;

/// <summary>
/// Handles receipt events (read, delivery, played receipts).
/// </summary>
public sealed class ReceiptCategoryHandler : IEventHandler<ReceiptEvent>
{
    private readonly IEventStreamService eventStream;

    public ReceiptCategoryHandler(IEventStreamService eventStream)
    {
        this.eventStream = eventStream;
    }

    public Task HandleAsync(WuzEventEnvelope<ReceiptEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new ReceiptMetadata
        {
            Category = EventCategory.Receipt,
            ChatJid = evt.Chat ?? string.Empty,
            SenderJid = evt.Sender ?? string.Empty,
            MessageIds = evt.MessageIDs ?? [],
            ReceiptTimestamp = evt.Timestamp,
            ReceiptType = evt.ReceiptType ?? "unknown",
            State = evt.State ?? "unknown",
            IsGroup = evt.IsGroup
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }
}

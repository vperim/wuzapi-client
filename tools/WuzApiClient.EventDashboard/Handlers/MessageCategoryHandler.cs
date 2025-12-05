using WuzApiClient.EventDashboard.Models;
using WuzApiClient.EventDashboard.Models.Metadata;
using WuzApiClient.EventDashboard.Services;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models.Events;
using WuzApiClient.RabbitMq.Models.Wuz;

namespace WuzApiClient.EventDashboard.Handlers;

/// <summary>
/// Handles message-related events: Message, UndecryptableMessage, FbMessage.
/// </summary>
public sealed class MessageCategoryHandler :
    IEventHandler<MessageEventEnvelope>,
    IEventHandler<UndecryptableMessageEventEnvelope>,
    IEventHandler<FbMessageEventEnvelope>
{
    private readonly IEventStreamService eventStream;

    public MessageCategoryHandler(IEventStreamService eventStream)
    {
        this.eventStream = eventStream;
    }

    public Task HandleAsync(IWuzEventEnvelope<MessageEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
        var metadata = new MessageMetadata
        {
            Category = EventCategory.Message,
            MessageId = evt.Info?.Id ?? string.Empty,
            ChatJid = evt.Info?.Chat ?? string.Empty,
            SenderJid = evt.Info?.Sender ?? string.Empty,
            PushName = evt.Info?.PushName,
            IsFromMe = evt.Info?.IsFromMe ?? false,
            IsGroup = evt.Info?.IsGroup ?? false,
            MessageTimestamp = evt.Info?.Timestamp,
            MessageType = evt.Info?.Type ?? "unknown",
            MediaType = envelope.Payload.MimeType,
            ContentPreview = ExtractContentPreview(evt),
            IsViewOnce = evt.IsViewOnce,
            IsEphemeral = evt.IsEphemeral
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(IWuzEventEnvelope<UndecryptableMessageEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
        var metadata = new MessageMetadata
        {
            Category = EventCategory.Message,
            MessageId = evt.Info?.Id ?? string.Empty,
            ChatJid = evt.Info?.Chat ?? string.Empty,
            SenderJid = evt.Info?.Sender ?? string.Empty,
            PushName = evt.Info?.PushName,
            IsFromMe = evt.Info?.IsFromMe ?? false,
            IsGroup = evt.Info?.IsGroup ?? false,
            MessageTimestamp = evt.Timestamp,
            MessageType = "undecryptable",
            MediaType = null,
            ContentPreview = "[Undecryptable Message]",
            IsViewOnce = false,
            IsEphemeral = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(IWuzEventEnvelope<FbMessageEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
        var metadata = new MessageMetadata
        {
            Category = EventCategory.Message,
            MessageId = evt.Info?.Id ?? string.Empty,
            ChatJid = evt.Info?.Chat ?? string.Empty,
            SenderJid = evt.Info?.Sender ?? string.Empty,
            PushName = evt.Info?.PushName,
            IsFromMe = evt.Info?.IsFromMe ?? false,
            IsGroup = evt.Info?.IsGroup ?? false,
            MessageTimestamp = evt.Info?.Timestamp,
            MessageType = "fbmessage",
            MediaType = null,
            ContentPreview = ExtractFbContentPreview(evt),
            IsViewOnce = evt.IsViewOnce,
            IsEphemeral = evt.IsEphemeral
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    private static string ExtractContentPreview(MessageEventData evt)
    {
        // Text message - conversation field
        if (evt.Message?.Conversation != null && !string.IsNullOrWhiteSpace(evt.Message.Conversation))
            return evt.Message.Conversation;

        // Text message - extendedTextMessage
        if (evt.Message?.ExtendedTextMessage?.Text != null && !string.IsNullOrWhiteSpace(evt.Message.ExtendedTextMessage.Text))
            return evt.Message.ExtendedTextMessage.Text;

        // Media types
        if (evt.Message?.ImageMessage != null)
            return "📷 Image";

        if (evt.Message?.AudioMessage != null)
        {
            var isPtt = evt.Message.AudioMessage.Ptt;
            return isPtt ? "🎤 Voice note" : "🎵 Audio";
        }

        if (evt.Message?.VideoMessage != null)
            return "🎬 Video";

        if (evt.Message?.DocumentMessage != null)
            return "📄 Document";

        if (evt.Message?.StickerMessage != null)
            return "🎭 Sticker";

        return "[Message]";
    }

    private static string ExtractFbContentPreview(FbMessageEventData evt)
    {
        // Text message - conversation field
        if (evt.Message?.Conversation != null && !string.IsNullOrWhiteSpace(evt.Message.Conversation))
            return evt.Message.Conversation;

        // Text message - extendedTextMessage
        if (evt.Message?.ExtendedTextMessage?.Text != null && !string.IsNullOrWhiteSpace(evt.Message.ExtendedTextMessage.Text))
            return evt.Message.ExtendedTextMessage.Text;

        return "[FB Message]";
    }
}

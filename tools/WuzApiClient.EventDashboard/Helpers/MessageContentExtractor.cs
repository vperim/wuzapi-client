using System.Text.Json;
using WuzApiClient.EventDashboard.Models;
using WuzApiClient.RabbitMq.Models;

namespace WuzApiClient.EventDashboard.Helpers;

public static class MessageContentExtractor
{
    public static MessagePreviewResult CreatePreview(WuzEvent evt)
    {
        // WuzEvent has: Type, UserId, InstanceName, RawEvent (JsonElement?)
        // All message details must be extracted from RawEvent
        return evt.Type switch
        {
            "Message" => ExtractMessage(evt),
            _ => new MessagePreviewResult($"[{evt.Type}]", "unknown-message")
        };
    }

    private static MessagePreviewResult ExtractMessage(WuzEvent evt)
    {
        // Handle nullable RawEvent
        if (evt.RawEvent is not JsonElement raw)
            return new MessagePreviewResult("[No content]", "unknown-message");

        // WuzAPI uses PascalCase: "Message" not "message"
        if (!raw.TryGetProperty("Message", out var message))
            return new MessagePreviewResult("[Message]", "unknown-message");

        // Text: conversation field
        if (message.TryGetProperty("conversation", out var conv) &&
            conv.ValueKind == JsonValueKind.String)
        {
            var text = conv.GetString();
            return new MessagePreviewResult(
                string.IsNullOrWhiteSpace(text) ? "[Empty message]" : $"\"{text}\"",
                "text-message");
        }

        // Text: extendedTextMessage.text
        if (message.TryGetProperty("extendedTextMessage", out var ext) &&
            ext.TryGetProperty("text", out var extText) &&
            extText.ValueKind == JsonValueKind.String)
        {
            var text = extText.GetString();
            return new MessagePreviewResult(
                string.IsNullOrWhiteSpace(text) ? "[Empty message]" : $"\"{text}\"",
                "text-message");
        }

        // Reaction
        if (message.TryGetProperty("reactionMessage", out var reaction) &&
            reaction.TryGetProperty("text", out var emoji) &&
            emoji.ValueKind == JsonValueKind.String)
        {
            return new MessagePreviewResult(
                $"{emoji.GetString()} reacted to message",
                "reaction-message");
        }

        // Media types (v1: icon only, no metadata)
        if (message.TryGetProperty("imageMessage", out _))
            return new MessagePreviewResult("📷 Image", "media-message");

        if (message.TryGetProperty("audioMessage", out var audio))
        {
            var isPtt = audio.TryGetProperty("ptt", out var ptt) &&
                        ptt.ValueKind == JsonValueKind.True;
            return new MessagePreviewResult(
                isPtt ? "🎤 Voice note" : "🎵 Audio",
                "media-message");
        }

        if (message.TryGetProperty("videoMessage", out _))
            return new MessagePreviewResult("🎬 Video", "media-message");

        if (message.TryGetProperty("documentMessage", out _))
            return new MessagePreviewResult("📄 Document", "media-message");

        if (message.TryGetProperty("stickerMessage", out _))
            return new MessagePreviewResult("🎭 Sticker", "media-message");

        if (message.TryGetProperty("locationMessage", out _))
            return new MessagePreviewResult("📍 Location", "media-message");

        return new MessagePreviewResult("[Message]", "unknown-message");
    }

    /// <summary>
    /// Extracts sender info from RawEvent for direction/sender display.
    /// WuzAPI structure: Info.IsFromMe, Info.PushName, Info.Sender
    /// </summary>
    public static (bool IsFromMe, string? PushName, string? SenderJid) ExtractSenderInfo(WuzEvent evt)
    {
        if (evt.RawEvent is not JsonElement raw)
            return (false, null, null);

        bool isFromMe = false;
        string? pushName = null;
        string? senderJid = null;

        // WuzAPI uses PascalCase "Info" with PascalCase properties
        if (raw.TryGetProperty("Info", out var info))
        {
            if (info.TryGetProperty("IsFromMe", out var fromMe) &&
                fromMe.ValueKind == JsonValueKind.True)
                isFromMe = true;

            if (info.TryGetProperty("Sender", out var sender) &&
                sender.ValueKind == JsonValueKind.String)
                senderJid = sender.GetString();

            if (info.TryGetProperty("PushName", out var pn) &&
                pn.ValueKind == JsonValueKind.String)
                pushName = pn.GetString();
        }

        return (isFromMe, pushName, senderJid);
    }
}

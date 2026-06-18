using System;
using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.Chat;

/// <summary>
/// A single persisted message from the gateway's message history
/// (GET /chat/history). Field names are snake_case on the wire, unlike the
/// rest of the API, so they are mapped explicitly.
/// </summary>
public sealed class HistoryMessageResponse
{
    /// <summary>Gateway row identifier.</summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>Owning wuzapi user id.</summary>
    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }

    /// <summary>Chat JID this message belongs to.</summary>
    [JsonPropertyName("chat_jid")]
    public string? ChatJid { get; set; }

    /// <summary>Sender JID.</summary>
    [JsonPropertyName("sender_jid")]
    public string? SenderJid { get; set; }

    /// <summary>WhatsApp message id.</summary>
    [JsonPropertyName("message_id")]
    public string? MessageId { get; set; }

    /// <summary>When the message was sent/received.</summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>Message type (e.g. "text", "image", "document").</summary>
    [JsonPropertyName("message_type")]
    public string? MessageType { get; set; }

    /// <summary>Text content (empty for non-text messages).</summary>
    [JsonPropertyName("text_content")]
    public string? TextContent { get; set; }

    /// <summary>Link to downloadable media, when present.</summary>
    [JsonPropertyName("media_link")]
    public string? MediaLink { get; set; }

    /// <summary>Quoted message id, when this is a reply.</summary>
    [JsonPropertyName("quoted_message_id")]
    public string? QuotedMessageId { get; set; }

    /// <summary>Raw gateway payload (JSON) for fields not modeled here.</summary>
    [JsonPropertyName("data_json")]
    public string? DataJson { get; set; }
}

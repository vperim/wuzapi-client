using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.Session;

/// <summary>
/// Acknowledgement of a history-sync request (GET /session/history). The gateway
/// asks WhatsApp to backfill history; the actual messages arrive asynchronously and
/// are later readable via GET /chat/history.
/// </summary>
public sealed class RequestHistorySyncResponse
{
    /// <summary>Human-readable status (e.g. "History sync request Sent").</summary>
    [JsonPropertyName("details")]
    public string? Details { get; set; }

    /// <summary>Server timestamp of the request.</summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    /// <summary>Number of messages requested.</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>Chat JID the sync targeted, when scoped.</summary>
    [JsonPropertyName("chat_jid")]
    public string? ChatJid { get; set; }

    /// <summary>Oldest message id used as the sync anchor, when paginating.</summary>
    [JsonPropertyName("oldest_msg_id")]
    public string? OldestMsgId { get; set; }

    /// <summary>Whether the oldest anchor message was sent by this account.</summary>
    [JsonPropertyName("oldest_msg_from_me")]
    public bool OldestMsgFromMe { get; set; }

    /// <summary>Oldest anchor message timestamp (Unix ms), when paginating.</summary>
    [JsonPropertyName("oldest_msg_timestamp")]
    public long OldestMsgTimestamp { get; set; }
}

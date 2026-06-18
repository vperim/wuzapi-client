using System;
using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.Chat;

/// <summary>
/// One chat entry returned by the history index (GET /chat/history?chat_jid=index),
/// which maps each user id to the list of chats it has persisted history for.
/// </summary>
public sealed class ChatHistoryIndexEntry
{
    /// <summary>Chat JID with persisted history.</summary>
    [JsonPropertyName("chat_jid")]
    public string? ChatJid { get; set; }

    /// <summary>Timestamp of the most recent persisted message for this chat.</summary>
    [JsonPropertyName("last_updated")]
    public DateTime LastUpdated { get; set; }
}

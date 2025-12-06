using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.Models.Common;

/// <summary>
/// Context information for message quoting and replies.
/// </summary>
public sealed class ContextInfo
{
    /// <summary>
    /// Gets or sets the stanza ID of the quoted message.
    /// </summary>
    [JsonPropertyName("StanzaId")]
    public string? StanzaId { get; set; }

    /// <summary>
    /// Gets or sets the participant JID (for group messages).
    /// </summary>
    [JsonPropertyName("Participant")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? Participant { get; set; }

    /// <summary>
    /// Gets or sets the quoted message content.
    /// </summary>
    [JsonPropertyName("QuotedMessage")]
    public object? QuotedMessage { get; set; }

    /// <summary>
    /// Gets or sets the list of mentioned JIDs.
    /// </summary>
    [JsonPropertyName("MentionedJid")]
    [JsonConverter(typeof(JidArrayConverter))]
    public Jid[]? MentionedJid { get; set; }

    /// <summary>
    /// Gets or sets whether this message is forwarded.
    /// </summary>
    [JsonPropertyName("IsForwarded")]
    public bool? IsForwarded { get; set; }

    /// <summary>
    /// Gets or sets the forwarding score.
    /// </summary>
    [JsonPropertyName("ForwardingScore")]
    public int? ForwardingScore { get; set; }
}

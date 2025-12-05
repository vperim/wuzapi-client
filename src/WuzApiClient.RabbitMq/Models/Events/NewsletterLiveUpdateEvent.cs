using System;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when a newsletter receives a live update.
/// Maps to whatsmeow events.NewsletterLiveUpdate.
/// </summary>
public sealed record NewsletterLiveUpdateEventEnvelope : WhatsAppEventEnvelope<NewsletterLiveUpdateEventData>
{
    [JsonPropertyName("event")]
    public override required NewsletterLiveUpdateEventData Event { get; init; }
}

/// <summary>
/// Event when a newsletter receives a live update.
/// Maps to whatsmeow events.NewsletterLiveUpdate.
/// </summary>
public sealed record NewsletterLiveUpdateEventData
{
    /// <summary>
    /// Gets the newsletter JID.
    /// </summary>
    [JsonPropertyName("JID")]
    public string? Jid { get; init; }

    /// <summary>
    /// Gets the timestamp of the live update.
    /// </summary>
    [JsonPropertyName("Time")]
    public DateTimeOffset? Time { get; init; }

    /// <summary>
    /// Gets the list of messages in the live update.
    /// </summary>
    [JsonPropertyName("Messages")]
    public NewsletterMessage[]? Messages { get; init; }
}

/// <summary>
/// Represents a newsletter message in a live update.
/// </summary>
public sealed record NewsletterMessage
{
    /// <summary>
    /// Gets the message server ID.
    /// </summary>
    [JsonPropertyName("MessageServerID")]
    public int? MessageServerId { get; init; }

    /// <summary>
    /// Gets the message ID.
    /// </summary>
    [JsonPropertyName("MessageID")]
    public string? MessageId { get; init; }

    /// <summary>
    /// Gets the message type.
    /// </summary>
    [JsonPropertyName("Type")]
    public string? Type { get; init; }

    /// <summary>
    /// Gets the message timestamp.
    /// </summary>
    [JsonPropertyName("Timestamp")]
    public DateTimeOffset? Timestamp { get; init; }

    /// <summary>
    /// Gets the number of views.
    /// </summary>
    [JsonPropertyName("ViewsCount")]
    public int? ViewsCount { get; init; }

    /// <summary>
    /// Gets the reaction counts by emoji.
    /// </summary>
    [JsonPropertyName("ReactionCounts")]
    public System.Collections.Generic.Dictionary<string, int>? ReactionCounts { get; init; }
}

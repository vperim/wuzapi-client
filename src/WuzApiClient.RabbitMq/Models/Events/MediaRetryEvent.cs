using System;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event for media retry requests.
/// Maps to whatsmeow events.MediaRetry.
/// </summary>
public sealed record MediaRetryEvent
{
    /// <summary>
    /// Gets the chat JID where the media was sent.
    /// </summary>
    [JsonPropertyName("ChatID")]
    public string? ChatId { get; init; }

    /// <summary>
    /// Gets the sender JID of the media message.
    /// </summary>
    [JsonPropertyName("SenderID")]
    public string? SenderId { get; init; }

    /// <summary>
    /// Gets the message ID being retried.
    /// </summary>
    [JsonPropertyName("MessageID")]
    public string? MessageId { get; init; }

    /// <summary>
    /// Gets the timestamp of the retry request.
    /// </summary>
    [JsonPropertyName("Timestamp")]
    public DateTimeOffset? Timestamp { get; init; }

    /// <summary>
    /// Gets the error code for the media failure.
    /// </summary>
    [JsonPropertyName("Error")]
    public int? Error { get; init; }
}

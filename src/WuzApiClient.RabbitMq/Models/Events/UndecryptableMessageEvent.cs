using System;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event for messages that could not be decrypted.
/// Maps to whatsmeow events.UndecryptableMessage.
/// </summary>
public sealed record UndecryptableMessageEvent
{
    /// <summary>
    /// Gets the message information (metadata).
    /// </summary>
    [JsonPropertyName("Info")]
    public MessageInfo? Info { get; init; }

    /// <summary>
    /// Gets whether this is an undecryptable message.
    /// </summary>
    [JsonPropertyName("IsUnavailable")]
    public bool IsUnavailable { get; init; }

    /// <summary>
    /// Gets the message type.
    /// </summary>
    [JsonPropertyName("Type")]
    public string? Type { get; init; }

    /// <summary>
    /// Gets the timestamp.
    /// </summary>
    [JsonPropertyName("Timestamp")]
    public DateTimeOffset? Timestamp { get; init; }
}

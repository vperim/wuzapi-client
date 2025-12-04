using System;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event emitted for call offer notice on WhatsApp.
/// Corresponds to whatsmeow events.CallOfferNotice.
/// </summary>
public sealed record CallOfferNoticeEvent
{
    // BasicCallMeta fields
    /// <summary>
    /// Gets the JID of the sender.
    /// </summary>
    [JsonPropertyName("From")]
    public string? From { get; init; }

    /// <summary>
    /// Gets the timestamp of the event.
    /// </summary>
    [JsonPropertyName("Timestamp")]
    public DateTimeOffset Timestamp { get; init; }

    /// <summary>
    /// Gets the JID of the call creator.
    /// </summary>
    [JsonPropertyName("CallCreator")]
    public string? CallCreator { get; init; }

    /// <summary>
    /// Gets the alternative JID of the call creator.
    /// </summary>
    [JsonPropertyName("CallCreatorAlt")]
    public string? CallCreatorAlt { get; init; }

    /// <summary>
    /// Gets the call identifier.
    /// </summary>
    [JsonPropertyName("CallID")]
    public string? CallId { get; init; }

    /// <summary>
    /// Gets the group JID if this is a group call.
    /// </summary>
    [JsonPropertyName("GroupJID")]
    public string? GroupJid { get; init; }

    // CallOfferNotice-specific fields
    /// <summary>
    /// Gets the media type of the call.
    /// </summary>
    [JsonPropertyName("Media")]
    public string? Media { get; init; }

    /// <summary>
    /// Gets the type of the call offer notice.
    /// </summary>
    [JsonPropertyName("Type")]
    public string? Type { get; init; }
}

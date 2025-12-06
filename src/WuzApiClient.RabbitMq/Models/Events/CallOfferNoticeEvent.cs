using System;
using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event emitted for call offer notice on WhatsApp.
/// Corresponds to whatsmeow events.CallOfferNotice.
/// </summary>
public sealed record CallOfferNoticeEventEnvelope : WhatsAppEventEnvelope<CallOfferNoticeEventData>
{
    [JsonPropertyName("event")]
    public override required CallOfferNoticeEventData Event { get; init; }
}

/// <summary>
/// Data for a call offer notice event on WhatsApp.
/// </summary>
public sealed record CallOfferNoticeEventData
{
    // BasicCallMeta fields
    /// <summary>
    /// Gets the JID of the sender.
    /// </summary>
    [JsonPropertyName("From")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? From { get; init; }

    /// <summary>
    /// Gets the timestamp of the event.
    /// </summary>
    [JsonPropertyName("Timestamp")]
    public DateTimeOffset Timestamp { get; init; }

    /// <summary>
    /// Gets the JID of the call creator.
    /// </summary>
    [JsonPropertyName("CallCreator")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? CallCreator { get; init; }

    /// <summary>
    /// Gets the alternative JID of the call creator.
    /// </summary>
    [JsonPropertyName("CallCreatorAlt")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? CallCreatorAlt { get; init; }

    /// <summary>
    /// Gets the call identifier.
    /// </summary>
    [JsonPropertyName("CallID")]
    public string? CallId { get; init; }

    /// <summary>
    /// Gets the group JID if this is a group call.
    /// </summary>
    [JsonPropertyName("GroupJID")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? GroupJid { get; init; }

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

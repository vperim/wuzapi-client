using System;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event emitted for call relay latency updates on WhatsApp.
/// Corresponds to whatsmeow events.CallRelayLatency.
/// </summary>
public sealed record CallRelayLatencyEventEnvelope : WhatsAppEventEnvelope<CallRelayLatencyEventData>
{
    [JsonPropertyName("event")]
    public override required CallRelayLatencyEventData Event { get; init; }
}

/// <summary>
/// Data for a call relay latency event on WhatsApp.
/// </summary>
public sealed record CallRelayLatencyEventData
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
}

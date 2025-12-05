using System;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event emitted when a call is terminated on WhatsApp.
/// Corresponds to whatsmeow events.CallTerminate.
/// </summary>
public sealed record CallTerminateEventEnvelope : WhatsAppEventEnvelope<CallTerminateEventData>
{
    [JsonPropertyName("event")]
    public override required CallTerminateEventData Event { get; init; }
}

/// <summary>
/// Data for a call terminate event on WhatsApp.
/// </summary>
public sealed record CallTerminateEventData
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

    // CallTerminate-specific fields
    /// <summary>
    /// Gets the reason for call termination.
    /// </summary>
    [JsonPropertyName("Reason")]
    public string? Reason { get; init; }
}

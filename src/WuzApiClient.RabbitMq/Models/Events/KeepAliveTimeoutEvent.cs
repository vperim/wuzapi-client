using System;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event emitted when the keepalive ping request to WhatsApp web servers times out.
/// Corresponds to whatsmeow events.KeepAliveTimeout.
/// </summary>
public sealed record KeepAliveTimeoutEventEnvelope : WhatsAppEventEnvelope<KeepAliveTimeoutEventData>
{
    [JsonPropertyName("event")]
    public override required KeepAliveTimeoutEventData Event { get; init; }
}

/// <summary>
/// Event data emitted when the keepalive ping request to WhatsApp web servers times out.
/// Corresponds to whatsmeow events.KeepAliveTimeout.
/// </summary>
public sealed record KeepAliveTimeoutEventData
{
    /// <summary>
    /// Gets the number of consecutive errors.
    /// </summary>
    [JsonPropertyName("ErrorCount")]
    public int ErrorCount { get; init; }

    /// <summary>
    /// Gets the timestamp of the last successful keepalive.
    /// </summary>
    [JsonPropertyName("LastSuccess")]
    public DateTimeOffset LastSuccess { get; init; }
}

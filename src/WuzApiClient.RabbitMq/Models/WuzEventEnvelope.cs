using System;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models;

/// <summary>
/// Non-generic base for polymorphic collections and routing.
/// Transport metadata for wuzapi events.
/// </summary>
public abstract record WuzEventEnvelope
{
    /// <summary>
    /// Gets the event type identifier from wuzapi.
    /// </summary>
    [JsonPropertyName("type")]
    public string EventType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the user ID that generated this event.
    /// </summary>
    [JsonPropertyName("userID")]
    public string UserId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the instance name.
    /// </summary>
    [JsonPropertyName("instanceName")]
    public string InstanceName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the timestamp when this event was received by the client.
    /// </summary>
    [JsonIgnore]
    public DateTimeOffset ReceivedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets the typed event object. Implemented by generic subclass.
    /// </summary>
    [JsonIgnore]
    public abstract object EventObject { get; }

    /// <summary>
    /// Gets the raw JSON string of the entire event envelope.
    /// Stored as string to avoid JsonDocument disposal issues.
    /// </summary>
    [JsonIgnore]
    public string RawJson { get; init; } = string.Empty;
}

/// <summary>
/// Generic envelope for typed access to event data.
/// </summary>
/// <typeparam name="TEvent">The event payload type.</typeparam>
public sealed record WuzEventEnvelope<TEvent> : WuzEventEnvelope
    where TEvent : class
{
    /// <summary>
    /// Gets the non-nullable typed event data.
    /// If envelope exists, event was successfully parsed.
    /// </summary>
    [JsonPropertyName("event")]
    public required TEvent Event { get; init; }

    /// <inheritdoc/>
    [JsonIgnore]
    public override object EventObject => Event;
}

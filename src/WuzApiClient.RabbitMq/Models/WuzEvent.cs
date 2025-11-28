using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models;

/// <summary>
/// Base record for all WhatsApp events from wuzapi.
/// </summary>
public abstract record WuzEvent
{
    /// <summary>
    /// Gets the event type identifier.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

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
    /// Gets the raw event payload for dynamic access.
    /// </summary>
    [JsonPropertyName("event")]
    public JsonElement? RawEvent { get; init; }

    /// <summary>
    /// Gets the timestamp when this event was received.
    /// </summary>
    [JsonIgnore]
    public DateTimeOffset ReceivedAt { get; init; } = DateTimeOffset.UtcNow;
}

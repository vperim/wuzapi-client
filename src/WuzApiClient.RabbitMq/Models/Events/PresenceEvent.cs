using System;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event for user presence (online/offline) updates.
/// Maps to whatsmeow events.Presence with wuzapi additions.
/// </summary>
public sealed record PresenceEvent
{
    // === whatsmeow Presence fields ===

    /// <summary>
    /// Gets the JID of the user whose presence changed.
    /// </summary>
    [JsonPropertyName("From")]
    public string? From { get; init; }

    /// <summary>
    /// Gets whether the user is unavailable (offline).
    /// </summary>
    [JsonPropertyName("Unavailable")]
    public bool Unavailable { get; init; }

    /// <summary>
    /// Gets the last seen timestamp.
    /// </summary>
    [JsonPropertyName("LastSeen")]
    public DateTimeOffset? LastSeen { get; init; }

    // === wuzapi-added fields ===

    /// <summary>
    /// Gets the presence state (added by wuzapi: "online" or "offline").
    /// This field is at the root level in wuzapi JSON.
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; init; }
}

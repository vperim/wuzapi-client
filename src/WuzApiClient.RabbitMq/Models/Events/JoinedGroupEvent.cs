using System;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event for joining a group.
/// Maps to whatsmeow events.JoinedGroup.
/// </summary>
public sealed record JoinedGroupEventEnvelope : WhatsAppEventEnvelope<JoinedGroupEventData>
{
    [JsonPropertyName("event")]
    public override required JoinedGroupEventData Event { get; init; }
}

/// <summary>
/// Data for a joined group event.
/// </summary>
public sealed record JoinedGroupEventData
{
    /// <summary>
    /// Gets the group JID that was joined.
    /// </summary>
    [JsonPropertyName("JID")]
    public string? Jid { get; init; }

    /// <summary>
    /// Gets the reason for joining (e.g., created, added, invited).
    /// </summary>
    [JsonPropertyName("Reason")]
    public string? Reason { get; init; }

    /// <summary>
    /// Gets the timestamp when the group was joined.
    /// </summary>
    [JsonPropertyName("CreateTime")]
    public DateTimeOffset? CreateTime { get; init; }
}

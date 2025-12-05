using System;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// User about/status update event from whatsmeow events.UserAbout.
/// Emitted when a user's about status is changed.
/// </summary>
public sealed record UserAboutEventEnvelope : WhatsAppEventEnvelope<UserAboutEventData>
{
    [JsonPropertyName("event")]
    public override required UserAboutEventData Event { get; init; }
}

/// <summary>
/// User about/status update event data from whatsmeow events.UserAbout.
/// Emitted when a user's about status is changed.
/// </summary>
public sealed record UserAboutEventData
{
    /// <summary>
    /// Gets the JID of the user whose about status changed.
    /// </summary>
    [JsonPropertyName("JID")]
    public string? Jid { get; init; }

    /// <summary>
    /// Gets the new about/status text.
    /// </summary>
    [JsonPropertyName("Status")]
    public string? Status { get; init; }

    /// <summary>
    /// Gets the timestamp of the status change.
    /// </summary>
    [JsonPropertyName("Timestamp")]
    public DateTimeOffset? Timestamp { get; init; }
}

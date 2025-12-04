using System;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Push name setting change event from whatsmeow events.PushNameSetting.
/// Emitted when the user's push name is changed from another device.
/// </summary>
public sealed record PushNameSettingEvent
{
    /// <summary>
    /// Gets the timestamp of the push name change.
    /// </summary>
    [JsonPropertyName("Timestamp")]
    public DateTimeOffset? Timestamp { get; init; }

    /// <summary>
    /// Gets the push name setting action data.
    /// </summary>
    [JsonPropertyName("Action")]
    public object? Action { get; init; }

    /// <summary>
    /// Gets whether this event came from a full sync.
    /// </summary>
    [JsonPropertyName("FromFullSync")]
    public bool FromFullSync { get; init; }
}

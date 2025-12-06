using System;
using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Identity change event from whatsmeow events.IdentityChange.
/// Emitted when another user changes their primary device.
/// </summary>
public sealed record IdentityChangeEventEnvelope : WhatsAppEventEnvelope<IdentityChangeEventData>
{
    [JsonPropertyName("event")]
    public override required IdentityChangeEventData Event { get; init; }
}

/// <summary>
/// Data for an identity change event from whatsmeow events.IdentityChange.
/// Emitted when another user changes their primary device.
/// </summary>
public sealed record IdentityChangeEventData
{
    /// <summary>
    /// Gets the JID of the user whose identity changed.
    /// </summary>
    [JsonPropertyName("JID")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? Jid { get; init; }

    /// <summary>
    /// Gets the timestamp of the identity change.
    /// </summary>
    [JsonPropertyName("Timestamp")]
    public DateTimeOffset? Timestamp { get; init; }

    /// <summary>
    /// Gets whether this change was implicit (triggered by an untrusted identity error).
    /// If true, the event was triggered by an untrusted identity error rather than an identity change notification from the server.
    /// </summary>
    [JsonPropertyName("Implicit")]
    public bool Implicit { get; init; }
}

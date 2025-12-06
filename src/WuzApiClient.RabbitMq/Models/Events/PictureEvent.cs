using System;
using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event for profile/group picture updates.
/// Maps to whatsmeow events.Picture.
/// </summary>
public sealed record PictureEventEnvelope : WhatsAppEventEnvelope<PictureEventData>
{
    [JsonPropertyName("event")]
    public override required PictureEventData Event { get; init; }
}

/// <summary>
/// Event data for profile/group picture updates.
/// Maps to whatsmeow events.Picture.
/// </summary>
public sealed record PictureEventData
{
    /// <summary>
    /// Gets the JID of the user or group whose picture changed.
    /// </summary>
    [JsonPropertyName("JID")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? Jid { get; init; }

    /// <summary>
    /// Gets the author of the picture change (for groups).
    /// </summary>
    [JsonPropertyName("Author")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? Author { get; init; }

    /// <summary>
    /// Gets the timestamp of the picture change.
    /// </summary>
    [JsonPropertyName("Timestamp")]
    public DateTimeOffset? Timestamp { get; init; }

    /// <summary>
    /// Gets whether the picture was removed.
    /// </summary>
    [JsonPropertyName("Remove")]
    public bool Remove { get; init; }

    /// <summary>
    /// Gets the picture ID.
    /// </summary>
    [JsonPropertyName("PictureID")]
    public string? PictureId { get; init; }
}

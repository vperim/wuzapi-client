using System;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event for profile/group picture updates.
/// Maps to whatsmeow events.Picture.
/// </summary>
public sealed record PictureEvent
{
    /// <summary>
    /// Gets the JID of the user or group whose picture changed.
    /// </summary>
    [JsonPropertyName("JID")]
    public string? Jid { get; init; }

    /// <summary>
    /// Gets the author of the picture change (for groups).
    /// </summary>
    [JsonPropertyName("Author")]
    public string? Author { get; init; }

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

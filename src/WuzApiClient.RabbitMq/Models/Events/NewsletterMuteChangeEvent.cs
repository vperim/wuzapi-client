using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when newsletter mute status is changed.
/// Maps to whatsmeow events.NewsletterMuteChange.
/// </summary>
public sealed record NewsletterMuteChangeEvent
{
    /// <summary>
    /// Gets the newsletter JID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Gets the mute state ("on" or "off").
    /// </summary>
    [JsonPropertyName("mute")]
    public string? Mute { get; init; }
}

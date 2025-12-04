using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when a newsletter is left.
/// Maps to whatsmeow events.NewsletterLeave.
/// </summary>
public sealed record NewsletterLeaveEvent
{
    /// <summary>
    /// Gets the newsletter JID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Gets the viewer's role in the newsletter ("subscriber", "guest", "admin", "owner").
    /// </summary>
    [JsonPropertyName("role")]
    public string? Role { get; init; }
}

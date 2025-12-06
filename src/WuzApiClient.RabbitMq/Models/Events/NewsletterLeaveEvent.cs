using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when a newsletter is left.
/// Maps to whatsmeow events.NewsletterLeave.
/// </summary>
public sealed record NewsletterLeaveEventEnvelope : WhatsAppEventEnvelope<NewsletterLeaveEventData>
{
    [JsonPropertyName("event")]
    public override required NewsletterLeaveEventData Event { get; init; }
}

/// <summary>
/// Event when a newsletter is left.
/// Maps to whatsmeow events.NewsletterLeave.
/// </summary>
public sealed record NewsletterLeaveEventData
{
    /// <summary>
    /// Gets the newsletter JID.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? Id { get; init; }

    /// <summary>
    /// Gets the viewer's role in the newsletter ("subscriber", "guest", "admin", "owner").
    /// </summary>
    [JsonPropertyName("role")]
    public string? Role { get; init; }
}

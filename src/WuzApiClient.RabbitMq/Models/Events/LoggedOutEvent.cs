using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Data for the event when the client is logged out from WhatsApp.
/// Corresponds to whatsmeow events.LoggedOut.
/// </summary>
public sealed record LoggedOutEventData
{
    /// <summary>
    /// Gets the reason for logout.
    /// From whatsmeow Reason field (converted to string).
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; init; }
}

/// <summary>
/// Envelope for the event when the client is logged out from WhatsApp.
/// Corresponds to whatsmeow events.LoggedOut.
/// </summary>
public sealed record LoggedOutEventEnvelope : WhatsAppEventEnvelope<LoggedOutEventData>
{
    [JsonPropertyName("event")]
    public override required LoggedOutEventData Event { get; init; }
}

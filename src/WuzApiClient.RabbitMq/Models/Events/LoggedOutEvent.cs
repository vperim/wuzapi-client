using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when the client is logged out from WhatsApp.
/// Corresponds to whatsmeow events.LoggedOut.
/// </summary>
public sealed record LoggedOutEvent
{
    /// <summary>
    /// Gets the reason for logout.
    /// From whatsmeow Reason field (converted to string).
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; init; }
}

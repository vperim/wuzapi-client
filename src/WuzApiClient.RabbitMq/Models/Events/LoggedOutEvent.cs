using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when the client is logged out from WhatsApp.
/// </summary>
public sealed record LoggedOutEvent : WuzEvent
{
    /// <summary>
    /// Gets the reason for logout.
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; init; }
}

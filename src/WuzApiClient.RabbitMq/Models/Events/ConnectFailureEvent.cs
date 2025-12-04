using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when connection to WhatsApp fails.
/// Corresponds to whatsmeow events.ConnectFailure with wuzapi enhancements.
/// </summary>
public sealed record ConnectFailureEvent
{
    /// <summary>
    /// Gets the error message describing the connection failure.
    /// This is a wuzapi-added field.
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; init; }

    /// <summary>
    /// Gets the number of connection attempts made before failure.
    /// This is a wuzapi-added field.
    /// </summary>
    [JsonPropertyName("attempts")]
    public int? Attempts { get; init; }

    /// <summary>
    /// Gets the reason for the connection failure.
    /// This is a wuzapi-added field.
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; init; }
}

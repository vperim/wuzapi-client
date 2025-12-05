using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Data for the event when connection to WhatsApp fails.
/// Corresponds to whatsmeow events.ConnectFailure with wuzapi enhancements.
/// </summary>
public sealed record ConnectFailureEventData
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

/// <summary>
/// Envelope for the event when connection to WhatsApp fails.
/// Corresponds to whatsmeow events.ConnectFailure with wuzapi enhancements.
/// </summary>
public sealed record ConnectFailureEventEnvelope : WhatsAppEventEnvelope<ConnectFailureEventData>
{
    [JsonPropertyName("event")]
    public override required ConnectFailureEventData Event { get; init; }
}

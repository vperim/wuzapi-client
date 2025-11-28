using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event for call offer received.
/// </summary>
public sealed record CallOfferEvent : WuzEvent
{
    /// <summary>
    /// Gets the call identifier.
    /// </summary>
    [JsonPropertyName("callId")]
    public string? CallId { get; init; }

    /// <summary>
    /// Gets the caller identifier.
    /// </summary>
    [JsonPropertyName("from")]
    public string? From { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is a video call.
    /// </summary>
    [JsonPropertyName("isVideo")]
    public bool IsVideo { get; init; }
}

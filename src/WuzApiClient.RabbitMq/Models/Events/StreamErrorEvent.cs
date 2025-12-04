using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event emitted when the WhatsApp server sends a stream error node with an unknown code.
/// Corresponds to whatsmeow events.StreamError.
/// </summary>
public sealed record StreamErrorEvent
{
    /// <summary>
    /// Gets the error code from the stream error.
    /// </summary>
    [JsonPropertyName("Code")]
    public string? Code { get; init; }
}

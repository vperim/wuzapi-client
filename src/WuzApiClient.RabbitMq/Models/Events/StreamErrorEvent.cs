using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Stream error event envelope emitted when the WhatsApp server sends a stream error node with an unknown code.
/// Corresponds to whatsmeow events.StreamError.
/// </summary>
public sealed record StreamErrorEventEnvelope : WhatsAppEventEnvelope<StreamErrorEventData>
{
    [JsonPropertyName("event")]
    public override required StreamErrorEventData Event { get; init; }
}

/// <summary>
/// Stream error event data emitted when the WhatsApp server sends a stream error node with an unknown code.
/// Contains the stream error information.
/// </summary>
public sealed record StreamErrorEventData
{
    /// <summary>
    /// Gets the error code from the stream error.
    /// </summary>
    [JsonPropertyName("Code")]
    public string? Code { get; init; }
}

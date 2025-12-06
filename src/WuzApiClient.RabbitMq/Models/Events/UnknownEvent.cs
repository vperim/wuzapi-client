using System.Text.Json;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Fallback event envelope for unknown or failed event types.
/// Provides forward compatibility when wuzapi adds new event types.
/// </summary>
public sealed record UnknownEventEnvelope : WhatsAppEventEnvelope<UnknownEvent>
{
    public override required UnknownEvent Event { get; init; }
}

/// <summary>
/// Fallback event data for unknown or failed event types.
/// Provides forward compatibility when wuzapi adds new event types.
/// </summary>
public sealed record UnknownEvent
{
    /// <summary>
    /// Gets the raw JSON of the entire event payload.
    /// </summary>
    [JsonPropertyName("raw")]
    public JsonElement Raw { get; init; }

    /// <summary>
    /// Gets the original type string from wuzapi (for debugging).
    /// </summary>
    [JsonPropertyName("originalType")]
    public string OriginalType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the error message if deserialization failed.
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; init; }
}

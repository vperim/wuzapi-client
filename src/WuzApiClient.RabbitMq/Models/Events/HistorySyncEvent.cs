using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// History sync event envelope from whatsmeow events.HistorySync.
/// Emitted when the phone has sent a blob of historical messages.
/// </summary>
public sealed record HistorySyncEventEnvelope : WhatsAppEventEnvelope<HistorySyncEventData>
{
    /// <summary>
    /// Gets the history sync event data.
    /// </summary>
    [JsonPropertyName("event")]
    public override required HistorySyncEventData Event { get; init; }
}

/// <summary>
/// History sync event data from whatsmeow events.HistorySync.
/// Contains conversations and messages information.
/// </summary>
public sealed record HistorySyncEventData
{
    /// <summary>
    /// Gets the history sync data containing conversations and messages.
    /// </summary>
    [JsonPropertyName("Data")]
    public object? Data { get; init; }
}

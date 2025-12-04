using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// History sync event from whatsmeow events.HistorySync.
/// Emitted when the phone has sent a blob of historical messages.
/// </summary>
public sealed record HistorySyncEvent
{
    /// <summary>
    /// Gets the history sync data containing conversations and messages.
    /// </summary>
    [JsonPropertyName("Data")]
    public object? Data { get; init; }
}

using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Offline sync completion event from whatsmeow events.OfflineSyncCompleted.
/// Emitted after the server has finished sending missed events.
/// </summary>
public sealed record OfflineSyncCompletedEvent
{
    /// <summary>
    /// Gets the count of events that were synced.
    /// </summary>
    [JsonPropertyName("Count")]
    public int Count { get; init; }
}

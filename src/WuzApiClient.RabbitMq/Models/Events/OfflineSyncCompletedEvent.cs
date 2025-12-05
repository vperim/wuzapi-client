using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Offline sync completion event envelope from whatsmeow events.OfflineSyncCompleted.
/// Emitted after the server has finished sending missed events.
/// </summary>
public sealed record OfflineSyncCompletedEventEnvelope : WhatsAppEventEnvelope<OfflineSyncCompletedEventData>
{
    [JsonPropertyName("event")]
    public override required OfflineSyncCompletedEventData Event { get; init; }
}

/// <summary>
/// Offline sync completion event data from whatsmeow events.OfflineSyncCompleted.
/// Contains the offline sync completion information.
/// </summary>
public sealed record OfflineSyncCompletedEventData
{
    /// <summary>
    /// Gets the count of events that were synced.
    /// </summary>
    [JsonPropertyName("Count")]
    public int Count { get; init; }
}

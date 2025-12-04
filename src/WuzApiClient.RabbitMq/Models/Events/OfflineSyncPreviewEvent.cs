using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Offline sync preview event from whatsmeow events.OfflineSyncPreview.
/// Emitted right after connecting if the server is going to send events that the client missed during downtime.
/// </summary>
public sealed record OfflineSyncPreviewEvent
{
    /// <summary>
    /// Gets the total number of events to be synced.
    /// </summary>
    [JsonPropertyName("Total")]
    public int Total { get; init; }

    /// <summary>
    /// Gets the number of app data changes to be synced.
    /// </summary>
    [JsonPropertyName("AppDataChanges")]
    public int AppDataChanges { get; init; }

    /// <summary>
    /// Gets the number of messages to be synced.
    /// </summary>
    [JsonPropertyName("Messages")]
    public int Messages { get; init; }

    /// <summary>
    /// Gets the number of notifications to be synced.
    /// </summary>
    [JsonPropertyName("Notifications")]
    public int Notifications { get; init; }

    /// <summary>
    /// Gets the number of receipts to be synced.
    /// </summary>
    [JsonPropertyName("Receipts")]
    public int Receipts { get; init; }
}

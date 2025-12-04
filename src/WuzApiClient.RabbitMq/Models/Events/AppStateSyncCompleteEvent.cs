using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Application state sync completion event from whatsmeow events.AppStateSyncComplete.
/// Emitted when app state is resynced.
/// </summary>
public sealed record AppStateSyncCompleteEvent
{
    /// <summary>
    /// Gets the name of the app state patch that was synced.
    /// </summary>
    [JsonPropertyName("Name")]
    public string? Name { get; init; }
}

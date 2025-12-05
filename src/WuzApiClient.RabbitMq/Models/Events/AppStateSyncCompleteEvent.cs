using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Application state sync completion event envelope from whatsmeow events.AppStateSyncComplete.
/// Emitted when app state is resynced.
/// </summary>
public sealed record AppStateSyncCompleteEventEnvelope : WhatsAppEventEnvelope<AppStateSyncCompleteEventData>
{
    [JsonPropertyName("event")]
    public override required AppStateSyncCompleteEventData Event { get; init; }
}

/// <summary>
/// Application state sync completion event data from whatsmeow events.AppStateSyncComplete.
/// Contains the app state sync completion information.
/// </summary>
public sealed record AppStateSyncCompleteEventData
{
    /// <summary>
    /// Gets the name of the app state patch that was synced.
    /// </summary>
    [JsonPropertyName("Name")]
    public string? Name { get; init; }
}

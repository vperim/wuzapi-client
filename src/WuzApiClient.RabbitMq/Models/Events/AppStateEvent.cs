using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Application state sync event envelope from whatsmeow events.AppState.
/// Emitted directly for new data received from app state syncing.
/// </summary>
public sealed record AppStateEventEnvelope : WhatsAppEventEnvelope<AppStateEventData>
{
    [JsonPropertyName("event")]
    public override required AppStateEventData Event { get; init; }
}

/// <summary>
/// Application state sync event data from whatsmeow events.AppState.
/// Contains the app state sync information.
/// </summary>
public sealed record AppStateEventData
{
    /// <summary>
    /// Gets the index array for the app state sync action.
    /// </summary>
    [JsonPropertyName("Index")]
    public List<string>? Index { get; init; }

    /// <summary>
    /// Gets the sync action value containing the app state data.
    /// </summary>
    [JsonPropertyName("SyncActionValue")]
    public object? SyncActionValue { get; init; }
}

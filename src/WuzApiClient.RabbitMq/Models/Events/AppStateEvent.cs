using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Application state sync event from whatsmeow events.AppState.
/// Emitted directly for new data received from app state syncing.
/// </summary>
public sealed record AppStateEvent
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

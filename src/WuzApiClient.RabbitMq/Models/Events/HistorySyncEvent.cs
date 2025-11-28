using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event for history synchronization progress.
/// </summary>
public sealed record HistorySyncEvent : WuzEvent
{
    /// <summary>
    /// Gets the synchronization progress.
    /// </summary>
    [JsonPropertyName("progress")]
    public int? Progress { get; init; }
}

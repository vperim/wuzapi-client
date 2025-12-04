using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when a Client Access Token (CAT) refresh operation fails.
/// Maps to whatsmeow events.CATRefreshError.
/// </summary>
public sealed record CatRefreshErrorEvent
{
    /// <summary>
    /// Gets the error message describing the CAT refresh failure.
    /// </summary>
    [JsonPropertyName("Error")]
    public string? Error { get; init; }
}

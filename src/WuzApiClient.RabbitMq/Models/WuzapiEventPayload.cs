using System.Text.Json;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models;

/// <summary>
/// Inner payload structure from wuzapi's jsonData field.
/// </summary>
internal sealed record WuzapiEventPayload
{
    /// <summary>
    /// Gets the event type.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// Gets the raw event data.
    /// </summary>
    [JsonPropertyName("event")]
    public JsonElement Event { get; init; }
}

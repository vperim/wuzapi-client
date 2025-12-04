using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event emitted when there's a connection failure with a temporary ban reason code.
/// Corresponds to whatsmeow events.TemporaryBan.
/// </summary>
public sealed record TemporaryBanEvent
{
    /// <summary>
    /// Gets the temporary ban reason code.
    /// </summary>
    [JsonPropertyName("Code")]
    public int Code { get; init; }

    /// <summary>
    /// Gets the ban expiration duration in nanoseconds.
    /// </summary>
    [JsonPropertyName("Expire")]
    public long Expire { get; init; }
}

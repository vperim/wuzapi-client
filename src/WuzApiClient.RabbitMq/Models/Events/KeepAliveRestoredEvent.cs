using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event emitted if the keepalive pings start working again after some KeepAliveTimeout events.
/// Corresponds to whatsmeow events.KeepAliveRestored.
/// This event has no additional properties.
/// </summary>
public sealed record KeepAliveRestoredEventEnvelope : WhatsAppEventEnvelope<KeepAliveRestoredEventData>
{
    [JsonPropertyName("event")]
    public override required KeepAliveRestoredEventData Event { get; init; }
}

/// <summary>
/// Event data emitted if the keepalive pings start working again after some KeepAliveTimeout events.
/// Corresponds to whatsmeow events.KeepAliveRestored.
/// This event has no additional properties.
/// </summary>
public sealed record KeepAliveRestoredEventData
{
}

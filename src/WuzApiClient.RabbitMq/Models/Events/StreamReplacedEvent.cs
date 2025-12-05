using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Stream replaced event envelope emitted when the client is disconnected by another client connecting with the same keys.
/// Corresponds to whatsmeow events.StreamReplaced.
/// </summary>
public sealed record StreamReplacedEventEnvelope : WhatsAppEventEnvelope<StreamReplacedEventData>
{
    [JsonPropertyName("event")]
    public override required StreamReplacedEventData Event { get; init; }
}

/// <summary>
/// Stream replaced event data emitted when the client is disconnected by another client connecting with the same keys.
/// This event has no additional properties.
/// </summary>
public sealed record StreamReplacedEventData
{
}

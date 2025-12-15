using System.Text.Json.Serialization;
using WuzApiClient.Common.Enums;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.RabbitMq.Models;

public interface IWhatsAppEventEnvelope
{
    /// <summary>
    /// Gets the event type.
    /// Serializes to/from JSON as a string (e.g., "Message", "QR", etc.).
    /// </summary>
    [JsonPropertyName("type")]
    [JsonConverter(typeof(WhatsAppEventTypeConverter))]
    public WhatsAppEventType EventType { get; }
}

public interface IWhatsAppEventEnvelope<out TEvent> : IWhatsAppEventEnvelope
    where TEvent : class
{
    public TEvent Event { get; }
}



public abstract record WhatsAppEventEnvelope<TEvent> : IWhatsAppEventEnvelope<TEvent>
    where TEvent : class
{
    /// <summary>
    /// Gets the event type.
    /// Serializes to/from JSON as a string (e.g., "Message", "QR", etc.).
    /// </summary>
    [JsonPropertyName("type")]
    [JsonConverter(typeof(WhatsAppEventTypeConverter))]
    public required WhatsAppEventType EventType { get; init; }

    [JsonPropertyName("event")]
    public abstract required TEvent Event { get; init; }
}

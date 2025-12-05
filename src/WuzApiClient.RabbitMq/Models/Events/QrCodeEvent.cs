using System.Text.Json.Serialization;
using WuzApiClient.Common.DataTypes;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

public interface IWhatsAppEnvelope
{
    public string Type { get; }
}

public interface IWhatsAppEnvelope<out TEvent> : IWhatsAppEnvelope
    where TEvent : class
{
    public TEvent Event { get; }
}



public abstract record WhatsAppEventEnvelope<TEvent> : IWhatsAppEnvelope<TEvent>
    where TEvent : class
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("event")]
    public abstract required TEvent Event { get; init;  }
}

/// <summary>
/// Event when a QR code is generated for pairing.
/// Corresponds to whatsmeow events.QR.
/// </summary>
public sealed record QrCodeEventEnvelope : WhatsAppEventEnvelope<string>
{
    /// <summary>
    /// Gets the QR code as a Data URI (RFC 2397).
    /// Contains the image data with media type (e.g., "data:image/png;base64,...").
    /// </summary>
    [JsonPropertyName("qrCodeBase64")]
    [JsonConverter(typeof(DataUriJsonConverter))]
    public required DataUri QrCode { get; init; }

    [JsonPropertyName("event")]
    public override required string Event { get; init; }
}

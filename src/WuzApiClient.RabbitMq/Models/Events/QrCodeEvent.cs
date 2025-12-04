using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when a QR code is generated for pairing.
/// Corresponds to whatsmeow events.QR.
/// </summary>
public sealed record QrCodeEvent
{
    /// <summary>
    /// Gets the QR code as a base64-encoded string.
    /// This is a wuzapi-added field for convenience.
    /// </summary>
    [JsonPropertyName("qrCodeBase64")]
    public string? QrCodeBase64 { get; init; }
}

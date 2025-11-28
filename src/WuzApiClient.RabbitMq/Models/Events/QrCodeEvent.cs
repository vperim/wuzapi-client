using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when a QR code is generated for pairing.
/// </summary>
public sealed record QrCodeEvent : WuzEvent
{
    /// <summary>
    /// Gets the QR code as a base64-encoded string.
    /// </summary>
    [JsonPropertyName("qrCodeBase64")]
    public string? QrCodeBase64 { get; init; }
}

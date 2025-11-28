using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.Session;

/// <summary>
/// Response containing QR code for authentication.
/// </summary>
public sealed class QrCodeResponse
{
    /// <summary>
    /// Gets or sets the base64-encoded QR code image.
    /// </summary>
    [JsonPropertyName("qrcode")]
    public string QrCode { get; set; } = string.Empty;
}

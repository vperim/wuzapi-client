using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Models.Requests.Chat;

/// <summary>
/// Request to send a sticker.
/// </summary>
public sealed class SendStickerRequest
{
    /// <summary>
    /// Gets or sets the recipient phone number.
    /// </summary>
    [JsonPropertyName("Phone")]
    public Phone Phone { get; set; }

    /// <summary>
    /// Gets or sets the base64-encoded sticker data.
    /// </summary>
    [JsonPropertyName("Sticker")]
    public string Sticker { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MIME type.
    /// </summary>
    [JsonPropertyName("MimeType")]
    public string MimeType { get; set; } = "image/webp";

    /// <summary>
    /// Gets or sets the message ID to reply to.
    /// </summary>
    [JsonPropertyName("Id")]
    public string? QuotedId { get; set; }

    /// <summary>
    /// Gets or sets the PNG thumbnail for the sticker.
    /// </summary>
    [JsonPropertyName("PngThumbnail")]
    public string? PngThumbnail { get; set; }

    /// <summary>
    /// Gets or sets the sticker pack ID.
    /// </summary>
    [JsonPropertyName("PackId")]
    public string? PackId { get; set; }

    /// <summary>
    /// Gets or sets the sticker pack name.
    /// </summary>
    [JsonPropertyName("PackName")]
    public string? PackName { get; set; }

    /// <summary>
    /// Gets or sets the sticker pack publisher.
    /// </summary>
    [JsonPropertyName("PackPublisher")]
    public string? PackPublisher { get; set; }

    /// <summary>
    /// Gets or sets the emojis associated with the sticker.
    /// </summary>
    [JsonPropertyName("Emojis")]
    public string[]? Emojis { get; set; }

    /// <summary>
    /// Gets or sets the context information for the message.
    /// </summary>
    [JsonPropertyName("ContextInfo")]
    public ContextInfo? ContextInfo { get; set; }
}

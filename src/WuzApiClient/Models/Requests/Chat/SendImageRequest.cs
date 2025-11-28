using System.Text.Json.Serialization;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Models.Requests.Chat;

/// <summary>
/// Request to send an image message.
/// </summary>
public sealed class SendImageRequest
{
    /// <summary>
    /// Gets or sets the recipient phone number.
    /// </summary>
    [JsonPropertyName("Phone")]
    public Phone Phone { get; set; }

    /// <summary>
    /// Gets or sets the base64-encoded image data.
    /// </summary>
    [JsonPropertyName("Image")]
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional caption.
    /// </summary>
    [JsonPropertyName("Caption")]
    public string? Caption { get; set; }

    /// <summary>
    /// Gets or sets the MIME type.
    /// </summary>
    [JsonPropertyName("MimeType")]
    public string MimeType { get; set; } = "image/jpeg";

    /// <summary>
    /// Gets or sets the message ID to reply to.
    /// </summary>
    [JsonPropertyName("Id")]
    public string? QuotedId { get; set; }

    /// <summary>
    /// Gets or sets the context information for quoted messages.
    /// </summary>
    [JsonPropertyName("ContextInfo")]
    public ContextInfo? ContextInfo { get; set; }
}

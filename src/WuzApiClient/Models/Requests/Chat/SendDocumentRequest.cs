using System.Text.Json.Serialization;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Models.Requests.Chat;

/// <summary>
/// Request to send a document.
/// </summary>
public sealed class SendDocumentRequest
{
    /// <summary>
    /// Gets or sets the recipient phone number.
    /// </summary>
    [JsonPropertyName("Phone")]
    public Phone Phone { get; set; }

    /// <summary>
    /// Gets or sets the base64-encoded document data.
    /// </summary>
    [JsonPropertyName("Document")]
    public string Document { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file name.
    /// </summary>
    [JsonPropertyName("FileName")]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MIME type.
    /// </summary>
    [JsonPropertyName("MimeType")]
    public string MimeType { get; set; } = "application/octet-stream";

    /// <summary>
    /// Gets or sets the message ID to reply to.
    /// </summary>
    [JsonPropertyName("Id")]
    public string? QuotedId { get; set; }

    /// <summary>
    /// Gets or sets the optional caption.
    /// </summary>
    [JsonPropertyName("Caption")]
    public string? Caption { get; set; }

    /// <summary>
    /// Gets or sets the context information for quoted messages.
    /// </summary>
    [JsonPropertyName("ContextInfo")]
    public ContextInfo? ContextInfo { get; set; }
}

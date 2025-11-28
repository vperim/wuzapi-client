using System.Text.Json.Serialization;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Models.Requests.Chat;

/// <summary>
/// Request to send a text message.
/// </summary>
public sealed class SendTextMessageRequest
{
    /// <summary>
    /// Gets or sets the recipient phone number.
    /// </summary>
    [JsonPropertyName("Phone")]
    public Phone Phone { get; set; }

    /// <summary>
    /// Gets or sets the message body.
    /// </summary>
    [JsonPropertyName("Body")]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message ID to reply to.
    /// </summary>
    [JsonPropertyName("Id")]
    public string? QuotedId { get; set; }

    /// <summary>
    /// Gets or sets whether to show link preview.
    /// </summary>
    [JsonPropertyName("LinkPreview")]
    public bool? LinkPreview { get; set; }

    /// <summary>
    /// Gets or sets the context information for quoted messages.
    /// </summary>
    [JsonPropertyName("ContextInfo")]
    public ContextInfo? ContextInfo { get; set; }
}

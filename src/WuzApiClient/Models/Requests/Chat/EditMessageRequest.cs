using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;

namespace WuzApiClient.Models.Requests.Chat;

/// <summary>
/// Request to edit a previously sent message.
/// </summary>
public sealed class EditMessageRequest
{
    /// <summary>
    /// Gets or sets the chat phone number or group ID.
    /// </summary>
    [JsonPropertyName("Phone")]
    public Phone Phone { get; set; }

    /// <summary>
    /// Gets or sets the message ID to edit.
    /// </summary>
    [JsonPropertyName("Id")]
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new message body.
    /// </summary>
    [JsonPropertyName("Body")]
    public string NewBody { get; set; } = string.Empty;
}

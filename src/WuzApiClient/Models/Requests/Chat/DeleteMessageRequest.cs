using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;

namespace WuzApiClient.Models.Requests.Chat;

/// <summary>
/// Request to delete a message.
/// </summary>
public sealed class DeleteMessageRequest
{
    /// <summary>
    /// Gets or sets the chat phone number or group ID.
    /// </summary>
    [JsonPropertyName("Phone")]
    public Phone Phone { get; set; }

    /// <summary>
    /// Gets or sets the message ID to delete.
    /// </summary>
    [JsonPropertyName("Id")]
    public string MessageId { get; set; } = string.Empty;
}

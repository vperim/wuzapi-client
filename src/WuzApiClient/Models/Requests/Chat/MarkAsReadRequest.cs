using System.Text.Json.Serialization;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Models.Requests.Chat;

/// <summary>
/// Request to mark messages as read.
/// </summary>
public sealed class MarkAsReadRequest
{
    /// <summary>
    /// Gets or sets the message IDs to mark as read.
    /// </summary>
    [JsonPropertyName("Id")]
    public string[] MessageIds { get; set; } = [];

    /// <summary>
    /// Gets or sets the chat phone number or group ID where messages should be marked as read.
    /// </summary>
    [JsonPropertyName("ChatPhone")]
    public Phone ChatPhone { get; set; }

    /// <summary>
    /// Gets or sets the sender phone number. Optional, used for group chats to identify the message sender.
    /// </summary>
    [JsonPropertyName("SenderPhone")]
    public Phone? SenderPhone { get; set; }
}

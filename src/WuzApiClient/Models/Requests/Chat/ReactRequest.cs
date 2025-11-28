using System.Text.Json.Serialization;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Models.Requests.Chat;

/// <summary>
/// Request to react to a message with an emoji.
/// </summary>
public sealed class ReactRequest
{
    /// <summary>
    /// Gets or sets the chat phone number or group ID.
    /// </summary>
    [JsonPropertyName("Phone")]
    public Phone Phone { get; set; }

    /// <summary>
    /// Gets or sets the message ID to react to.
    /// </summary>
    [JsonPropertyName("Id")]
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the reaction emoji. Serialized as "Body" to match wuzapi API expectations.
    /// </summary>
    [JsonPropertyName("Body")]
    public string Emoji { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the participant for group chat reactions.
    /// </summary>
    [JsonPropertyName("Participant")]
    public string? Participant { get; set; }
}

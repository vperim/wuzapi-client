using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.Chat;

/// <summary>
/// Response from sending a message.
/// </summary>
public sealed class SendMessageResponse
{
    /// <summary>
    /// Gets or sets the status details.
    /// </summary>
    [JsonPropertyName("details")]
    public string? Details { get; set; }

    /// <summary>
    /// Gets or sets the message ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    /// <summary>
    /// Gets the message ID (alias for Id).
    /// </summary>
    [JsonIgnore]
    public string MessageId => this.Id;
}

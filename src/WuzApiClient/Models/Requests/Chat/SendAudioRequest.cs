using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Models.Requests.Chat;

/// <summary>
/// Request to send an audio message.
/// </summary>
public sealed class SendAudioRequest
{
    /// <summary>
    /// Gets or sets the recipient phone number.
    /// </summary>
    [JsonPropertyName("Phone")]
    public Phone Phone { get; set; }

    /// <summary>
    /// Gets or sets the base64-encoded audio data.
    /// </summary>
    [JsonPropertyName("Audio")]
    public string Audio { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MIME type.
    /// </summary>
    [JsonPropertyName("MimeType")]
    public string MimeType { get; set; } = "audio/ogg";

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
    /// Gets or sets whether this is a push-to-talk voice note.
    /// </summary>
    [JsonPropertyName("PTT")]
    public bool? Ptt { get; set; }

    /// <summary>
    /// Gets or sets the audio duration in seconds.
    /// </summary>
    [JsonPropertyName("Seconds")]
    public int? Seconds { get; set; }

    /// <summary>
    /// Gets or sets the audio waveform data.
    /// </summary>
    [JsonPropertyName("Waveform")]
    public byte[]? Waveform { get; set; }

    /// <summary>
    /// Gets or sets the context information for quoted messages.
    /// </summary>
    [JsonPropertyName("ContextInfo")]
    public ContextInfo? ContextInfo { get; set; }
}

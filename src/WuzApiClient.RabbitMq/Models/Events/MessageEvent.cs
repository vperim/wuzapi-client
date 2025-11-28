using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event for incoming WhatsApp messages.
/// </summary>
public sealed record MessageEvent : WuzEvent
{
    /// <summary>
    /// Gets the message information (metadata).
    /// </summary>
    [JsonPropertyName("Info")]
    public MessageInfo? Info { get; init; }

    /// <summary>
    /// Gets the message content.
    /// </summary>
    [JsonPropertyName("Message")]
    public MessageContent? Message { get; init; }

    /// <summary>
    /// Gets the base64-encoded media content.
    /// </summary>
    [JsonPropertyName("Base64")]
    public string? Base64 { get; init; }

    /// <summary>
    /// Gets the media MIME type.
    /// </summary>
    [JsonPropertyName("MimeType")]
    public string? MimeType { get; init; }

    /// <summary>
    /// Gets the media file name.
    /// </summary>
    [JsonPropertyName("FileName")]
    public string? FileName { get; init; }

    /// <summary>
    /// Gets the S3 media information.
    /// </summary>
    [JsonPropertyName("S3")]
    public S3MediaInfo? S3 { get; init; }

    /// <summary>
    /// Gets whether this message is a sticker.
    /// </summary>
    [JsonPropertyName("IsSticker")]
    public bool IsSticker { get; init; }

    /// <summary>
    /// Gets whether the sticker is animated.
    /// </summary>
    [JsonPropertyName("StickerAnimated")]
    public bool StickerAnimated { get; init; }

    /// <summary>
    /// Gets whether this is a view-once message.
    /// </summary>
    [JsonPropertyName("IsViewOnce")]
    public bool IsViewOnce { get; init; }

    /// <summary>
    /// Gets whether this is an ephemeral message.
    /// </summary>
    [JsonPropertyName("IsEphemeral")]
    public bool IsEphemeral { get; init; }
}

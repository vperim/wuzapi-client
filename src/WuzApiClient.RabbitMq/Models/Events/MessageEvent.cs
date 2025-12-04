using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Message event from whatsmeow events.Message.
/// Contains message metadata, content, and optional media attachments.
/// </summary>
public sealed record MessageEvent
{
    // === whatsmeow fields (from "event" object) ===

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
    /// Gets whether this is a view-once message.
    /// </summary>
    [JsonPropertyName("IsViewOnce")]
    public bool IsViewOnce { get; init; }

    /// <summary>
    /// Gets whether this is an ephemeral message.
    /// </summary>
    [JsonPropertyName("IsEphemeral")]
    public bool IsEphemeral { get; init; }

    /// <summary>
    /// Gets whether this is a view-once v2 message.
    /// </summary>
    [JsonPropertyName("IsViewOnceV2")]
    public bool IsViewOnceV2 { get; init; }

    /// <summary>
    /// Gets whether this is a document with caption.
    /// </summary>
    [JsonPropertyName("IsDocumentWithCaption")]
    public bool IsDocumentWithCaption { get; init; }

    /// <summary>
    /// Gets whether this is a Lottie sticker.
    /// </summary>
    [JsonPropertyName("IsLottieSticker")]
    public bool IsLottieSticker { get; init; }

    // === wuzapi-added fields (from root level) ===

    /// <summary>
    /// Gets the base64-encoded media content (added by wuzapi).
    /// </summary>
    [JsonPropertyName("base64")]
    public string? Base64 { get; init; }

    /// <summary>
    /// Gets the media MIME type (added by wuzapi).
    /// </summary>
    [JsonPropertyName("mimeType")]
    public string? MimeType { get; init; }

    /// <summary>
    /// Gets the media file name (added by wuzapi).
    /// </summary>
    [JsonPropertyName("fileName")]
    public string? FileName { get; init; }

    /// <summary>
    /// Gets the S3 media information (added by wuzapi).
    /// </summary>
    [JsonPropertyName("s3")]
    public S3MediaInfo? S3 { get; init; }

    /// <summary>
    /// Gets whether this message is a sticker (added by wuzapi).
    /// </summary>
    [JsonPropertyName("isSticker")]
    public bool IsSticker { get; init; }

    /// <summary>
    /// Gets whether the sticker is animated (added by wuzapi).
    /// </summary>
    [JsonPropertyName("stickerAnimated")]
    public bool StickerAnimated { get; init; }
}

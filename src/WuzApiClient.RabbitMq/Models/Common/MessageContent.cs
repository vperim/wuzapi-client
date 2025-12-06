using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Common;

/// <summary>
/// Message content within a MessageEvent.
/// </summary>
public sealed record MessageContent
{
    /// <summary>
    /// Gets the text conversation content.
    /// </summary>
    [JsonPropertyName("conversation")]
    public string? Conversation { get; init; }

    /// <summary>
    /// Gets the extended text message content.
    /// </summary>
    [JsonPropertyName("extendedTextMessage")]
    public ExtendedTextMessage? ExtendedTextMessage { get; init; }

    /// <summary>
    /// Gets the image message content.
    /// </summary>
    [JsonPropertyName("imageMessage")]
    public ImageMessage? ImageMessage { get; init; }

    /// <summary>
    /// Gets the video message content.
    /// </summary>
    [JsonPropertyName("videoMessage")]
    public VideoMessage? VideoMessage { get; init; }

    /// <summary>
    /// Gets the audio message content.
    /// </summary>
    [JsonPropertyName("audioMessage")]
    public AudioMessage? AudioMessage { get; init; }

    /// <summary>
    /// Gets the document message content.
    /// </summary>
    [JsonPropertyName("documentMessage")]
    public DocumentMessage? DocumentMessage { get; init; }

    /// <summary>
    /// Gets the sticker message content.
    /// </summary>
    [JsonPropertyName("stickerMessage")]
    public StickerMessage? StickerMessage { get; init; }
}

/// <summary>
/// Extended text message with URL preview.
/// </summary>
public sealed record ExtendedTextMessage
{
    /// <summary>
    /// Gets the text content.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }

    /// <summary>
    /// Gets the matched text (URL).
    /// </summary>
    [JsonPropertyName("matchedText")]
    public string? MatchedText { get; init; }

    /// <summary>
    /// Gets the canonical URL.
    /// </summary>
    [JsonPropertyName("canonicalUrl")]
    public string? CanonicalUrl { get; init; }

    /// <summary>
    /// Gets the preview title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Gets the preview description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }
}

/// <summary>
/// Image message content.
/// </summary>
public sealed record ImageMessage
{
    /// <summary>
    /// Gets the image caption.
    /// </summary>
    [JsonPropertyName("caption")]
    public string? Caption { get; init; }

    /// <summary>
    /// Gets the MIME type.
    /// </summary>
    [JsonPropertyName("mimetype")]
    public string? MimeType { get; init; }
}

/// <summary>
/// Video message content.
/// </summary>
public sealed record VideoMessage
{
    /// <summary>
    /// Gets the video caption.
    /// </summary>
    [JsonPropertyName("caption")]
    public string? Caption { get; init; }

    /// <summary>
    /// Gets the MIME type.
    /// </summary>
    [JsonPropertyName("mimetype")]
    public string? MimeType { get; init; }

    /// <summary>
    /// Gets the duration in seconds.
    /// </summary>
    [JsonPropertyName("seconds")]
    public int? Seconds { get; init; }
}

/// <summary>
/// Audio message content.
/// </summary>
public sealed record AudioMessage
{
    /// <summary>
    /// Gets the MIME type.
    /// </summary>
    [JsonPropertyName("mimetype")]
    public string? MimeType { get; init; }

    /// <summary>
    /// Gets the duration in seconds.
    /// </summary>
    [JsonPropertyName("seconds")]
    public int? Seconds { get; init; }

    /// <summary>
    /// Gets whether this is a voice note (PTT).
    /// </summary>
    [JsonPropertyName("ptt")]
    public bool Ptt { get; init; }
}

/// <summary>
/// Document message content.
/// </summary>
public sealed record DocumentMessage
{
    /// <summary>
    /// Gets the file name.
    /// </summary>
    [JsonPropertyName("fileName")]
    public string? FileName { get; init; }

    /// <summary>
    /// Gets the MIME type.
    /// </summary>
    [JsonPropertyName("mimetype")]
    public string? MimeType { get; init; }

    /// <summary>
    /// Gets the document title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }
}

/// <summary>
/// Sticker message content.
/// </summary>
public sealed record StickerMessage
{
    /// <summary>
    /// Gets the MIME type.
    /// </summary>
    [JsonPropertyName("mimetype")]
    public string? MimeType { get; init; }

    /// <summary>
    /// Gets whether this is an animated sticker.
    /// </summary>
    [JsonPropertyName("isAnimated")]
    public bool IsAnimated { get; init; }
}

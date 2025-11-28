using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Requests.Chat;

/// <summary>
/// Base request for downloading media from WhatsApp servers.
/// </summary>
public abstract class MediaDownloadRequest
{
    /// <summary>
    /// Gets or sets the media URL from WhatsApp.
    /// </summary>
    [JsonPropertyName("Url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the media decryption key.
    /// </summary>
    [JsonPropertyName("Mediakey")]
    public string MediaKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MIME type of the media.
    /// </summary>
    [JsonPropertyName("Mimetype")]
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SHA256 hash of the file.
    /// </summary>
    [JsonPropertyName("FileSHA256")]
    public string FileSha256 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file length in bytes.
    /// </summary>
    [JsonPropertyName("FileLength")]
    public long FileLength { get; set; }
}

/// <summary>
/// Request to download an image from WhatsApp servers.
/// </summary>
public sealed class DownloadImageRequest : MediaDownloadRequest
{
}

/// <summary>
/// Request to download a video from WhatsApp servers.
/// </summary>
public sealed class DownloadVideoRequest : MediaDownloadRequest
{
}

/// <summary>
/// Request to download a document from WhatsApp servers.
/// </summary>
public sealed class DownloadDocumentRequest : MediaDownloadRequest
{
}

/// <summary>
/// Request to download audio from WhatsApp servers.
/// </summary>
public sealed class DownloadAudioRequest : MediaDownloadRequest
{
}

using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.Download;

/// <summary>
/// Response containing downloaded media data.
/// </summary>
public sealed class MediaDownloadResponse
{
    /// <summary>
    /// Gets or sets the base64-encoded media data.
    /// </summary>
    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MIME type.
    /// </summary>
    [JsonPropertyName("mimetype")]
    public string? MimeType { get; set; }

    /// <summary>
    /// Gets or sets the file name.
    /// </summary>
    [JsonPropertyName("filename")]
    public string? FileName { get; set; }
}

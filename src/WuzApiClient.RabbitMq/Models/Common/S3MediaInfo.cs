using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Common;

/// <summary>
/// S3 media information for uploaded media.
/// </summary>
public sealed record S3MediaInfo
{
    /// <summary>
    /// Gets the S3 URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    /// Gets the S3 bucket name.
    /// </summary>
    [JsonPropertyName("bucket")]
    public string? Bucket { get; init; }

    /// <summary>
    /// Gets the S3 object key.
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; init; }
}

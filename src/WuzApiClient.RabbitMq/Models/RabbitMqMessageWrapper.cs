using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models;

/// <summary>
/// Wrapper for RabbitMQ messages published by wuzapi.
/// </summary>
internal sealed record RabbitMqMessageWrapper
{
    /// <summary>
    /// Gets the user ID.
    /// </summary>
    [JsonPropertyName("userID")]
    public string UserId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the instance name.
    /// </summary>
    [JsonPropertyName("instanceName")]
    public string InstanceName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the JSON data as a string.
    /// </summary>
    [JsonPropertyName("jsonData")]
    public string JsonData { get; init; } = string.Empty;
}

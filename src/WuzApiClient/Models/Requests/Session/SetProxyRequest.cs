using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Requests.Session;

/// <summary>
/// Request to set proxy configuration.
/// </summary>
public sealed class SetProxyRequest
{
    /// <summary>
    /// Gets or sets the proxy URL.
    /// </summary>
    [JsonPropertyName("proxy")]
    public string Proxy { get; set; } = string.Empty;
}

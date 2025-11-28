using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Requests.Session;

/// <summary>
/// Request to connect a WhatsApp session.
/// </summary>
public sealed class ConnectSessionRequest
{
    /// <summary>
    /// Gets or sets the events to subscribe to.
    /// </summary>
    [JsonPropertyName("subscribe")]
    public string[]? Subscribe { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to connect immediately.
    /// </summary>
    [JsonPropertyName("immediate")]
    public bool Immediate { get; set; } = true;
}

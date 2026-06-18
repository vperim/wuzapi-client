using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Requests.Session;

/// <summary>
/// Configures how many messages of history the gateway retains
/// (POST /session/history). 0 disables retention; a positive value caps it.
/// </summary>
public sealed class SetHistoryRequest
{
    /// <summary>Retention limit (0 = disabled).</summary>
    [JsonPropertyName("history")]
    public int History { get; set; }
}

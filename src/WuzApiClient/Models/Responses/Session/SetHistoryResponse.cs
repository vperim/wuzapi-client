using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.Session;

/// <summary>
/// Result of configuring history retention (POST /session/history).
/// </summary>
public sealed class SetHistoryResponse
{
    /// <summary>Human-readable status (e.g. "History configured successfully").</summary>
    [JsonPropertyName("Details")]
    public string? Details { get; set; }

    /// <summary>The retention limit that was applied.</summary>
    [JsonPropertyName("History")]
    public int History { get; set; }
}

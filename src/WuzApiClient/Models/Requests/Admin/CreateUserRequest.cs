using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Requests.Admin;

/// <summary>
/// Request to create a new user.
/// </summary>
public sealed class CreateUserRequest
{
    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user token.
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the webhook URL.
    /// </summary>
    [JsonPropertyName("webhook")]
    public string? Webhook { get; set; }

    /// <summary>
    /// Gets or sets the events to subscribe to.
    /// </summary>
    [JsonPropertyName("events")]
    public string? Events { get; set; }
}

using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.Admin;

/// <summary>
/// Response containing user information.
/// </summary>
public sealed class UserResponse
{
    /// <summary>
    /// Gets or sets the user ID (hash string).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the user token.
    /// </summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>
    /// Gets or sets the webhook URL.
    /// </summary>
    [JsonPropertyName("webhook")]
    public string? Webhook { get; set; }

    /// <summary>
    /// Gets or sets the subscribed events (comma-separated string).
    /// </summary>
    [JsonPropertyName("events")]
    public string? Events { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is connected.
    /// </summary>
    [JsonPropertyName("connected")]
    public bool Connected { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is logged in.
    /// </summary>
    [JsonPropertyName("loggedIn")]
    public bool LoggedIn { get; set; }

    /// <summary>
    /// Gets or sets the JID (WhatsApp ID) when logged in.
    /// </summary>
    [JsonPropertyName("jid")]
    public string? Jid { get; set; }
}

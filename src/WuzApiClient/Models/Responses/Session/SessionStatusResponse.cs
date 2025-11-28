using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.Session;

/// <summary>
/// Response containing session status information.
/// </summary>
public sealed class SessionStatusResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the session is connected.
    /// </summary>
    [JsonPropertyName("connected")]
    public bool Connected { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the session is logged in.
    /// </summary>
    [JsonPropertyName("loggedIn")]
    public bool LoggedIn { get; set; }

    /// <summary>
    /// Gets or sets the JID of the logged-in user.
    /// </summary>
    [JsonPropertyName("jid")]
    public string? Jid { get; set; }
}

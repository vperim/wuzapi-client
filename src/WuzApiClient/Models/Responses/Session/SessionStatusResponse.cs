using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

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
    [MemberNotNullWhen(true, nameof(Jid))]
    public bool LoggedIn { get; set; }

    /// <summary>
    /// Gets or sets the JID of the logged-in user.
    /// </summary>
    [JsonPropertyName("jid")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? Jid { get; set; }
}

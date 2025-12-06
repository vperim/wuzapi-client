using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.Models.Responses.User;

/// <summary>
/// Response from checking phone numbers.
/// </summary>
public sealed class CheckPhonesResponse
{
    /// <summary>
    /// Gets or sets the check results.
    /// </summary>
    [JsonPropertyName("Users")]
    public CheckPhoneResult[] Users { get; set; } = [];
}

/// <summary>
/// WuzResult for a single phone number check.
/// </summary>
public sealed class CheckPhoneResult
{
    /// <summary>
    /// Gets or sets the query phone number.
    /// </summary>
    [JsonPropertyName("Query")]
    public string? Query { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the phone has WhatsApp.
    /// </summary>
    [JsonPropertyName("IsInWhatsapp")]
    public bool IsInWhatsapp { get; set; }

    /// <summary>
    /// Gets or sets the JID if on WhatsApp.
    /// </summary>
    [JsonPropertyName("JID")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? Jid { get; set; }

    /// <summary>
    /// Gets or sets the verified business name.
    /// </summary>
    [JsonPropertyName("VerifiedName")]
    public string? VerifiedName { get; set; }
}

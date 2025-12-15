using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.Models.Responses.Admin;

/// <summary>
/// Response returned when a user is fully deleted with cleanup.
/// </summary>
public sealed class DeletedUserResponse
{
    /// <summary>
    /// Gets or sets the deleted user's ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the deleted user's name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the deleted user's JID (WhatsApp ID).
    /// </summary>
    [JsonPropertyName("jid")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? Jid { get; set; }
}

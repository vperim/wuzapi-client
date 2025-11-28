using System.Text.Json.Serialization;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Models.Requests.Chat;

/// <summary>
/// Request to send a contact card.
/// </summary>
public sealed class SendContactRequest
{
    /// <summary>
    /// Gets or sets the recipient phone number.
    /// </summary>
    [JsonPropertyName("Phone")]
    public Phone Phone { get; set; }

    /// <summary>
    /// Gets or sets the vCard data.
    /// </summary>
    [JsonPropertyName("Vcard")]
    public string VCard { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message ID to reply to.
    /// </summary>
    [JsonPropertyName("Id")]
    public string? QuotedId { get; set; }

    /// <summary>
    /// Gets or sets the contact name.
    /// </summary>
    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the context information for the message.
    /// </summary>
    [JsonPropertyName("ContextInfo")]
    public ContextInfo? ContextInfo { get; set; }
}

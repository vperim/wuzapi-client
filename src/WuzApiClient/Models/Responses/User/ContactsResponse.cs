using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.User;

/// <summary>
/// Response containing contact list.
/// The API returns contacts as a dictionary with JID as key.
/// </summary>
public sealed class ContactsResponse
{
    /// <summary>
    /// Gets or sets the contacts dictionary (JID -> ContactInfo).
    /// </summary>
    public Dictionary<string, ContactInfo> Contacts { get; set; } = new();

    /// <summary>
    /// Creates a ContactsResponse from a dictionary.
    /// </summary>
    internal static ContactsResponse FromDictionary(Dictionary<string, ContactInfo> contacts)
        => new() { Contacts = contacts };
}

/// <summary>
/// Contact information.
/// </summary>
public sealed class ContactInfo
{
    /// <summary>
    /// Gets or sets the business name.
    /// </summary>
    [JsonPropertyName("BusinessName")]
    public string? BusinessName { get; set; }

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    [JsonPropertyName("FirstName")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the full name.
    /// </summary>
    [JsonPropertyName("FullName")]
    public string? FullName { get; set; }

    /// <summary>
    /// Gets or sets the push name.
    /// </summary>
    [JsonPropertyName("PushName")]
    public string? PushName { get; set; }

    /// <summary>
    /// Gets or sets whether the contact was found.
    /// </summary>
    [JsonPropertyName("Found")]
    public bool Found { get; set; }
}

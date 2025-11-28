using System.Text.Json.Serialization;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Models.Requests.Chat;

/// <summary>
/// Request to send an interactive list message.
/// </summary>
public sealed class SendListRequest
{
    /// <summary>
    /// Gets or sets the recipient phone number.
    /// </summary>
    [JsonPropertyName("Phone")]
    public Phone Phone { get; set; } = default!;

    /// <summary>
    /// Gets or sets the button text displayed to open the list.
    /// </summary>
    [JsonPropertyName("ButtonText")]
    public string ButtonText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description/body text.
    /// </summary>
    [JsonPropertyName("Desc")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the top text (title).
    /// </summary>
    [JsonPropertyName("TopText")]
    public string? TopText { get; set; }

    /// <summary>
    /// Gets or sets the list sections.
    /// </summary>
    [JsonPropertyName("Sections")]
    public ListSection[]? Sections { get; set; }

    /// <summary>
    /// Gets or sets the footer text.
    /// </summary>
    [JsonPropertyName("FooterText")]
    public string? FooterText { get; set; }

    /// <summary>
    /// Gets or sets the ID of a message to quote.
    /// </summary>
    [JsonPropertyName("Id")]
    public string? QuotedId { get; set; }
}

/// <summary>
/// A section in an interactive list.
/// </summary>
public sealed class ListSection
{
    /// <summary>
    /// Gets or sets the section title.
    /// </summary>
    [JsonPropertyName("Title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rows in this section.
    /// </summary>
    [JsonPropertyName("Rows")]
    public ListRow[]? Rows { get; set; }
}

/// <summary>
/// A row in a list section.
/// </summary>
public sealed class ListRow
{
    /// <summary>
    /// Gets or sets the unique row identifier.
    /// </summary>
    [JsonPropertyName("RowId")]
    public string RowId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the row title.
    /// </summary>
    [JsonPropertyName("Title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the row description.
    /// </summary>
    [JsonPropertyName("Description")]
    public string? Description { get; set; }
}

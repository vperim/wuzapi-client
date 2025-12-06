using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;

namespace WuzApiClient.Models.Requests.Chat;

/// <summary>
/// Request to send a poll message.
/// </summary>
public sealed class SendPollRequest
{
    /// <summary>
    /// Gets or sets the recipient phone number.
    /// </summary>
    [JsonPropertyName("Phone")]
    public Phone Phone { get; set; } = default!;

    /// <summary>
    /// Gets or sets the poll question.
    /// </summary>
    [JsonPropertyName("Question")]
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the poll options.
    /// </summary>
    [JsonPropertyName("Options")]
    public string[] Options { get; set; } = [];

    /// <summary>
    /// Gets or sets the maximum number of selections allowed.
    /// </summary>
    [JsonPropertyName("MaxSelections")]
    public int? MaxSelections { get; set; }

    /// <summary>
    /// Gets or sets the ID of a message to quote.
    /// </summary>
    [JsonPropertyName("Id")]
    public string? QuotedId { get; set; }
}

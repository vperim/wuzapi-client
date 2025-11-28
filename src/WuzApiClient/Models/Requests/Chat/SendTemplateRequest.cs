using System.Text.Json.Serialization;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Models.Requests.Chat;

/// <summary>
/// Request to send a template message with buttons.
/// </summary>
public sealed class SendTemplateRequest
{
    /// <summary>
    /// Gets or sets the recipient phone number.
    /// </summary>
    [JsonPropertyName("phone")]
    public Phone Phone { get; set; }

    /// <summary>
    /// Gets or sets the template content.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional footer text.
    /// </summary>
    [JsonPropertyName("footer")]
    public string? Footer { get; set; }

    /// <summary>
    /// Gets or sets the buttons.
    /// </summary>
    [JsonPropertyName("buttons")]
    public TemplateButton[]? Buttons { get; set; }
}

/// <summary>
/// Represents a template button.
/// </summary>
public sealed class TemplateButton
{
    /// <summary>
    /// Gets or sets the button display text.
    /// </summary>
    [JsonPropertyName("displayText")]
    public string DisplayText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the button ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}

using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Json;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Models.Requests.Chat;

/// <summary>
/// Request to send an interactive button message.
/// </summary>
public sealed class SendButtonsRequest
{
    /// <summary>
    /// Gets or sets the recipient phone number.
    /// </summary>
    [JsonPropertyName("Phone")]
    public Phone Phone { get; set; } = default!;

    /// <summary>
    /// Gets or sets the message body text.
    /// </summary>
    [JsonPropertyName("Body")]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional header title.
    /// </summary>
    [JsonPropertyName("Title")]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the optional footer text.
    /// </summary>
    [JsonPropertyName("Footer")]
    public string? Footer { get; set; }

    /// <summary>
    /// Gets or sets the optional base64-encoded image for the message header.
    /// </summary>
    [JsonPropertyName("Image")]
    public string? Image { get; set; }

    /// <summary>
    /// Gets or sets the buttons.
    /// </summary>
    [JsonPropertyName("Buttons")]
    public ButtonDefinition[] Buttons { get; set; } = [];

    /// <summary>
    /// Gets or sets the message ID to reply to.
    /// </summary>
    [JsonPropertyName("Id")]
    public string? QuotedId { get; set; }

    /// <summary>
    /// Gets or sets the context information for quoted messages.
    /// </summary>
    [JsonPropertyName("ContextInfo")]
    public ContextInfo? ContextInfo { get; set; }
}

/// <summary>
/// Defines a button in an interactive button message.
/// </summary>
public sealed class ButtonDefinition
{
    /// <summary>
    /// Gets or sets the button type. Defaults to Reply.
    /// </summary>
    [JsonPropertyName("type")]
    [JsonConverter(typeof(ButtonTypeConverter))]
    public ButtonType Type { get; set; } = ButtonType.Reply;

    /// <summary>
    /// Gets or sets the button display text.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the button ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL for CtaUrl buttons.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the phone number for CtaCall buttons.
    /// </summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the copy code for Copy buttons.
    /// </summary>
    [JsonPropertyName("copy_code")]
    public string? CopyCode { get; set; }
}

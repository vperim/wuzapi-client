using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event for chat typing and recording indicators.
/// Maps to whatsmeow events.ChatPresence.
/// </summary>
public sealed record ChatPresenceEventEnvelope : WhatsAppEventEnvelope<ChatPresenceEventData>
{
    [JsonPropertyName("event")]
    public override required ChatPresenceEventData Event { get; init; }
}

/// <summary>
/// Event data for chat typing and recording indicators.
/// Maps to whatsmeow events.ChatPresence.
/// </summary>
public sealed record ChatPresenceEventData
{
    // === MessageSource fields (embedded in Go) ===

    /// <summary>
    /// Gets the chat JID.
    /// </summary>
    [JsonPropertyName("Chat")]
    public string? Chat { get; init; }

    /// <summary>
    /// Gets the sender JID.
    /// </summary>
    [JsonPropertyName("Sender")]
    public string? Sender { get; init; }

    /// <summary>
    /// Gets whether this is from the current user.
    /// </summary>
    [JsonPropertyName("IsFromMe")]
    public bool IsFromMe { get; init; }

    /// <summary>
    /// Gets whether this is a group chat.
    /// </summary>
    [JsonPropertyName("IsGroup")]
    public bool IsGroup { get; init; }

    // === ChatPresence fields ===

    /// <summary>
    /// Gets the presence state (composing, paused).
    /// </summary>
    [JsonPropertyName("State")]
    public string? State { get; init; }

    /// <summary>
    /// Gets the media type being composed (text, audio).
    /// </summary>
    [JsonPropertyName("Media")]
    public string? Media { get; init; }
}

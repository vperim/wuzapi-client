using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when a contact is blocked or unblocked.
/// Maps to whatsmeow events.BlocklistChange.
/// </summary>
public sealed record BlocklistChangeEventEnvelope : WhatsAppEventEnvelope<BlocklistChangeEventData>
{
    [JsonPropertyName("event")]
    public override required BlocklistChangeEventData Event { get; init; }
}

/// <summary>
/// Event when a contact is blocked or unblocked.
/// Maps to whatsmeow events.BlocklistChange.
/// </summary>
public sealed record BlocklistChangeEventData
{
    /// <summary>
    /// Gets the JID of the contact whose block status changed.
    /// </summary>
    [JsonPropertyName("JID")]
    public string? Jid { get; init; }

    /// <summary>
    /// Gets the action performed ("block" or "unblock").
    /// </summary>
    [JsonPropertyName("Action")]
    public string? Action { get; init; }
}

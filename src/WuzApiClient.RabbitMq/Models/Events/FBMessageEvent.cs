using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event data for Facebook/Meta messages.
/// Maps to whatsmeow events.FBMessage.
/// </summary>
public sealed record FbMessageEventData
{
    /// <summary>
    /// Gets the message information (metadata).
    /// </summary>
    [JsonPropertyName("Info")]
    public MessageInfo? Info { get; init; }

    /// <summary>
    /// Gets the Facebook message content.
    /// </summary>
    [JsonPropertyName("Message")]
    public MessageContent? Message { get; init; }

    /// <summary>
    /// Gets whether this is a view-once message.
    /// </summary>
    [JsonPropertyName("IsViewOnce")]
    public bool IsViewOnce { get; init; }

    /// <summary>
    /// Gets whether this is an ephemeral message.
    /// </summary>
    [JsonPropertyName("IsEphemeral")]
    public bool IsEphemeral { get; init; }
}

/// <summary>
/// Event envelope for Facebook/Meta messages.
/// Maps to whatsmeow events.FBMessage.
/// </summary>
public sealed record FbMessageEventEnvelope : WhatsAppEventEnvelope<FbMessageEventData>
{
    [JsonPropertyName("event")]
    public override required FbMessageEventData Event { get; init; }
}

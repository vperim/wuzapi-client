using System;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event emitted when the user receives a call on WhatsApp.
/// Corresponds to whatsmeow events.CallOffer.
/// </summary>
public sealed record CallOfferEvent
{
    // BasicCallMeta fields
    /// <summary>
    /// Gets the JID of the sender.
    /// </summary>
    [JsonPropertyName("From")]
    public string? From { get; init; }

    /// <summary>
    /// Gets the timestamp of the event.
    /// </summary>
    [JsonPropertyName("Timestamp")]
    public DateTimeOffset Timestamp { get; init; }

    /// <summary>
    /// Gets the JID of the call creator.
    /// </summary>
    [JsonPropertyName("CallCreator")]
    public string? CallCreator { get; init; }

    /// <summary>
    /// Gets the alternative JID of the call creator.
    /// </summary>
    [JsonPropertyName("CallCreatorAlt")]
    public string? CallCreatorAlt { get; init; }

    /// <summary>
    /// Gets the call identifier.
    /// </summary>
    [JsonPropertyName("CallID")]
    public string? CallId { get; init; }

    /// <summary>
    /// Gets the group JID if this is a group call.
    /// </summary>
    [JsonPropertyName("GroupJID")]
    public string? GroupJid { get; init; }

    // CallRemoteMeta fields
    /// <summary>
    /// Gets the platform of the caller's WhatsApp client.
    /// </summary>
    [JsonPropertyName("RemotePlatform")]
    public string? RemotePlatform { get; init; }

    /// <summary>
    /// Gets the version of the caller's WhatsApp client.
    /// </summary>
    [JsonPropertyName("RemoteVersion")]
    public string? RemoteVersion { get; init; }
}

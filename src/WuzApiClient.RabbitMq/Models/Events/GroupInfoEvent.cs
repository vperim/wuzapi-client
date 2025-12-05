using System;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event for group information updates.
/// Maps to whatsmeow events.GroupInfo.
/// </summary>
public sealed record GroupInfoEventEnvelope : WhatsAppEventEnvelope<GroupInfoEventData>
{
    [JsonPropertyName("event")]
    public override required GroupInfoEventData Event { get; init; }
}

/// <summary>
/// Data for a group information event.
/// </summary>
public sealed record GroupInfoEventData
{
    /// <summary>
    /// Gets the group JID.
    /// </summary>
    [JsonPropertyName("JID")]
    public string? Jid { get; init; }

    /// <summary>
    /// Gets the group name.
    /// </summary>
    [JsonPropertyName("Name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the group topic/description.
    /// </summary>
    [JsonPropertyName("Topic")]
    public string? Topic { get; init; }

    /// <summary>
    /// Gets whether the group is locked (only admins can send messages).
    /// </summary>
    [JsonPropertyName("Locked")]
    public bool? Locked { get; init; }

    /// <summary>
    /// Gets whether the group is announced (only admins can send messages).
    /// </summary>
    [JsonPropertyName("Announce")]
    public bool? Announce { get; init; }

    /// <summary>
    /// Gets whether the group is ephemeral.
    /// </summary>
    [JsonPropertyName("Ephemeral")]
    public bool? Ephemeral { get; init; }

    /// <summary>
    /// Gets when the group was created.
    /// </summary>
    [JsonPropertyName("GroupCreated")]
    public DateTimeOffset? GroupCreated { get; init; }

    /// <summary>
    /// Gets the participant version ID.
    /// </summary>
    [JsonPropertyName("ParticipantVersionID")]
    public string? ParticipantVersionId { get; init; }

    /// <summary>
    /// Gets the join method (invite link, etc.).
    /// </summary>
    [JsonPropertyName("JoinMethod")]
    public string? JoinMethod { get; init; }
}

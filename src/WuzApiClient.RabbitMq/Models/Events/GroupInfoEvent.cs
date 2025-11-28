using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event for group information updates.
/// </summary>
public sealed record GroupInfoEvent : WuzEvent
{
    /// <summary>
    /// Gets the group JID.
    /// </summary>
    [JsonPropertyName("groupJid")]
    public string? GroupJid { get; init; }

    /// <summary>
    /// Gets the group name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the group topic.
    /// </summary>
    [JsonPropertyName("topic")]
    public string? Topic { get; init; }
}

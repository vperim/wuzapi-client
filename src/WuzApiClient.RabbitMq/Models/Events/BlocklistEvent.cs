using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event related to blocklist operations.
/// Maps to whatsmeow events.Blocklist.
/// </summary>
public sealed record BlocklistEvent
{
    /// <summary>
    /// Gets the action performed on the blocklist ("" for default, "modify" for modifications).
    /// </summary>
    [JsonPropertyName("Action")]
    public string? Action { get; init; }

    /// <summary>
    /// Gets the current blocklist hash.
    /// </summary>
    [JsonPropertyName("DHash")]
    public string? DHash { get; init; }

    /// <summary>
    /// Gets the previous blocklist hash.
    /// </summary>
    [JsonPropertyName("PrevDHash")]
    public string? PrevDHash { get; init; }

    /// <summary>
    /// Gets the list of changes made to the blocklist.
    /// </summary>
    [JsonPropertyName("Changes")]
    public BlocklistChange[]? Changes { get; init; }
}

/// <summary>
/// Represents a single change to the blocklist.
/// </summary>
public sealed record BlocklistChange
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

using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Privacy settings change event from whatsmeow events.PrivacySettings.
/// Emitted when the user changes their privacy settings.
/// </summary>
public sealed record PrivacySettingsEvent
{
    /// <summary>
    /// Gets the new privacy settings.
    /// </summary>
    [JsonPropertyName("NewSettings")]
    public object? NewSettings { get; init; }

    /// <summary>
    /// Gets whether the group add setting changed.
    /// </summary>
    [JsonPropertyName("GroupAddChanged")]
    public bool GroupAddChanged { get; init; }

    /// <summary>
    /// Gets whether the last seen setting changed.
    /// </summary>
    [JsonPropertyName("LastSeenChanged")]
    public bool LastSeenChanged { get; init; }

    /// <summary>
    /// Gets whether the status setting changed.
    /// </summary>
    [JsonPropertyName("StatusChanged")]
    public bool StatusChanged { get; init; }

    /// <summary>
    /// Gets whether the profile setting changed.
    /// </summary>
    [JsonPropertyName("ProfileChanged")]
    public bool ProfileChanged { get; init; }

    /// <summary>
    /// Gets whether the read receipts setting changed.
    /// </summary>
    [JsonPropertyName("ReadReceiptsChanged")]
    public bool ReadReceiptsChanged { get; init; }

    /// <summary>
    /// Gets whether the online setting changed.
    /// </summary>
    [JsonPropertyName("OnlineChanged")]
    public bool OnlineChanged { get; init; }

    /// <summary>
    /// Gets whether the call add setting changed.
    /// </summary>
    [JsonPropertyName("CallAddChanged")]
    public bool CallAddChanged { get; init; }
}

namespace WuzApiClient.EventDashboard.Models.Metadata;

/// <summary>
/// Metadata extracted from presence events (Presence, ChatPresence).
/// </summary>
public sealed record PresenceMetadata : EventMetadata
{
    public required string UserJid { get; init; }
    public required string State { get; init; }
    public required bool IsAvailable { get; init; }
    public DateTimeOffset? LastSeen { get; init; }
    public string? Media { get; init; }
    public required bool IsChatPresence { get; init; }
}

namespace WuzApiClient.EventDashboard.Models.Metadata;

/// <summary>
/// Metadata extracted from group events (GroupInfo, JoinedGroup, Picture).
/// </summary>
public sealed record GroupMetadata : EventMetadata
{
    public required string GroupJid { get; init; }
    public string? GroupName { get; init; }
    public string? Topic { get; init; }
    public bool? IsLocked { get; init; }
    public bool? IsAnnounce { get; init; }
    public string? JoinReason { get; init; }
    public DateTimeOffset? CreateTime { get; init; }
    public string? PictureAuthor { get; init; }
    public required bool IsPictureRemoved { get; init; }
}

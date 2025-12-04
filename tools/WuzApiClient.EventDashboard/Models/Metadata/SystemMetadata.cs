namespace WuzApiClient.EventDashboard.Models.Metadata;

/// <summary>
/// Metadata extracted from system events (Sync, Newsletter, Privacy, Blocklist, Keep-alive, etc.).
/// </summary>
public sealed record SystemMetadata : EventMetadata
{
    public required string SystemEvent { get; init; }
    public string? Details { get; init; }
    public string? NewsletterName { get; init; }
    public int? SubscriberCount { get; init; }
    public string? SettingType { get; init; }
    public required bool IsSyncComplete { get; init; }
}

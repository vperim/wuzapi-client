namespace WuzApiClient.EventDashboard.Models.Metadata;

/// <summary>
/// Metadata extracted from receipt events (read, delivery, played receipts).
/// </summary>
public sealed record ReceiptMetadata : EventMetadata
{
    public required string ChatJid { get; init; }
    public required string SenderJid { get; init; }
    public required IReadOnlyList<string> MessageIds { get; init; }
    public DateTimeOffset? ReceiptTimestamp { get; init; }
    public required string ReceiptType { get; init; }
    public required string State { get; init; }
    public required bool IsGroup { get; init; }
}

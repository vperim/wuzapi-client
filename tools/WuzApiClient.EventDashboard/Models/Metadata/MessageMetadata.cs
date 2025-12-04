namespace WuzApiClient.EventDashboard.Models.Metadata;

/// <summary>
/// Metadata extracted from message events (Message, UndecryptableMessage, FbMessage).
/// </summary>
public sealed record MessageMetadata : EventMetadata
{
    public required string MessageId { get; init; }
    public required string ChatJid { get; init; }
    public required string SenderJid { get; init; }
    public string? PushName { get; init; }
    public required bool IsFromMe { get; init; }
    public required bool IsGroup { get; init; }
    public DateTimeOffset? MessageTimestamp { get; init; }
    public required string MessageType { get; init; }
    public string? MediaType { get; init; }
    public required string ContentPreview { get; init; }
    public required bool IsViewOnce { get; init; }
    public required bool IsEphemeral { get; init; }
}

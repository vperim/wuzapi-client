namespace WuzApiClient.EventDashboard.Models.Metadata;

/// <summary>
/// Metadata extracted from connection lifecycle events (Connected, Disconnected, QR codes, pairing, etc.).
/// </summary>
public sealed record ConnectionMetadata : EventMetadata
{
    public required string ConnectionEvent { get; init; }
    public string? Reason { get; init; }
    public string? ErrorMessage { get; init; }
    public int? RetryAttempts { get; init; }
    public string? QrCodeBase64 { get; init; }
}

namespace WuzApiClient.EventDashboard.Models.Metadata;

/// <summary>
/// Metadata extracted from call events (CallOffer, CallAccept, CallTerminate, etc.).
/// </summary>
public sealed record CallMetadata : EventMetadata
{
    public required string CallEvent { get; init; }
    public string? CallId { get; init; }
    public string? FromJid { get; init; }
    public string? RemotePlatform { get; init; }
    public string? RemoteVersion { get; init; }
    public string? TerminateReason { get; init; }
    public string? MediaType { get; init; }
    public string? CallType { get; init; }
}

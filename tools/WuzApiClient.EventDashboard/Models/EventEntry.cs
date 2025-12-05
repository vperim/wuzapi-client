using WuzApiClient.EventDashboard.Models.Metadata;
using WuzApiClient.RabbitMq.Models.Wuz;

namespace WuzApiClient.EventDashboard.Models;

public sealed record EventEntry
{
    public required string Id { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required string EventType { get; init; }
    public required EventCategory Category { get; init; }
    public required string UserId { get; init; }
    public required string InstanceName { get; init; }

    // Typed metadata extracted from strongly-typed event properties
    public required EventMetadata Metadata { get; init; }

    // Original envelope and raw JSON for diagnostics/backward compatibility
    public required IWuzEventEnvelope Event { get; init; }
    public required string RawJson { get; init; }
    public string? Error { get; init; }
}

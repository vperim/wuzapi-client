using WuzApiClient.RabbitMq.Models;

namespace WuzApiClient.EventDashboard.Models;

public sealed record EventEntry
{
    public required string Id { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required string EventType { get; init; }
    public required EventCategory Category { get; init; }
    public required string UserId { get; init; }
    public required string InstanceName { get; init; }
    public required WuzEvent Event { get; init; }
    public required string RawJson { get; init; }
    public string? Error { get; init; }
}

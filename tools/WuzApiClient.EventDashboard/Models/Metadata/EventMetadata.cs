namespace WuzApiClient.EventDashboard.Models.Metadata;

/// <summary>
/// Base abstraction for strongly-typed event metadata.
/// Uses discriminated union pattern for type-safe metadata storage.
/// </summary>
public abstract record EventMetadata
{
    public required EventCategory Category { get; init; }
}

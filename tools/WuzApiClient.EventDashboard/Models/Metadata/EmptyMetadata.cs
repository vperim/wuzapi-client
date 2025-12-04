namespace WuzApiClient.EventDashboard.Models.Metadata;

/// <summary>
/// Metadata for events with no extractable metadata (marker events like Connected, Disconnected, etc.).
/// </summary>
public sealed record EmptyMetadata : EventMetadata
{
    // Intentionally empty - marker events have no additional metadata
}

using System;

namespace WuzApiClient.RabbitMq.Models.Wuz;

/// <summary>
///     Generic envelope for typed access to event data.
/// </summary>
/// <typeparam name="TPayload">The event payload type.</typeparam>
public sealed record WuzEventEnvelope<TPayload> : IWuzEventEnvelope<TPayload>
    where TPayload : class
{
    /// <inheritdoc />
    public required TPayload Payload { get; init; }

    public required WuzEventMetadata Metadata { get; init; }

    /// <inheritdoc />
    public DateTimeOffset ReceivedAt { get; init; } = DateTimeOffset.UtcNow;
}
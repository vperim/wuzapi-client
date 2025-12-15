using System;

namespace WuzApiClient.RabbitMq.Models.Wuz;

/// <summary>
///     Generic envelope for typed access to event data.
/// </summary>
/// <typeparam name="T">The event payload type.</typeparam>
public sealed record WuzEventEnvelope<T> : IWuzEventEnvelope<T>
    where T : class, IWhatsAppEventEnvelope
{
    /// <inheritdoc />
    public required T Payload { get; init; }

    public required WuzEventMetadata Metadata { get; init; }

    /// <inheritdoc />
    public DateTimeOffset ReceivedAt { get; init; } = DateTimeOffset.UtcNow;
}

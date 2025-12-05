using System;

namespace WuzApiClient.RabbitMq.Models.Wuz;

/// <summary>
/// Non-generic base for polymorphic collections and routing.
/// Transport metadata for wuzapi events.
/// </summary>
public interface IWuzEventEnvelope
{
    WuzEventMetadata Metadata { get; }

    /// <summary>
    /// Gets the timestamp when this event was received by the client.
    /// </summary>
    public DateTimeOffset ReceivedAt { get; }
}

public interface IWuzEventEnvelope<out TEvent> : IWuzEventEnvelope
    where TEvent : class
{
    /// <summary>
    /// Gets the non-nullable typed event data.
    /// If envelope exists, event was successfully parsed.
    /// </summary>
    public TEvent Payload { get; }
}
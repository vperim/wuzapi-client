using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.RabbitMq.Models;
using WuzApiClient.RabbitMq.Models.Events;
using WuzApiClient.RabbitMq.Models.Wuz;

namespace WuzApiClient.RabbitMq.Core.Interfaces;

/// <summary>
/// Handles a specific type of WhatsApp event.
/// </summary>
/// <typeparam name="TEvent">The event type to handle.</typeparam>
public interface IEventHandler<in TEvent>
    where TEvent : class, IWhatsAppEventEnvelope
{
    /// <summary>
    /// Handles the event asynchronously.
    /// </summary>
    /// <param name="envelope">The event envelope containing the typed event data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the async operation.</returns>
    Task HandleAsync(IWuzEventEnvelope<TEvent> envelope, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handles any WhatsApp event (non-generic fallback).
/// </summary>
public interface IEventHandler
{
    /// <summary>
    /// Gets the event types this handler processes.
    /// </summary>
    IReadOnlyCollection<string> EventTypes { get; }

    /// <summary>
    /// Handles the event asynchronously.
    /// </summary>
    /// <param name="envelope">The event envelope containing event data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the async operation.</returns>
    Task HandleAsync(IWuzEventEnvelope envelope, CancellationToken cancellationToken = default);
}

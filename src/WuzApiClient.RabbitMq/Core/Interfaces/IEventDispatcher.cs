using System;
using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.Results;

namespace WuzApiClient.RabbitMq.Core.Interfaces;

/// <summary>
/// Dispatches events to registered handlers.
/// </summary>
public interface IEventDispatcher
{
    /// <summary>
    /// Dispatches an event from raw message bytes to all matching handlers.
    /// Creates a new DI scope per message to properly support scoped handler lifetimes.
    /// </summary>
    /// <param name="body">The raw message bytes from RabbitMQ.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure with error details.</returns>
    Task<WuzResult> DispatchAsync(ReadOnlyMemory<byte> body, CancellationToken cancellationToken = default);
}

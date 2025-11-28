using System;
using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.RabbitMq.Models;

namespace WuzApiClient.RabbitMq.Core.Interfaces;

/// <summary>
/// Handles errors that occur during event processing.
/// </summary>
public interface IEventErrorHandler
{
    /// <summary>
    /// Handles an error that occurred during event processing.
    /// </summary>
    /// <param name="evt">The event that caused the error.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the async operation.</returns>
    Task HandleErrorAsync(
        WuzEvent evt,
        Exception exception,
        CancellationToken cancellationToken = default);
}

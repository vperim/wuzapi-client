using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.RabbitMq.Models;
using WuzApiClient.Results;

namespace WuzApiClient.RabbitMq.Core.Interfaces;

/// <summary>
/// Dispatches events to registered handlers.
/// </summary>
public interface IEventDispatcher
{
    /// <summary>
    /// Dispatches an event to all matching handlers.
    /// Creates a new DI scope per message to properly support scoped handler lifetimes.
    /// </summary>
    /// <param name="evt">The event to dispatch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure with error details.</returns>
    Task<WuzResult> DispatchAsync(WuzEvent evt, CancellationToken cancellationToken = default);
}

using System;
using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.RabbitMq.Models.Wuz;

namespace WuzApiClient.RabbitMq.Core.Interfaces;

/// <summary>
/// Handles deserialization and dispatch for a specific event type.
/// </summary>
/// <remarks>
/// This interface eliminates reflection from the dispatch path by providing
/// a typed contract for each event type. Implementations receive pre-extracted
/// metadata and handle deserialization to their specific event type.
/// </remarks>
public interface ITypedEventDispatcher
{
    /// <summary>
    /// Deserializes and dispatches an event to registered handlers.
    /// </summary>
    /// <param name="metadata">The extracted event metadata containing type, user info, and JSON elements.</param>
    /// <param name="serviceProvider">The scoped service provider for resolving handlers.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result indicating success or failure with error details.</returns>
    Task DispatchAsync(
        WuzEventMetadata metadata,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

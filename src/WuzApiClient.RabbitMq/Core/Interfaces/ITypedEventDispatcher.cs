using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.Results;

namespace WuzApiClient.RabbitMq.Core.Interfaces;

/// <summary>
/// Handles deserialization and dispatch for a specific event type.
/// </summary>
/// <remarks>
/// This interface eliminates reflection from the dispatch path by providing
/// a typed contract for each event type. Implementations receive raw JSON
/// elements and handle deserialization to their specific event type.
/// </remarks>
public interface ITypedEventDispatcher
{
    /// <summary>
    /// Deserializes and dispatches an event to registered handlers.
    /// </summary>
    /// <param name="eventElement">The JSON element containing the event-specific data.</param>
    /// <param name="rootElement">The complete JSON root element for metadata extraction.</param>
    /// <param name="type">The wuzapi event type string.</param>
    /// <param name="userId">The user ID associated with the event.</param>
    /// <param name="instanceName">The WhatsApp instance name.</param>
    /// <param name="serviceProvider">The scoped service provider for resolving handlers.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the dispatch operation with aggregated results.</returns>
    Task<WuzResult> DispatchAsync(
        JsonElement eventElement,
        JsonElement rootElement,
        string type,
        string userId,
        string instanceName,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

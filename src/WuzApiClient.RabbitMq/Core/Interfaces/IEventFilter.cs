using WuzApiClient.RabbitMq.Models;

namespace WuzApiClient.RabbitMq.Core.Interfaces;

/// <summary>
/// Filters events before dispatching to handlers.
/// </summary>
public interface IEventFilter
{
    /// <summary>
    /// Determines if the event should be processed.
    /// </summary>
    /// <param name="evt">The event to evaluate.</param>
    /// <returns>true if the event should be processed; otherwise, false.</returns>
    bool ShouldProcess(WuzEvent evt);

    /// <summary>
    /// Gets the filter priority (lower = earlier execution).
    /// </summary>
    int Order { get; }
}

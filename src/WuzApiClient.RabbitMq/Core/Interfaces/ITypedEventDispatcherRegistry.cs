namespace WuzApiClient.RabbitMq.Core.Interfaces;

/// <summary>
/// Registry for typed event dispatchers.
/// </summary>
/// <remarks>
/// This singleton registry maps wuzapi event type strings to their
/// corresponding typed dispatchers, enabling O(1) dispatch routing
/// without reflection. Unknown event types are handled by a fallback
/// dispatcher that creates UnknownEvent envelopes.
/// </remarks>
public interface ITypedEventDispatcherRegistry
{
    /// <summary>
    /// Gets the typed dispatcher for the specified event type.
    /// </summary>
    /// <param name="eventType">The wuzapi event type string.</param>
    /// <returns>
    /// The typed dispatcher for the event type, or a fallback dispatcher
    /// for unknown event types. Never returns null.
    /// </returns>
    ITypedEventDispatcher GetDispatcher(string eventType);
}

using Microsoft.Extensions.Options;
using WuzApiClient.RabbitMq.Configuration;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models;

namespace WuzApiClient.RabbitMq.Filters;

/// <summary>
/// Filters events based on configured event types.
/// Returns true if no event types are configured (no filter) or if the event type matches.
/// </summary>
public sealed class EventTypeFilter : IEventFilter
{
    private readonly WuzEventOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventTypeFilter"/> class.
    /// </summary>
    /// <param name="options">The event consumer options.</param>
    public EventTypeFilter(IOptions<WuzEventOptions> options)
    {
        this.options = options.Value;
    }

    /// <inheritdoc/>
    public int Order => 100;

    /// <inheritdoc/>
    public bool ShouldProcess(WuzEvent evt)
    {
        // No filter if no event types configured
        if (this.options.SubscribedEventTypes.Count == 0)
        {
            return true;
        }

        // Check if event type is in the subscribed set
        return this.options.SubscribedEventTypes.Contains(evt.Type);
    }
}

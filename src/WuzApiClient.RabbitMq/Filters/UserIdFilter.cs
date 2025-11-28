using Microsoft.Extensions.Options;
using WuzApiClient.RabbitMq.Configuration;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models;

namespace WuzApiClient.RabbitMq.Filters;

/// <summary>
/// Filters events based on configured user IDs.
/// Returns true if no user IDs are configured (no filter) or if the event user ID matches.
/// </summary>
public sealed class UserIdFilter : IEventFilter
{
    private readonly WuzEventOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserIdFilter"/> class.
    /// </summary>
    /// <param name="options">The event consumer options.</param>
    public UserIdFilter(IOptions<WuzEventOptions> options)
    {
        this.options = options.Value;
    }

    /// <inheritdoc/>
    public int Order => 101;

    /// <inheritdoc/>
    public bool ShouldProcess(WuzEvent evt)
    {
        // No filter if no user IDs configured
        if (this.options.FilterUserIds.Count == 0)
        {
            return true;
        }

        // Check if event user ID is in the filter set
        return this.options.FilterUserIds.Contains(evt.UserId);
    }
}

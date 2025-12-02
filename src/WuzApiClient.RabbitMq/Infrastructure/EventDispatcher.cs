using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WuzApiClient.RabbitMq.Configuration;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models;
using WuzApiClient.Results;

namespace WuzApiClient.RabbitMq.Infrastructure;

/// <summary>
/// Dispatches events to registered handlers with per-message DI scope support.
/// </summary>
public sealed class EventDispatcher : IEventDispatcher
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly ILogger<EventDispatcher> logger;
    private readonly ConcurrentDictionary<Type, EventHandlerOptionsAttribute?> optionsCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventDispatcher"/> class.
    /// </summary>
    /// <param name="scopeFactory">The service scope factory for creating per-message scopes.</param>
    /// <param name="logger">The logger instance.</param>
    public EventDispatcher(
        IServiceScopeFactory scopeFactory,
        ILogger<EventDispatcher> logger)
    {
        this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.optionsCache = new ConcurrentDictionary<Type, EventHandlerOptionsAttribute?>();
    }

    /// <inheritdoc/>
    public async Task<WuzResult> DispatchAsync(WuzEvent evt, CancellationToken cancellationToken = default)
    {
        if (evt == null)
        {
            throw new ArgumentNullException(nameof(evt));
        }

        this.logger.LogDebug(
            "Dispatching event {EventType} from user {UserId}/{InstanceName}",
            evt.Type,
            evt.UserId,
            evt.InstanceName);

        // Create a new DI scope for each message to properly support scoped handler/filter lifetimes
        using var scope = this.scopeFactory.CreateScope();

        try
        {
            // Apply filters
            var filters = scope.ServiceProvider.GetServices<IEventFilter>();
            var orderedFilters = filters.OrderBy(f => f.Order).ToList();

            foreach (var filter in orderedFilters)
            {
                if (!filter.ShouldProcess(evt))
                {
                    this.logger.LogInformation(
                        "Event {EventType} from {UserId}/{InstanceName} filtered out by {FilterType}",
                        evt.Type,
                        evt.UserId,
                        evt.InstanceName,
                        filter.GetType().Name);

                    return WuzResult.Success();
                }
            }

            // Resolve and invoke handlers
            var errors = new List<WuzApiError>();
            var totalHandlersInvoked = 0;

            // Resolve typed handlers for this specific event type
            var (typedHandlerErrors, typedHandlerCount) = await this.InvokeTypedHandlersAsync(
                evt,
                scope.ServiceProvider,
                cancellationToken).ConfigureAwait(false);

            errors.AddRange(typedHandlerErrors);
            totalHandlersInvoked += typedHandlerCount;

            // Resolve non-generic handlers that match this event type
            var (nonGenericHandlerErrors, nonGenericHandlerCount) = await this.InvokeNonGenericHandlersAsync(
                evt,
                scope.ServiceProvider,
                cancellationToken).ConfigureAwait(false);

            errors.AddRange(nonGenericHandlerErrors);
            totalHandlersInvoked += nonGenericHandlerCount;

            // Warn if no handlers processed this event
            if (totalHandlersInvoked == 0)
            {
                this.logger.LogWarning(
                    "No handlers registered for event type {EventType} from {UserId}/{InstanceName}",
                    evt.Type,
                    evt.UserId,
                    evt.InstanceName);
            }

            // Return aggregated result
            if (errors.Count > 0)
            {
                var aggregatedMessage = string.Join("; ", errors.Select(e => e.Message));
                this.logger.LogError(
                    "Event {EventType} processing completed with {ErrorCount} error(s): {Errors}",
                    evt.Type,
                    errors.Count,
                    aggregatedMessage);

                return WuzResult.Failure(new WuzApiError(
                    WuzApiErrorCode.Unknown,
                    $"Event processing failed with {errors.Count} error(s): {aggregatedMessage}"));
            }

            this.logger.LogDebug(
                "Event {EventType} dispatched successfully to {HandlerCount} handler(s)",
                evt.Type,
                totalHandlersInvoked);

            return WuzResult.Success();
        }
        catch (Exception ex)
        {
            this.logger.LogError(
                ex,
                "Unexpected error dispatching event {EventType}",
                evt.Type);

            return WuzResult.Failure(new WuzApiError(
                WuzApiErrorCode.Unknown,
                $"Unexpected error during event dispatch: {ex.Message}"));
        }
    }

    /// <summary>
    /// Invokes typed handlers for the event.
    /// </summary>
    /// <param name="evt">The event to dispatch.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tuple of errors encountered and number of handlers invoked.</returns>
    private async Task<(List<WuzApiError> Errors, int HandlerCount)> InvokeTypedHandlersAsync(
        WuzEvent evt,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var errors = new List<WuzApiError>();
        var handlerCount = 0;

        // Use reflection to resolve the correct typed handler interface
        var eventType = evt.GetType();
        var handlerInterfaceType = typeof(IEventHandler<>).MakeGenericType(eventType);

        // Resolve all handlers for this specific event type
        var handlers = serviceProvider.GetServices(handlerInterfaceType);

        foreach (var handler in handlers)
        {
            if (handler is null) continue;
            try
            {
                // Get handler options before invoking
                var handlerType = handler.GetType();
                this.GetHandlerOptions(handlerType);

                // Get the HandleAsync method from the handler
                var handleMethod = handlerInterfaceType.GetMethod("HandleAsync");
                if (handleMethod != null)
                {
                    this.logger.LogDebug(
                        "Invoking typed handler {HandlerType} for event {EventType}",
                        handlerType.Name,
                        evt.Type);

                    var task = (Task)handleMethod.Invoke(handler, [evt, cancellationToken]);
                    if (task != null)
                    {
                        await task.ConfigureAwait(false);
                    }

                    handlerCount++;
                }
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException ?? ex;
                this.logger.LogError(
                    innerException,
                    "Error in typed handler {HandlerType} for event {EventType}",
                    handler.GetType().Name,
                    evt.Type);

                errors.Add(new WuzApiError(
                    WuzApiErrorCode.Unknown,
                    $"Handler {handler.GetType().Name} failed: {innerException.Message}"));
            }
        }

        return (errors, handlerCount);
    }

    /// <summary>
    /// Invokes non-generic handlers that match the event type.
    /// </summary>
    /// <param name="evt">The event to dispatch.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tuple of errors encountered and number of handlers invoked.</returns>
    private async Task<(List<WuzApiError> Errors, int HandlerCount)> InvokeNonGenericHandlersAsync(
        WuzEvent evt,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var errors = new List<WuzApiError>();
        var handlerCount = 0;
        
        // Resolve all non-generic handlers
        var handlers = serviceProvider.GetServices<IEventHandler>();

        foreach (var handler in handlers)
        {
            try
            {
                // Check if this handler processes this event type (empty = catch-all)
                if (handler.EventTypes.Count == 0 || handler.EventTypes.Contains(evt.Type))
                {
                    // Get handler options before invoking
                    var handlerType = handler.GetType();
                    this.GetHandlerOptions(handlerType);

                    this.logger.LogDebug(
                        "Invoking non-generic handler {HandlerType} for event {EventType}",
                        handlerType.Name,
                        evt.Type);

                    await handler.HandleAsync(evt, cancellationToken).ConfigureAwait(false);
                    handlerCount++;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex,
                    "Error in non-generic handler {HandlerType} for event {EventType}",
                    handler.GetType().Name,
                    evt.Type);

                errors.Add(new WuzApiError(
                    WuzApiErrorCode.Unknown,
                    $"Handler {handler.GetType().Name} failed: {ex.Message}"));
            }
        }

        return (errors, handlerCount);
    }

    /// <summary>
    /// Gets the handler options for the specified handler type using cached reflection.
    /// </summary>
    /// <param name="handlerType">The handler type to query.</param>
    /// <returns>The event handler options attribute, or null if not present.</returns>
    private void GetHandlerOptions(Type handlerType)
    {
        this.optionsCache.GetOrAdd(
            handlerType,
            t => t.GetCustomAttribute<EventHandlerOptionsAttribute>());
    }
}

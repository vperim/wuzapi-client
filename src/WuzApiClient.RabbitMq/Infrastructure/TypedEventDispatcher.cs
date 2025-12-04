using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models;
using WuzApiClient.RabbitMq.Serialization;
using WuzApiClient.Results;

namespace WuzApiClient.RabbitMq.Infrastructure;

internal static class TypedEventDispatcherSerializationHelper
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}

/// <summary>
/// Generic typed event dispatcher that handles deserialization and handler invocation
/// for a specific event type without reflection.
/// </summary>
/// <typeparam name="TEvent">The event type this dispatcher handles.</typeparam>
public sealed class TypedEventDispatcher<TEvent> : ITypedEventDispatcher
    where TEvent : class
{
    /// <inheritdoc/>
    public async Task<WuzResult> DispatchAsync(
        JsonElement eventElement,
        JsonElement rootElement,
        string type,
        string userId,
        string instanceName,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<TypedEventDispatcher<TEvent>>>();

        try
        {
            // Step 1: Deserialize event data
            TEvent? eventData;

            if (typeof(TEvent) == typeof(UnknownEventData))
            {
                // Special handling for unknown events
                eventData = (TEvent)(object)new UnknownEventData
                {
                    Raw = rootElement.Clone(),
                    OriginalType = type,
                };
            }
            else
            {
                // Build merged JSON and deserialize using shared helper
                var mergedJson = JsonMergeHelper.BuildEventDataJson(eventElement, rootElement);
                eventData = JsonSerializer.Deserialize<TEvent>(mergedJson, TypedEventDispatcherSerializationHelper.JsonOptions);
            }

            if (eventData == null)
            {
                logger.LogError(
                    "Failed to deserialize event type '{EventType}' (null result)",
                    type);

                return WuzResult.Failure(new WuzApiError(
                    WuzApiErrorCode.Unknown,
                    $"Deserialization returned null for {type}"));
            }

            // Step 2: Create typed envelope
            var envelope = new WuzEventEnvelope<TEvent>
            {
                EventType = type,
                UserId = userId,
                InstanceName = instanceName,
                ReceivedAt = DateTimeOffset.UtcNow,
                Event = eventData,
                RawJson = JsonSerializer.Serialize(rootElement)
            };

            // Step 3: Resolve and invoke typed handlers (no reflection!)
            var errors = new List<WuzApiError>();
            var handlerCount = 0;

            // Typed handlers - direct resolution and invocation
            var typedHandlers = serviceProvider.GetServices<IEventHandler<TEvent>>();
            foreach (var handler in typedHandlers)
            {
                try
                {
                    logger.LogDebug(
                        "Invoking handler {HandlerType} for event {EventType}",
                        handler.GetType().Name,
                        type);

                    // Direct call - no reflection!
                    await handler.HandleAsync(envelope, cancellationToken).ConfigureAwait(false);
                    handlerCount++;
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Error in handler {HandlerType} for event {EventType}",
                        handler.GetType().Name,
                        type);

                    errors.Add(new WuzApiError(
                        WuzApiErrorCode.Unknown,
                        $"Handler {handler.GetType().Name} failed: {ex.Message}"));
                }
            }

            // Non-generic handlers (catch-all)
            var nonGenericHandlers = serviceProvider.GetServices<IEventHandler>();
            foreach (var handler in nonGenericHandlers)
            {
                try
                {
                    if (handler.EventTypes.Count == 0 || handler.EventTypes.Contains(type))
                    {
                        logger.LogDebug(
                            "Invoking catch-all handler {HandlerType} for event {EventType}",
                            handler.GetType().Name,
                            type);

                        await handler.HandleAsync(envelope, cancellationToken).ConfigureAwait(false);
                        handlerCount++;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Error in catch-all handler {HandlerType} for event {EventType}",
                        handler.GetType().Name,
                        type);

                    errors.Add(new WuzApiError(
                        WuzApiErrorCode.Unknown,
                        $"Handler {handler.GetType().Name} failed: {ex.Message}"));
                }
            }

            // Warn if no handlers
            if (handlerCount == 0)
            {
                logger.LogWarning(
                    "No handlers registered for event type {EventType}",
                    type);
            }

            // Return result
            if (errors.Count > 0)
            {
                var aggregatedMessage = string.Join("; ", errors.Select(e => e.Message));
                return WuzResult.Failure(new WuzApiError(
                    WuzApiErrorCode.Unknown,
                    $"Event processing failed with {errors.Count} error(s): {aggregatedMessage}"));
            }

            logger.LogDebug(
                "Event {EventType} dispatched to {HandlerCount} handler(s)",
                type,
                handlerCount);

            return WuzResult.Success();
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize event type '{EventType}'", type);
            return WuzResult.Failure(new WuzApiError(
                WuzApiErrorCode.Unknown,
                $"Deserialization failed: {ex.Message}"));
        }
    }
}

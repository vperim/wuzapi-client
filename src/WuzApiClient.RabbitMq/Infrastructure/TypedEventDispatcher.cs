using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models;
using WuzApiClient.RabbitMq.Models.Wuz;

namespace WuzApiClient.RabbitMq.Infrastructure;

/// <summary>
/// Generic typed event dispatcher that handles deserialization and handler invocation
/// for a specific event type without reflection.
/// </summary>
/// <typeparam name="TEvent">The event type this dispatcher handles.</typeparam>
public sealed class TypedEventDispatcher<TEvent> : ITypedEventDispatcher
    where TEvent : class, IWhatsAppEventEnvelope
{
    private readonly ILogger<TypedEventDispatcher<TEvent>> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedEventDispatcher{TEvent}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public TypedEventDispatcher(ILogger<TypedEventDispatcher<TEvent>> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task DispatchAsync(
        WuzEventMetadata metadata,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            // Step 1: Deserialize to typed envelope
            var envelope = metadata.ToEnvelope<TEvent>();

            // Typed handlers - direct resolution and invocation
            var typedHandlers = serviceProvider.GetServices<IEventHandler<TEvent>>();
            foreach (var handler in typedHandlers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    this.logger.LogDebug(
                        "Invoking handler {HandlerType} for event {EventType}",
                        handler.GetType().Name,
                        typeof(TEvent).Name);

                    // Direct call - no reflection!
                    await handler.HandleAsync(envelope, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(
                        ex,
                        "Error in handler {HandlerType} for event {EventType}",
                        handler.GetType().Name,
                        typeof(TEvent).Name);


                }
            }

            // TODO: Disabled for now, will re-design this later on.
            //// Non-generic handlers (catch-all)
            //var nonGenericHandlers = serviceProvider.GetServices<IEventHandler>();
            //foreach (var handler in nonGenericHandlers)
            //{
            //    cancellationToken.ThrowIfCancellationRequested();

            //    try
            //    {
            //        if (handler.EventTypes.Count == 0 || handler.EventTypes.Contains(metadata.Type))
            //        {
            //            this.logger.LogDebug(
            //                "Invoking catch-all handler {HandlerType} for event {EventType}",
            //                handler.GetType().Name,
            //                metadata.Type);

            //            await handler.HandleAsync(envelope, cancellationToken).ConfigureAwait(false);
            //            handlerCount++;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        this.logger.LogError(
            //            ex,
            //            "Error in catch-all handler {HandlerType} for event {EventType}",
            //            handler.GetType().Name,
            //            metadata.Type);

            //        errors.Add(new WuzApiError(
            //            WuzApiErrorCode.Unknown,
            //            $"Handler {handler.GetType().Name} failed: {ex.Message}"));
            //    }
            //}

        }
        catch (JsonException ex)
        {
            this.logger.LogError(
                ex,
                "JSON error while processing event type '{EventType}'. Raw JSON: {RawJson}",
                metadata.WaEventMetadata.Type,
                metadata.RawJson);
        }
    }
}

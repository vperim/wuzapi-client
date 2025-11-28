using System;
using Microsoft.Extensions.DependencyInjection;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models.Events;

namespace WuzApiClient.RabbitMq.Configuration;

/// <summary>
/// Fluent builder for configuring WuzEvents services.
/// </summary>
public sealed class WuzEventBuilder
{
    private readonly IServiceCollection services;

    /// <summary>
    /// Initializes a new instance of the <see cref="WuzEventBuilder"/> class.
    /// </summary>
    /// <param name="services">The service collection.</param>
    internal WuzEventBuilder(IServiceCollection services)
    {
        this.services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <summary>
    /// Adds a handler for message events.
    /// </summary>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <param name="lifetime">The service lifetime.</param>
    /// <returns>The builder for chaining.</returns>
    public WuzEventBuilder OnMessage<THandler>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where THandler : class, IEventHandler<MessageEvent>
    {
        this.services.AddEventHandler<MessageEvent, THandler>(lifetime);
        return this;
    }

    /// <summary>
    /// Adds a handler for presence events.
    /// </summary>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <param name="lifetime">The service lifetime.</param>
    /// <returns>The builder for chaining.</returns>
    public WuzEventBuilder OnPresence<THandler>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where THandler : class, IEventHandler<PresenceEvent>
    {
        this.services.AddEventHandler<PresenceEvent, THandler>(lifetime);
        return this;
    }

    /// <summary>
    /// Adds a handler for receipt events.
    /// </summary>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <param name="lifetime">The service lifetime.</param>
    /// <returns>The builder for chaining.</returns>
    public WuzEventBuilder OnReceipt<THandler>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where THandler : class, IEventHandler<ReceiptEvent>
    {
        this.services.AddEventHandler<ReceiptEvent, THandler>(lifetime);
        return this;
    }

    /// <summary>
    /// Adds a handler for connected events.
    /// </summary>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <param name="lifetime">The service lifetime.</param>
    /// <returns>The builder for chaining.</returns>
    public WuzEventBuilder OnConnected<THandler>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where THandler : class, IEventHandler<ConnectedEvent>
    {
        this.services.AddEventHandler<ConnectedEvent, THandler>(lifetime);
        return this;
    }

    /// <summary>
    /// Adds a handler for disconnected events.
    /// </summary>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <param name="lifetime">The service lifetime.</param>
    /// <returns>The builder for chaining.</returns>
    public WuzEventBuilder OnDisconnected<THandler>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where THandler : class, IEventHandler<DisconnectedEvent>
    {
        this.services.AddEventHandler<DisconnectedEvent, THandler>(lifetime);
        return this;
    }

    /// <summary>
    /// Adds a handler for call offer events.
    /// </summary>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <param name="lifetime">The service lifetime.</param>
    /// <returns>The builder for chaining.</returns>
    public WuzEventBuilder OnCallOffer<THandler>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where THandler : class, IEventHandler<CallOfferEvent>
    {
        this.services.AddEventHandler<CallOfferEvent, THandler>(lifetime);
        return this;
    }

    /// <summary>
    /// Adds a non-generic handler for any event type.
    /// </summary>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <param name="lifetime">The service lifetime.</param>
    /// <returns>The builder for chaining.</returns>
    public WuzEventBuilder OnAny<THandler>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where THandler : class, IEventHandler
    {
        this.services.AddEventHandler<THandler>(lifetime);
        return this;
    }

    /// <summary>
    /// Adds a custom event filter.
    /// </summary>
    /// <typeparam name="TFilter">The filter type.</typeparam>
    /// <param name="lifetime">The service lifetime.</param>
    /// <returns>The builder for chaining.</returns>
    public WuzEventBuilder WithFilter<TFilter>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TFilter : class, IEventFilter
    {
        this.services.AddEventFilter<TFilter>(lifetime);
        return this;
    }

    /// <summary>
    /// Configures the event consumer options.
    /// </summary>
    /// <param name="configure">Configuration action.</param>
    /// <returns>The builder for chaining.</returns>
    public WuzEventBuilder Configure(Action<WuzEventOptions> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        this.services.Configure(configure);
        return this;
    }
}

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Filters;
using WuzApiClient.RabbitMq.Infrastructure;
using WuzApiClient.RabbitMq.Models;

namespace WuzApiClient.RabbitMq.Configuration;

/// <summary>
/// Extension methods for configuring WuzEvents services in the dependency injection container.
/// </summary>
public static class WuzEventServiceCollectionExtensions
{
    /// <summary>
    /// Adds WuzApi event consumer services with inline configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration action for event options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWuzEvents(
        this IServiceCollection services,
        Action<WuzEventOptions> configure)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        services
            .AddOptions<WuzEventOptions>()
            .Configure(configure)
            .PostConfigure(options => options.Validate());

        return services.AddWuzEventsCore();
    }

    /// <summary>
    /// Adds WuzApi event consumer services with configuration section binding.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="sectionName">The configuration section name. Default: "WuzEvents".</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWuzEvents(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "WuzEvents")
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        services
            .AddOptions<WuzEventOptions>()
            .Bind(configuration.GetSection(sectionName))
            .PostConfigure(options => options.Validate());

        return services.AddWuzEventsCore();
    }

    /// <summary>
    /// Adds WuzEvents services using the fluent builder.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Builder configuration action.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWuzEvents(
        this IServiceCollection services,
        Action<WuzEventBuilder> configure)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        // Initialize options with defaults and validation
        services
            .AddOptions<WuzEventOptions>()
            .PostConfigure(options => options.Validate());

        // Create builder and let caller configure
        var builder = new WuzEventBuilder(services);
        configure(builder);

        // Add core services
        return services.AddWuzEventsCore();
    }

    /// <summary>
    /// Registers a typed event handler with the specified service lifetime.
    /// </summary>
    /// <typeparam name="TEvent">The event type to handle.</typeparam>
    /// <typeparam name="THandler">The handler implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The service lifetime. Default: Scoped.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEventHandler<TEvent, THandler>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TEvent : WuzEvent
        where THandler : class, IEventHandler<TEvent>
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.Add(new ServiceDescriptor(
            typeof(IEventHandler<TEvent>),
            typeof(THandler),
            lifetime));

        return services;
    }

    /// <summary>
    /// Registers a non-generic event handler with the specified service lifetime.
    /// </summary>
    /// <typeparam name="THandler">The handler implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The service lifetime. Default: Scoped.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEventHandler<THandler>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where THandler : class, IEventHandler
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.Add(new ServiceDescriptor(
            typeof(IEventHandler),
            typeof(THandler),
            lifetime));

        return services;
    }

    /// <summary>
    /// Registers an event filter with the specified service lifetime.
    /// </summary>
    /// <typeparam name="TFilter">The filter implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The service lifetime. Default: Scoped (per-message).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEventFilter<TFilter>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TFilter : class, IEventFilter
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.Add(new ServiceDescriptor(
            typeof(IEventFilter),
            typeof(TFilter),
            lifetime));

        return services;
    }

    /// <summary>
    /// Registers core WuzEvents infrastructure services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    private static IServiceCollection AddWuzEventsCore(this IServiceCollection services)
    {
        // Core infrastructure services - singletons
        services.TryAddSingleton<IRabbitMqConnection, RabbitMqConnection>();
        services.TryAddSingleton<IEventDispatcher, EventDispatcher>();
        services.TryAddSingleton<IEventErrorHandler, LoggingEventErrorHandler>();

        // Register EventConsumer as IEventConsumer singleton
        services.TryAddSingleton<IEventConsumer, EventConsumer>();

        // Make the hosted service use the same singleton instance
        // This ensures IEventConsumer can be injected elsewhere (e.g., to observe ConnectionStateChanged)
        services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService>(
            sp => (EventConsumer)sp.GetRequiredService<IEventConsumer>());

        // Default filters - scoped to support per-message resolution with scoped dependencies
        services.TryAddEnumerable(ServiceDescriptor.Scoped<IEventFilter, EventTypeFilter>());
        services.TryAddEnumerable(ServiceDescriptor.Scoped<IEventFilter, UserIdFilter>());
        services.TryAddEnumerable(ServiceDescriptor.Scoped<IEventFilter, InstanceNameFilter>());

        return services;
    }
}

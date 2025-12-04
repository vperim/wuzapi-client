using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Infrastructure;

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
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWuzEvents(
        this IServiceCollection services,
        IConfiguration configuration)
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
            .Bind(configuration.GetSection(WuzEventOptions.SectionName))
            .PostConfigure(options => options.Validate());

        return services.AddWuzEventsCore();
    }

    /// <summary>
    /// Adds WuzApi event consumer services with configuration section binding and handler registration via fluent builder.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="configureHandlers">Optional builder configuration action for registering event handlers.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWuzEvents(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<WuzEventBuilder>? configureHandlers)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        // Bind configuration
        services
            .AddOptions<WuzEventOptions>()
            .Bind(configuration.GetSection(WuzEventOptions.SectionName))
            .PostConfigure(options => options.Validate());

        // Add core services
        services.AddWuzEventsCore();

        // Register handlers via builder if provided
        if (configureHandlers != null)
        {
            var builder = new WuzEventBuilder(services);
            configureHandlers(builder);
        }

        return services;
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
    /// Registers core WuzEvents infrastructure services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    private static IServiceCollection AddWuzEventsCore(this IServiceCollection services)
    {
        // Core infrastructure services - singletons
        services.TryAddSingleton<IRabbitMqConnection, RabbitMqConnection>();
        services.TryAddSingleton<ITypedEventDispatcherRegistry, TypedEventDispatcherRegistry>();
        services.TryAddSingleton<IEventDispatcher, EventDispatcher>();

        // Register EventConsumer as IEventConsumer singleton
        services.TryAddSingleton<IEventConsumer, EventConsumer>();

        // Make the hosted service use the same singleton instance
        // This ensures IEventConsumer can be injected elsewhere (e.g., to observe ConnectionStateChanged)
        services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService>(
            sp => (EventConsumer)sp.GetRequiredService<IEventConsumer>());

        return services;
    }
}

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using WuzApiClient.Core.Implementations;
using WuzApiClient.Core.Interfaces;

namespace WuzApiClient.Configuration;

/// <summary>
/// Extension methods for configuring WuzAPI client factories in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds WuzAPI client factories to the service collection using the default configuration section.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration root.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>
    /// Registers <see cref="IWaClientFactory"/> and <see cref="IWuzApiAdminClientFactory"/>
    /// for creating client instances with dynamic tokens at runtime.
    /// </remarks>
    public static IServiceCollection AddWuzApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        return services.AddWuzApi(configuration.GetSection(WuzApiOptions.SectionName));
    }

    /// <summary>
    /// Adds WuzAPI client factories to the service collection using a specific configuration section.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configurationSection">The configuration section containing WuzAPI settings.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWuzApi(
        this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configurationSection == null) throw new ArgumentNullException(nameof(configurationSection));

        services.Configure<WuzApiOptions>(configurationSection);

        return services.AddWuzApiCore();
    }

    /// <summary>
    /// Adds WuzAPI client factories to the service collection using inline configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The configuration action for server settings.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWuzApi(
        this IServiceCollection services,
        Action<WuzApiOptions> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        services.Configure(configure);

        return services.AddWuzApiCore();
    }

    /// <summary>
    /// Adds WuzAPI client factories to the service collection with HttpClient builder customization.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The configuration action for server settings.</param>
    /// <param name="configureHttpClient">
    /// Optional action to configure the underlying <see cref="IHttpClientBuilder"/>.
    /// Use this to add Polly resilience policies, custom DelegatingHandlers, or adjust handler lifetime.
    /// </param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddWuzApi(
    ///     options => { options.BaseUrl = "http://localhost:8080/"; },
    ///     httpClientBuilder =>
    ///     {
    ///         httpClientBuilder
    ///             .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(300)))
    ///             .SetHandlerLifetime(TimeSpan.FromMinutes(5));
    ///     });
    /// </code>
    /// </example>
    public static IServiceCollection AddWuzApi(
        this IServiceCollection services,
        Action<WuzApiOptions> configure,
        Action<IHttpClientBuilder> configureHttpClient)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));
        if (configureHttpClient == null) throw new ArgumentNullException(nameof(configureHttpClient));

        services.Configure(configure);

        return services.AddWuzApiCore(configureHttpClient);
    }

    /// <summary>
    /// Adds WuzAPI client factories using configuration section with HttpClient builder customization.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration root.</param>
    /// <param name="configureHttpClient">
    /// Optional action to configure the underlying <see cref="IHttpClientBuilder"/>.
    /// </param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWuzApi(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IHttpClientBuilder> configureHttpClient)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        if (configureHttpClient == null) throw new ArgumentNullException(nameof(configureHttpClient));

        services.Configure<WuzApiOptions>(configuration.GetSection(WuzApiOptions.SectionName));

        return services.AddWuzApiCore(configureHttpClient);
    }

    private static IServiceCollection AddWuzApiCore(
        this IServiceCollection services,
        Action<IHttpClientBuilder>? configureHttpClient = null)
    {
        var builder = services.AddHttpClient(WaClientFactory.HttpClientName, (serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<WuzApiOptions>>().Value;
            options.Validate();

            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = options.Timeout;
        });

        configureHttpClient?.Invoke(builder);

        services.TryAddSingleton<IWaClientFactory, WaClientFactory>();
        services.TryAddSingleton<IWuzApiAdminClientFactory, WuzApiAdminClientFactory>();

        return services;
    }
}

using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WuzApiClient.Core.Interfaces;

using WuzApiClientImpl = WuzApiClient.Core.Implementations.WuzApiClient;
using WuzApiAdminClientImpl = WuzApiClient.Core.Implementations.WuzApiAdminClient;

namespace WuzApiClient.Configuration;

/// <summary>
/// Extension methods for configuring WuzAPI clients in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    private const string WuzApiClientName = "WuzApiClient";
    private const string WuzApiAdminClientName = "WuzApiAdminClient";

    /// <summary>
    /// Adds the WuzAPI client to the service collection using the default configuration section.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration root.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWuzApiClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        return services.AddWuzApiClient(configuration.GetSection(WuzApiOptions.SectionName));
    }

    /// <summary>
    /// Adds the WuzAPI client to the service collection using a specific configuration section.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configurationSection">The configuration section containing WuzAPI settings.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWuzApiClient(
        this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configurationSection == null) throw new ArgumentNullException(nameof(configurationSection));

        services.Configure<WuzApiOptions>(configurationSection);

        return services.AddWuzApiClientCore();
    }

    /// <summary>
    /// Adds the WuzAPI client to the service collection using inline configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWuzApiClient(
        this IServiceCollection services,
        Action<WuzApiOptions> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        services.Configure(configure);

        return services.AddWuzApiClientCore();
    }

    /// <summary>
    /// Adds the WuzAPI admin client to the service collection using the default configuration section.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration root.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWuzApiAdminClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        return services.AddWuzApiAdminClient(configuration.GetSection(WuzApiAdminOptions.SectionName));
    }

    /// <summary>
    /// Adds the WuzAPI admin client to the service collection using a specific configuration section.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configurationSection">The configuration section containing WuzAPI admin settings.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWuzApiAdminClient(
        this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configurationSection == null) throw new ArgumentNullException(nameof(configurationSection));

        services.Configure<WuzApiAdminOptions>(configurationSection);

        return services.AddWuzApiAdminClientCore();
    }

    /// <summary>
    /// Adds the WuzAPI admin client to the service collection using inline configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWuzApiAdminClient(
        this IServiceCollection services,
        Action<WuzApiAdminOptions> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        services.Configure(configure);

        return services.AddWuzApiAdminClientCore();
    }

    private static IServiceCollection AddWuzApiClientCore(this IServiceCollection services)
    {
        services.AddHttpClient(WuzApiClientName, (serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<WuzApiOptions>>().Value;
            options.Validate();

            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = options.Timeout;
        });

        services.AddTransient<IWuzApiClient>(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var options = serviceProvider.GetRequiredService<IOptions<WuzApiOptions>>();
            var httpClient = httpClientFactory.CreateClient(WuzApiClientName);

            return new WuzApiClientImpl(httpClient, options);
        });

        return services;
    }

    private static IServiceCollection AddWuzApiAdminClientCore(this IServiceCollection services)
    {
        services.AddHttpClient(WuzApiAdminClientName, (serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<WuzApiAdminOptions>>().Value;
            options.Validate();

            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = options.Timeout;
        });

        services.AddTransient<IWuzApiAdminClient>(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var options = serviceProvider.GetRequiredService<IOptions<WuzApiAdminOptions>>();
            var httpClient = httpClientFactory.CreateClient(WuzApiAdminClientName);

            return new WuzApiAdminClientImpl(httpClient, options);
        });

        return services;
    }
}

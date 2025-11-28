using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WuzApiClient.Configuration;
using WuzApiClient.Core.Interfaces;

namespace WuzApiClient.IntegrationTests.TestInfrastructure.Fixtures;

/// <summary>
/// Collection fixture for WuzAPI integration tests providing shared client instances.
/// </summary>
public sealed class WuzApiIntegrationFixture : IAsyncLifetime
{
    private ServiceProvider? serviceProvider;

    /// <summary>
    /// Gets the configuration root.
    /// </summary>
    public IConfiguration Configuration { get; private set; } = null!;

    /// <summary>
    /// Gets the WuzAPI client.
    /// </summary>
    public IWuzApiClient Client { get; private set; } = null!;

    /// <summary>
    /// Gets the WuzAPI admin client.
    /// </summary>
    public IWuzApiAdminClient AdminClient { get; private set; } = null!;

    /// <inheritdoc/>
    public Task InitializeAsync()
    {
        this.Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();

        services.AddSingleton(this.Configuration);
        services.AddWuzApiClient(this.Configuration);
        services.AddWuzApiAdminClient(this.Configuration);

        this.serviceProvider = services.BuildServiceProvider();

        this.Client = this.serviceProvider.GetRequiredService<IWuzApiClient>();
        this.AdminClient = this.serviceProvider.GetRequiredService<IWuzApiAdminClient>();

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task DisposeAsync()
    {
        if (this.serviceProvider is not null)
        {
            await this.serviceProvider.DisposeAsync();
        }
    }
}

/// <summary>
/// Collection definition for WuzAPI integration tests.
/// </summary>
[CollectionDefinition(Name)]
public sealed class WuzApiIntegrationCollection : ICollectionFixture<WuzApiIntegrationFixture>
{
    /// <summary>
    /// The collection name.
    /// </summary>
    public const string Name = "WuzApi Integration";
}

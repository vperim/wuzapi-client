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
    private WuzApiTestContainer? container;

    /// <summary>
    /// Gets the configuration root.
    /// </summary>
    public IConfiguration Configuration { get; private set; } = null!;

    /// <summary>
    /// Gets the WuzAPI client.
    /// </summary>
    public IWaClient Client { get; private set; } = null!;

    /// <summary>
    /// Gets the WuzAPI admin client.
    /// </summary>
    public IWuzApiAdminClient AdminClient { get; private set; } = null!;

    /// <summary>
    /// Gets the wuzapi test container.
    /// </summary>
    public WuzApiTestContainer Container => this.container ?? throw new InvalidOperationException("Container not initialized");

    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        // Load order: appsettings.json -> env vars -> appsettings.Local.json (highest priority)
        this.Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.Local.json", optional: true)
            .Build();

        // Initialize container
        this.container = new WuzApiTestContainer(this.Configuration);
        await this.container.InitializeAsync();

        // Configure services to use container URL
        var services = new ServiceCollection();

        services.AddSingleton(this.Configuration);

        // Override WuzApi configuration with container URL and token
        services.AddWuzApiClient(options =>
        {
            options.BaseUrl = this.container.BaseUrl;
            options.UserToken = this.container.UserToken;
            options.TimeoutSeconds = this.Configuration.GetValue<int>("WuzApi:TimeoutSeconds", 60);
        });

        // Override WuzApiAdmin configuration with container URL and token
        services.AddWuzApiAdminClient(options =>
        {
            options.BaseUrl = this.container.BaseUrl;
            options.AdminToken = this.container.AdminToken;
            options.TimeoutSeconds = this.Configuration.GetValue<int>("WuzApiAdmin:TimeoutSeconds", 60);
        });

        this.serviceProvider = services.BuildServiceProvider();

        this.Client = this.serviceProvider.GetRequiredService<IWaClient>();
        this.AdminClient = this.serviceProvider.GetRequiredService<IWuzApiAdminClient>();
    }

    /// <inheritdoc/>
    public async Task DisposeAsync()
    {
        if (this.serviceProvider is not null)
        {
            await this.serviceProvider.DisposeAsync();
        }

        if (this.container is not null)
        {
            await this.container.DisposeAsync();
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

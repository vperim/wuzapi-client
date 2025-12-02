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
    /// Gets the client factory for creating additional clients.
    /// </summary>
    public IWaClientFactory ClientFactory { get; private set; } = null!;

    /// <summary>
    /// Gets the admin client factory for creating additional admin clients.
    /// </summary>
    public IWuzApiAdminClientFactory AdminClientFactory { get; private set; } = null!;

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

        // Register WuzApi with container URL
        services.AddWuzApi(options =>
        {
            options.BaseUrl = this.container.BaseUrl;
            options.TimeoutSeconds = this.Configuration.GetValue<int>("WuzApi:TimeoutSeconds", 60);
        });

        this.serviceProvider = services.BuildServiceProvider();

        // Get factories
        this.ClientFactory = this.serviceProvider.GetRequiredService<IWaClientFactory>();
        this.AdminClientFactory = this.serviceProvider.GetRequiredService<IWuzApiAdminClientFactory>();

        // Create default clients using container tokens
        this.Client = this.ClientFactory.CreateClient(this.container.UserToken);
        this.AdminClient = this.AdminClientFactory.CreateClient(this.container.AdminToken);
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

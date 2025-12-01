using Microsoft.Extensions.Options;
using WuzApiClient.Configuration;
using WuzApiClient.Core.Implementations;
using WuzApiClient.UnitTests.TestInfrastructure.Mocks;

namespace WuzApiClient.UnitTests.TestInfrastructure.Fixtures;

/// <summary>
/// Base class for WaClient unit tests providing common setup and teardown.
/// </summary>
public abstract class WuzApiClientTestBase : IAsyncLifetime
{
    private const string TestBaseUrl = "http://localhost:8080/";
    private const string TestUserToken = "test-token";

    private HttpClient? httpClient;

    /// <summary>
    /// Gets the mock HTTP message handler for enqueuing responses and verifying requests.
    /// </summary>
    protected MockHttpMessageHandler MockHandler { get; private set; } = null!;

    /// <summary>
    /// Gets the system under test.
    /// </summary>
    protected WaClient Sut { get; private set; } = null!;

    /// <summary>
    /// Gets the options used to configure the client.
    /// </summary>
    protected IOptions<WuzApiOptions> Options { get; private set; } = null!;

    /// <inheritdoc/>
    public Task InitializeAsync()
    {
        this.MockHandler = new MockHttpMessageHandler();
        this.httpClient = new HttpClient(this.MockHandler)
        {
            BaseAddress = new Uri(TestBaseUrl)
        };

        this.Options = Microsoft.Extensions.Options.Options.Create(new WuzApiOptions
        {
            BaseUrl = TestBaseUrl,
            UserToken = TestUserToken,
            TimeoutSeconds = 30
        });

        this.Sut = new WaClient(this.httpClient, this.Options);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task DisposeAsync()
    {
        this.httpClient?.Dispose();
        this.MockHandler?.Dispose();

        return Task.CompletedTask;
    }
}

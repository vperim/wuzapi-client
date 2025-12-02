using WuzApiClient.Core.Implementations;
using WuzApiClient.UnitTests.TestInfrastructure.Mocks;

namespace WuzApiClient.UnitTests.TestInfrastructure.Fixtures;

/// <summary>
/// Base class for WaClient unit tests providing common setup and teardown.
/// </summary>
public abstract class WuzApiClientTestBase : IAsyncLifetime
{
    private const string TestBaseUrl = "http://localhost:8080/";

    /// <summary>
    /// The user token used for testing.
    /// </summary>
    protected const string TestUserToken = "test-token";

    private HttpClient? httpClient;

    /// <summary>
    /// Gets the mock HTTP message handler for enqueuing responses and verifying requests.
    /// </summary>
    protected MockHttpMessageHandler MockHandler { get; private set; } = null!;

    /// <summary>
    /// Gets the system under test.
    /// </summary>
    protected WaClient Sut { get; private set; } = null!;

    /// <inheritdoc/>
    public Task InitializeAsync()
    {
        this.MockHandler = new MockHttpMessageHandler();
        this.httpClient = new HttpClient(this.MockHandler)
        {
            BaseAddress = new Uri(TestBaseUrl)
        };

        this.Sut = new WaClient(this.httpClient, TestUserToken);

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

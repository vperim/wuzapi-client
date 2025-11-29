using System.Net.Http.Json;
using System.Text.Json;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Configuration;

namespace WuzApiClient.IntegrationTests.TestInfrastructure.Fixtures;

/// <summary>
/// Manages wuzapi container access for integration testing.
/// Prefers reusing an existing running container (set up via Setup-TestSession.ps1)
/// but can fall back to creating a new container via Testcontainers if needed.
/// </summary>
public sealed class WuzApiTestContainer : IAsyncLifetime
{
    private const int DefaultPort = 8080;

    private readonly IConfiguration configuration;
    private IContainer? managedContainer;
    private bool useExistingContainer;

    /// <summary>
    /// Gets the base URL of the wuzapi instance.
    /// </summary>
    public string BaseUrl { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the admin token for administrative API calls.
    /// </summary>
    public string AdminToken { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the user token for standard API calls.
    /// </summary>
    public string UserToken { get; private set; } = string.Empty;

    /// <summary>
    /// Gets whether the container is using an existing external instance.
    /// </summary>
    public bool IsUsingExistingContainer => this.useExistingContainer;

    /// <summary>
    /// Initializes a new instance of the <see cref="WuzApiTestContainer"/> class.
    /// </summary>
    /// <param name="configuration">Configuration containing container and API settings.</param>
    public WuzApiTestContainer(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        // First, try to use existing container from appsettings (WuzApi section)
        var existingBaseUrl = this.configuration["WuzApi:BaseUrl"]?.TrimEnd('/');
        var existingUserToken = this.configuration["WuzApi:UserToken"];
        var existingAdminToken = this.configuration["WuzApiAdmin:AdminToken"];

        if (!string.IsNullOrEmpty(existingBaseUrl) && !string.IsNullOrEmpty(existingUserToken))
        {
            if (await this.TryConnectToExistingContainerAsync(existingBaseUrl, existingUserToken, existingAdminToken))
            {
                this.useExistingContainer = true;
                this.BaseUrl = existingBaseUrl;
                this.UserToken = existingUserToken;
                this.AdminToken = existingAdminToken ?? string.Empty;
                return;
            }
        }

        // Fall back to creating a new container via Testcontainers
        await this.CreateManagedContainerAsync();
    }

    /// <inheritdoc/>
    public async Task DisposeAsync()
    {
        // Only dispose containers we created; don't touch external ones
        if (this.managedContainer is not null)
        {
            await this.managedContainer.DisposeAsync();
        }
    }

    /// <summary>
    /// Gets the container logs for debugging.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Container stdout and stderr logs, or empty if using external container.</returns>
    public async Task<(string Stdout, string Stderr)> GetContainerLogsAsync(CancellationToken cancellationToken = default)
    {
        if (this.managedContainer is not null)
        {
            return await this.managedContainer.GetLogsAsync(ct: cancellationToken);
        }

        return (string.Empty, string.Empty);
    }

    /// <summary>
    /// Checks if the WhatsApp session is authenticated and ready.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if session is connected and logged in; otherwise false.</returns>
    public async Task<bool> IsSessionAuthenticatedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = new HttpClient { BaseAddress = new Uri(this.BaseUrl) };
            client.DefaultRequestHeaders.Add("token", this.UserToken);

            var response = await client.GetAsync("/session/status", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var status = JsonSerializer.Deserialize<SessionStatusResponse>(json, JsonOptions);

            return status?.Data?.Connected == true && status?.Data?.LoggedIn == true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the current session state.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current session state.</returns>
    public async Task<SessionState> GetSessionStateAsync(CancellationToken cancellationToken = default)
    {
        if (await this.IsSessionAuthenticatedAsync(cancellationToken))
        {
            return SessionState.Ready;
        }

        return SessionState.NotConnected;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private async Task<bool> TryConnectToExistingContainerAsync(string baseUrl, string userToken, string? adminToken)
    {
        try
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(5)
            };

            // Check health endpoint first
            var healthResponse = await client.GetAsync("/health");
            if (!healthResponse.IsSuccessStatusCode)
            {
                return false;
            }

            // Verify session is authenticated
            client.DefaultRequestHeaders.Add("token", userToken);
            var statusResponse = await client.GetAsync("/session/status");

            if (!statusResponse.IsSuccessStatusCode)
            {
                return false;
            }

            var json = await statusResponse.Content.ReadAsStringAsync();
            var status = JsonSerializer.Deserialize<SessionStatusResponse>(json, JsonOptions);

            // Only use existing container if session is fully authenticated
            if (status?.Data?.Connected == true && status?.Data?.LoggedIn == true)
            {
                return true;
            }

            // Container exists but session not authenticated - still use it for tests
            // that don't require authentication (like testing error handling)
            return true;
        }
        catch
        {
            // Container not available
            return false;
        }
    }

    private async Task CreateManagedContainerAsync()
    {
        var imageTag = this.configuration["Container:ImageTag"] ?? "asternic/wuzapi:latest";
        var volumeName = this.configuration["Container:VolumeName"] ?? "wuzapi-integration-test-session";
        var adminToken = this.configuration["Container:AdminToken"] ?? "integration-test-admin-token";
        var userToken = this.configuration["Container:UserToken"] ?? "integration-test-user-token";
        var userName = this.configuration["Container:UserName"] ?? "IntegrationTestUser";

        this.managedContainer = new ContainerBuilder()
            .WithImage(imageTag)
            .WithPortBinding(DefaultPort, assignRandomHostPort: true)
            .WithVolumeMount(volumeName, "/app/dbdata")
            .WithEnvironment("WUZAPI_ADMIN_TOKEN", adminToken)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(r => r
                    .ForPath("/health")
                    .ForPort(DefaultPort)
                    .ForStatusCode(System.Net.HttpStatusCode.OK)))
            .Build();

        await this.managedContainer.StartAsync();

        var mappedPort = this.managedContainer.GetMappedPublicPort(DefaultPort);
        this.BaseUrl = $"http://localhost:{mappedPort}";
        this.AdminToken = adminToken;
        this.UserToken = userToken;

        // Ensure test user exists in managed container
        await this.EnsureUserExistsAsync(userName);
    }

    private async Task EnsureUserExistsAsync(string userName)
    {
        // Give the container a moment after health check to ensure admin API is ready
        await Task.Delay(TimeSpan.FromSeconds(2));

        using var client = new HttpClient { BaseAddress = new Uri(this.BaseUrl) };
        client.DefaultRequestHeaders.Add("Authorization", this.AdminToken);

        // Check if user exists
        var listResponse = await client.GetAsync("/admin/users");
        if (!listResponse.IsSuccessStatusCode)
        {
            var errorContent = await listResponse.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Failed to query admin users endpoint. Status: {listResponse.StatusCode}, Response: {errorContent}");
        }

        var users = await listResponse.Content.ReadFromJsonAsync<AdminUsersResponse>();
        if (users?.Users?.Any(u => u.Token == this.UserToken) == true)
        {
            return;
        }

        // Create user
        var createRequest = new { name = userName, token = this.UserToken };
        var createResponse = await client.PostAsJsonAsync("/admin/users", createRequest);

        // Handle case where user already exists (409 Conflict)
        if (createResponse.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            return;
        }

        if (!createResponse.IsSuccessStatusCode)
        {
            var errorContent = await createResponse.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Failed to create test user. Status: {createResponse.StatusCode}, Response: {errorContent}");
        }
    }

    /// <summary>
    /// Response wrapper from session status endpoint.
    /// </summary>
    private sealed class SessionStatusResponse
    {
        public SessionStatusData? Data { get; set; }
    }

    /// <summary>
    /// Session status data.
    /// </summary>
    private sealed class SessionStatusData
    {
        public bool Connected { get; set; }
        public bool LoggedIn { get; set; }
    }

    /// <summary>
    /// Response from admin users list endpoint.
    /// </summary>
    private sealed class AdminUsersResponse
    {
        public List<UserInfo>? Users { get; set; }
    }

    /// <summary>
    /// User information from admin endpoint.
    /// </summary>
    private sealed class UserInfo
    {
        public string? Token { get; set; }
    }
}

/// <summary>
/// Represents the state of the WhatsApp session.
/// </summary>
public enum SessionState
{
    /// <summary>
    /// Session is authenticated and ready for testing.
    /// </summary>
    Ready,

    /// <summary>
    /// Session requires QR code scanning.
    /// </summary>
    RequiresQrScan,

    /// <summary>
    /// Session is not connected.
    /// </summary>
    NotConnected,

    /// <summary>
    /// An error occurred checking session state.
    /// </summary>
    Error
}

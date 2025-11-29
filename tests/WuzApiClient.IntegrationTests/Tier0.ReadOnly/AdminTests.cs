using AwesomeAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WuzApiClient.Configuration;
using WuzApiClient.Core.Interfaces;
using WuzApiClient.IntegrationTests.TestInfrastructure.Fixtures;
using WuzApiClient.IntegrationTests.TestInfrastructure.Ordering;
using WuzApiClient.Results;

namespace WuzApiClient.IntegrationTests.Tier0.ReadOnly;

/// <summary>
/// Tier 0 (ReadOnly) tests for admin list operations.
/// These tests retrieve admin data without modifying any state.
/// </summary>
[Collection(WuzApiIntegrationCollection.Name)]
[Trait("Category", "Integration")]
[Trait("Tier", "0")]
public sealed class AdminTests
{
    private readonly WuzApiIntegrationFixture fixture;

    public AdminTests(WuzApiIntegrationFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    [TestTier(TestTiers.ReadOnly, order: 30)]
    [Trait("Category", "LiveApi")]
    public async Task ListUsers_ReturnsUserList()
    {
        // Act
        var result = await this.fixture.AdminClient.ListUsersAsync();

        // Assert
        result.IsSuccess.Should().BeTrue($"Expected success but got error: {(result.IsFailure ? result.Error.ToString() : "N/A")}");
        result.Value.Users.Should().NotBeNull();
    }

    [Fact]
    [TestTier(TestTiers.ReadOnly, order: 31)]
    [Trait("Category", "LiveApi")]
    public async Task ListUsers_InvalidAdminToken_ReturnsUnauthorizedError()
    {
        // Arrange - Create a client with invalid token
        var configValues = new Dictionary<string, string?>
        {
            ["WuzApi:BaseUrl"] = this.fixture.Configuration["WuzApi:BaseUrl"],
            ["WuzApiAdmin:BaseUrl"] = this.fixture.Configuration["WuzApiAdmin:BaseUrl"],
            ["WuzApiAdmin:AdminToken"] = "invalid-token-12345"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddWuzApiAdminClient(configuration);

        await using var serviceProvider = services.BuildServiceProvider();
        var invalidAdminClient = serviceProvider.GetRequiredService<IWuzApiAdminClient>();

        // Act
        var result = await invalidAdminClient.ListUsersAsync();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(WuzApiErrorCode.Unauthorized);
    }
}

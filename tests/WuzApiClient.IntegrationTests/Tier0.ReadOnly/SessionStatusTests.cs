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
/// Tier 0 (ReadOnly) tests for session status operations.
/// These tests are safe read-only operations that don't modify session state.
/// </summary>
[Collection(WuzApiIntegrationCollection.Name)]
[Trait("Category", "Integration")]
[Trait("Tier", "0")]
public sealed class SessionStatusTests
{
    private readonly WuzApiIntegrationFixture fixture;

    public SessionStatusTests(WuzApiIntegrationFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    [TestTier(TestTiers.ReadOnly, order: 0)]
    [Trait("Category", "LiveApi")]
    public async Task GetSessionStatus_ReturnsValidStatus()
    {
        // Act
        var result = await this.fixture.Client.GetSessionStatusAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    [TestTier(TestTiers.ReadOnly, order: 1)]
    [Trait("Category", "LiveApi")]
    public async Task GetSessionStatus_InvalidToken_ReturnsUnauthorizedError()
    {
        // Arrange
        var invalidConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["WuzApi:BaseUrl"] = this.fixture.Configuration["WuzApi:BaseUrl"],
                ["WuzApi:UserToken"] = "invalid-token-12345"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(invalidConfig);
        services.AddWuzApiClient(invalidConfig);

        await using var provider = services.BuildServiceProvider();
        var invalidClient = provider.GetRequiredService<IWaClient>();

        // Act
        var result = await invalidClient.GetSessionStatusAsync();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(WuzApiErrorCode.Unauthorized);
    }
}

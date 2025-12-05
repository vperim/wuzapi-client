using AwesomeAssertions;
using WuzApiClient.Common.Results;
using WuzApiClient.IntegrationTests.TestInfrastructure.Fixtures;
using WuzApiClient.IntegrationTests.TestInfrastructure.Ordering;

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
        // Arrange - Create a client with invalid token using the factory
        var invalidClient = this.fixture.ClientFactory.CreateClient("invalid-token-12345");

        // Act
        var result = await invalidClient.GetSessionStatusAsync();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(WuzApiErrorCode.Unauthorized);
    }
}

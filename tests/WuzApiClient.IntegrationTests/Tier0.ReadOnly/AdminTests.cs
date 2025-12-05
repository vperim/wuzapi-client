using AwesomeAssertions;
using WuzApiClient.Common.Results;
using WuzApiClient.IntegrationTests.TestInfrastructure.Fixtures;
using WuzApiClient.IntegrationTests.TestInfrastructure.Ordering;

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
        // Arrange - Create an admin client with invalid token using the factory
        var invalidAdminClient = this.fixture.AdminClientFactory.CreateClient("invalid-token-12345");

        // Act
        var result = await invalidAdminClient.ListUsersAsync();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(WuzApiErrorCode.Unauthorized);
    }
}

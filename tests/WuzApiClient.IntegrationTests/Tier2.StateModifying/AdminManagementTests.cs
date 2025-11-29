using AwesomeAssertions;
using WuzApiClient.IntegrationTests.TestInfrastructure.Fixtures;
using WuzApiClient.IntegrationTests.TestInfrastructure.Ordering;
using WuzApiClient.Models.Requests.Admin;
using WuzApiClient.Models.Responses.Admin;
using WuzApiClient.Results;

namespace WuzApiClient.IntegrationTests.Tier2.StateModifying;

/// <summary>
/// Tier 2 (StateModifying) tests for admin user management operations.
/// These tests create or delete users.
/// </summary>
[Collection(WuzApiIntegrationCollection.Name)]
[Trait("Category", "Integration")]
[Trait("Tier", "2")]
public sealed class AdminManagementTests
{
    private readonly WuzApiIntegrationFixture fixture;

    public AdminManagementTests(WuzApiIntegrationFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    [TestTier(TestTiers.StateModifying, order: 20)]
    [Trait("Category", "LiveApi")]
    public async Task CreateUser_ThenDelete_Succeeds()
    {
        // Arrange
        var testUserName = $"integration-test-{Guid.NewGuid():N}";
        var testUserToken = Guid.NewGuid().ToString("N");
        var request = new CreateUserRequest
        {
            Name = testUserName,
            Token = testUserToken
        };

        WuzResult<UserResponse> createResult = default;
        var userCreated = false;

        try
        {
            // Act - Create
            createResult = await this.fixture.AdminClient.CreateUserAsync(request);
            userCreated = createResult.IsSuccess;

            // Assert - Created
            createResult.IsSuccess.Should().BeTrue();
            createResult.Value.Name.Should().Be(testUserName);

            // Act - Delete
            var deleteResult = await this.fixture.AdminClient.DeleteUserAsync(createResult.Value.Id);

            // Assert - Deleted
            deleteResult.IsSuccess.Should().BeTrue();
            userCreated = false;
        }
        finally
        {
            // Cleanup on failure
            if (userCreated)
            {
                await this.fixture.AdminClient.DeleteUserAsync(createResult.Value.Id);
            }
        }
    }
}

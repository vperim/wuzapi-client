using AwesomeAssertions;
using WuzApiClient.IntegrationTests.TestInfrastructure.Fixtures;
using WuzApiClient.Models.Requests.Admin;
using WuzApiClient.Models.Responses.Admin;
using WuzApiClient.Results;

namespace WuzApiClient.IntegrationTests.Admin;

[Collection(WuzApiIntegrationCollection.Name)]
[Trait("Category", "Integration")]
public sealed class AdminIntegrationTests
{
    private readonly WuzApiIntegrationFixture fixture;

    public AdminIntegrationTests(WuzApiIntegrationFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
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

    [Fact]
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

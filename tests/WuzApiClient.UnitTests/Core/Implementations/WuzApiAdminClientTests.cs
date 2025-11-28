using System.Net;
using System.Text.Json;
using AwesomeAssertions;
using Microsoft.Extensions.Options;
using WuzApiClient.Configuration;
using WuzApiClient.Core.Implementations;
using WuzApiClient.Json;
using WuzApiClient.Models.Requests.Admin;
using WuzApiClient.Models.Responses.Admin;
using WuzApiClient.UnitTests.TestInfrastructure.Mocks;

namespace WuzApiClient.UnitTests.Core.Implementations;

/// <summary>
/// Unit tests for WuzApiAdminClient.
/// </summary>
[Trait("Category", "Unit")]
public sealed class WuzApiAdminClientTests : IAsyncLifetime
{
    private const string TestBaseUrl = "http://localhost:8080/";
    private const string TestAdminToken = "test-admin-token";

    private MockHttpMessageHandler mockHandler = null!;
    private HttpClient httpClient = null!;
    private IOptions<WuzApiAdminOptions> options = null!;
    private WuzApiAdminClient sut = null!;

    public Task InitializeAsync()
    {
        this.mockHandler = new MockHttpMessageHandler();
        this.httpClient = new HttpClient(this.mockHandler)
        {
            BaseAddress = new Uri(TestBaseUrl)
        };
        this.options = Options.Create(new WuzApiAdminOptions
        {
            BaseUrl = TestBaseUrl,
            AdminToken = TestAdminToken
        });
        this.sut = new WuzApiAdminClient(this.httpClient, this.options);

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        this.httpClient.Dispose();
        this.mockHandler.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ListUsersAsync_Success_ReturnsUserList()
    {
        // Arrange
        var expectedUsers = new UserResponse[]
        {
            new UserResponse { Id = "1", Name = "user1", Token = "token1" },
            new UserResponse { Id = "2", Name = "user2", Token = "token2" }
        };
        this.mockHandler.EnqueueSuccessResponse(expectedUsers);

        // Act
        var result = await this.sut.ListUsersAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Users.Should().HaveCount(2);
        result.Value.Users[0].Name.Should().Be("user1");
        result.Value.Users[1].Name.Should().Be("user2");
    }

    [Fact]
    public async Task ListUsersAsync_SendsAuthorizationHeader()
    {
        // Arrange
        this.mockHandler.EnqueueSuccessResponse(new UserListResponse { Users = [] });

        // Act
        await this.sut.ListUsersAsync();

        // Assert
        this.mockHandler.ReceivedRequests.Should().ContainSingle();
        var sentRequest = this.mockHandler.ReceivedRequests[0];
        sentRequest.Headers.GetValues("Authorization").Should().ContainSingle().Which.Should().Be(TestAdminToken);
    }

    [Fact]
    public async Task ListUsersAsync_SendsCorrectEndpoint()
    {
        // Arrange
        this.mockHandler.EnqueueSuccessResponse(new UserListResponse { Users = [] });

        // Act
        await this.sut.ListUsersAsync();

        // Assert
        this.mockHandler.ReceivedRequests.Should().ContainSingle();
        var sentRequest = this.mockHandler.ReceivedRequests[0];
        sentRequest.Method.Should().Be(HttpMethod.Get);
        sentRequest.RequestUri!.PathAndQuery.Should().Be("/admin/users");
    }

    [Fact]
    public async Task CreateUserAsync_Success_ReturnsCreatedUser()
    {
        // Arrange
        var expectedUser = new UserResponse { Id = "1", Name = "newuser", Token = "newtoken" };
        this.mockHandler.EnqueueSuccessResponse(expectedUser);
        var request = new CreateUserRequest { Name = "newuser", Token = "newtoken" };

        // Act
        var result = await this.sut.CreateUserAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be("1");
        result.Value.Name.Should().Be("newuser");
        result.Value.Token.Should().Be("newtoken");
    }

    [Fact]
    public async Task CreateUserAsync_SendsNameAndToken()
    {
        // Arrange
        this.mockHandler.EnqueueSuccessResponse(new UserResponse { Id = "1", Name = "testuser", Token = "testtoken" });
        var request = new CreateUserRequest { Name = "testuser", Token = "testtoken" };

        // Act
        await this.sut.CreateUserAsync(request);

        // Assert
        this.mockHandler.ReceivedRequests.Should().ContainSingle();
        this.mockHandler.ReceivedRequestContents.Should().ContainSingle();
        var sentContent = this.mockHandler.ReceivedRequestContents[0];
        sentContent.Should().NotBeNullOrEmpty();

        var payload = JsonSerializer.Deserialize<CreateUserRequest>(sentContent!, WuzApiJsonSerializerOptions.Default);
        payload.Should().NotBeNull();
        payload!.Name.Should().Be("testuser");
        payload.Token.Should().Be("testtoken");
    }

    [Fact]
    public async Task CreateUserAsync_HttpError_ReturnsFailure()
    {
        // Arrange
        this.mockHandler.EnqueueErrorResponse(HttpStatusCode.BadRequest, "User already exists");
        var request = new CreateUserRequest { Name = "existinguser", Token = "token" };

        // Act
        var result = await this.sut.CreateUserAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteUserAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.mockHandler.EnqueueSuccessResponse(new { });

        // Act
        var result = await this.sut.DeleteUserAsync("user123");

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteUserAsync_IncludesUserIdInPath()
    {
        // Arrange
        this.mockHandler.EnqueueSuccessResponse(new { });

        // Act
        await this.sut.DeleteUserAsync("user123");

        // Assert
        this.mockHandler.ReceivedRequests.Should().ContainSingle();
        var sentRequest = this.mockHandler.ReceivedRequests[0];
        sentRequest.RequestUri!.PathAndQuery.Should().Be("/admin/users/user123");
    }

    [Fact]
    public async Task DeleteUserAsync_SendsDeleteRequest()
    {
        // Arrange
        this.mockHandler.EnqueueSuccessResponse(new { });

        // Act
        await this.sut.DeleteUserAsync("user123");

        // Assert
        this.mockHandler.ReceivedRequests.Should().ContainSingle();
        var sentRequest = this.mockHandler.ReceivedRequests[0];
        sentRequest.Method.Should().Be(HttpMethod.Delete);
        sentRequest.Headers.GetValues("Authorization").Should().ContainSingle().Which.Should().Be(TestAdminToken);
    }
}

using System.Net;
using AwesomeAssertions;
using WuzApiClient.Core.Internal;
using WuzApiClient.Results;
using WuzApiClient.UnitTests.TestInfrastructure.Mocks;

namespace WuzApiClient.UnitTests.Core.Internal;

[Trait("Category", "Unit")]
public sealed class WuzApiHttpClientTests : IAsyncLifetime
{
    private const string AuthHeader = "X-Api-Key";
    private const string AuthValue = "test-token-123";
    private const string TestPath = "/api/test";

    private MockHttpMessageHandler mockHandler = null!;
    private HttpClient httpClient = null!;
    private WuzApiHttpClient sut = null!;

    public Task InitializeAsync()
    {
        this.mockHandler = new MockHttpMessageHandler();
        this.httpClient = new HttpClient(this.mockHandler)
        {
            BaseAddress = new Uri("http://localhost:8080/")
        };
        this.sut = new WuzApiHttpClient(this.httpClient);
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        this.httpClient.Dispose();
        this.mockHandler.Dispose();
        return Task.CompletedTask;
    }

    #region SendAsync<T> Success Tests

    [Fact]
    public async Task SendAsync_SuccessResponse_ReturnsSuccessResult()
    {
        // Arrange
        var expectedData = new TestResponse { Id = 42, Name = "Test" };
        this.mockHandler.EnqueueSuccessResponse(expectedData);

        // Act
        var result = await this.sut.SendAsync<TestResponse>(
            HttpMethod.Get,
            TestPath,
            AuthHeader,
            AuthValue,
            body: null,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(42);
        result.Value.Name.Should().Be("Test");
    }

    [Fact]
    public async Task SendAsync_SuccessResponse_IncludesAuthHeader()
    {
        // Arrange
        this.mockHandler.EnqueueSuccessResponse(new TestResponse { Id = 1, Name = "X" });

        // Act
        await this.sut.SendAsync<TestResponse>(
            HttpMethod.Get,
            TestPath,
            AuthHeader,
            AuthValue,
            body: null,
            CancellationToken.None);

        // Assert
        this.mockHandler.ReceivedRequests.Should().ContainSingle();
        var request = this.mockHandler.ReceivedRequests[0];
        request.Headers.Should().Contain(h => h.Key == AuthHeader);
        request.Headers.GetValues(AuthHeader).Should().Contain(AuthValue);
    }

    [Fact]
    public async Task SendAsync_WithBody_SerializesAsJson()
    {
        // Arrange
        this.mockHandler.EnqueueSuccessResponse(new TestResponse { Id = 1, Name = "X" });
        var requestBody = new TestRequest { Message = "Hello", Count = 5 };

        // Act
        await this.sut.SendAsync<TestResponse>(
            HttpMethod.Post,
            TestPath,
            AuthHeader,
            AuthValue,
            requestBody,
            CancellationToken.None);

        // Assert
        var content = this.mockHandler.ReceivedRequestContents[0];
        content.Should().NotBeNull();
        content.Should().Contain("\"message\"");
        content.Should().Contain("\"Hello\"");
        content.Should().Contain("\"count\"");
        content.Should().Contain("5");
    }

    #endregion

    #region SendAsync<T> HTTP Error Tests

    [Theory]
    [InlineData(400, WuzApiErrorCode.BadRequest)]
    [InlineData(401, WuzApiErrorCode.Unauthorized)]
    [InlineData(403, WuzApiErrorCode.Forbidden)]
    [InlineData(404, WuzApiErrorCode.NotFound)]
    [InlineData(409, WuzApiErrorCode.Conflict)]
    [InlineData(429, WuzApiErrorCode.RateLimitExceeded)]
    [InlineData(500, WuzApiErrorCode.InternalServerError)]
    [InlineData(503, WuzApiErrorCode.InternalServerError)]
    public async Task SendAsync_HttpError_MapsToCorrectErrorCode(int statusCode, WuzApiErrorCode expectedCode)
    {
        // Arrange
        this.mockHandler.EnqueueErrorResponse((HttpStatusCode)statusCode, "Test error");

        // Act
        var result = await this.sut.SendAsync<TestResponse>(
            HttpMethod.Get,
            TestPath,
            AuthHeader,
            AuthValue,
            body: null,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(expectedCode);
        result.Error.HttpStatusCode.Should().Be(statusCode);
    }

    #endregion

    #region SendAsync<T> Exception Handling Tests

    [Fact]
    public async Task SendAsync_Timeout_ReturnsTimeoutError()
    {
        // Arrange
        var timeoutHandler = new TimeoutHttpMessageHandler();
        using var timeoutClient = new HttpClient(timeoutHandler)
        {
            BaseAddress = new Uri("http://localhost:8080/")
        };
        var timeoutSut = new WuzApiHttpClient(timeoutClient);

        // Act
        var result = await timeoutSut.SendAsync<TestResponse>(
            HttpMethod.Get,
            TestPath,
            AuthHeader,
            AuthValue,
            body: null,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(WuzApiErrorCode.Timeout);
    }

    [Fact]
    public async Task SendAsync_NetworkError_ReturnsNetworkError()
    {
        // Arrange
        var networkErrorHandler = new NetworkErrorHttpMessageHandler("Connection refused");
        using var errorClient = new HttpClient(networkErrorHandler)
        {
            BaseAddress = new Uri("http://localhost:8080/")
        };
        var errorSut = new WuzApiHttpClient(errorClient);

        // Act
        var result = await errorSut.SendAsync<TestResponse>(
            HttpMethod.Get,
            TestPath,
            AuthHeader,
            AuthValue,
            body: null,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(WuzApiErrorCode.NetworkError);
        result.Error.Message.Should().Contain("Connection refused");
    }

    [Fact]
    public async Task SendAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        this.mockHandler.EnqueueSuccessResponse(new TestResponse { Id = 1, Name = "X" });

        // Act
        var act = () => this.sut.SendAsync<TestResponse>(
            HttpMethod.Get,
            TestPath,
            AuthHeader,
            AuthValue,
            body: null,
            cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region SendAsync (Non-Generic) Tests

    [Fact]
    public async Task SendAsyncVoid_Success_ReturnsSuccessResult()
    {
        // Arrange
        this.mockHandler.EnqueueResponse(HttpStatusCode.OK, "{}");

        // Act
        var result = await this.sut.SendAsync(
            HttpMethod.Post,
            TestPath,
            AuthHeader,
            AuthValue,
            body: null,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsyncVoid_HttpError_ReturnsFailureResult()
    {
        // Arrange
        this.mockHandler.EnqueueErrorResponse(HttpStatusCode.BadRequest, "Invalid request");

        // Act
        var result = await this.sut.SendAsync(
            HttpMethod.Post,
            TestPath,
            AuthHeader,
            AuthValue,
            body: null,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(WuzApiErrorCode.BadRequest);
    }

    #endregion

    #region Method Delegation Tests

    [Fact]
    public async Task GetAsync_SendsGetRequest()
    {
        // Arrange
        this.mockHandler.EnqueueSuccessResponse(new TestResponse { Id = 1, Name = "Get" });

        // Act
        await this.sut.GetAsync<TestResponse>(
            TestPath,
            AuthHeader,
            AuthValue,
            CancellationToken.None);

        // Assert
        var request = this.mockHandler.ReceivedRequests[0];
        request.Method.Should().Be(HttpMethod.Get);
    }

    [Fact]
    public async Task PostAsync_SendsPostRequest()
    {
        // Arrange
        this.mockHandler.EnqueueSuccessResponse(new TestResponse { Id = 1, Name = "Post" });

        // Act
        await this.sut.PostAsync<TestResponse>(
            TestPath,
            AuthHeader,
            AuthValue,
            body: new { data = "test" },
            CancellationToken.None);

        // Assert
        var request = this.mockHandler.ReceivedRequests[0];
        request.Method.Should().Be(HttpMethod.Post);
    }

    [Fact]
    public async Task DeleteAsync_SendsDeleteRequest()
    {
        // Arrange
        this.mockHandler.EnqueueResponse(HttpStatusCode.OK, "{}");

        // Act
        await this.sut.DeleteAsync(
            TestPath,
            AuthHeader,
            AuthValue,
            CancellationToken.None);

        // Assert
        var request = this.mockHandler.ReceivedRequests[0];
        request.Method.Should().Be(HttpMethod.Delete);
    }

    #endregion

    #region Test Helpers

    private sealed class TestResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private sealed class TestRequest
    {
        public string Message { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    private sealed class TimeoutHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            throw new TaskCanceledException("The request was canceled due to the configured HttpClient.Timeout");
        }
    }

    private sealed class NetworkErrorHttpMessageHandler : HttpMessageHandler
    {
        private readonly string message;

        public NetworkErrorHttpMessageHandler(string message)
        {
            this.message = message;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            throw new HttpRequestException(this.message);
        }
    }

    #endregion
}

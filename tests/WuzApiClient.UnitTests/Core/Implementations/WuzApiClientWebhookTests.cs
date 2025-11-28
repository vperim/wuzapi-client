using AwesomeAssertions;
using WuzApiClient.Models.Requests.Webhook;
using WuzApiClient.Models.Responses.Webhook;
using WuzApiClient.UnitTests.TestInfrastructure.Fixtures;

namespace WuzApiClient.UnitTests.Core.Implementations;

[Trait("Category", "Unit")]
public sealed class WuzApiClientWebhookTests : WuzApiClientTestBase
{
    [Fact]
    public async Task SetWebhookAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.MockHandler.EnqueueResponse(System.Net.HttpStatusCode.OK, "{}");
        var request = new SetWebhookRequest
        {
            Url = "https://example.com/webhook",
            Events = ["message", "status"]
        };

        // Act
        var result = await this.Sut.SetWebhookAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SetWebhookAsync_SendsUrlAndEvents()
    {
        // Arrange
        this.MockHandler.EnqueueResponse(System.Net.HttpStatusCode.OK, "{}");
        var request = new SetWebhookRequest
        {
            Url = "https://example.com/webhook",
            Events = ["message", "status"]
        };

        // Act
        await this.Sut.SetWebhookAsync(request);

        // Assert
        this.MockHandler.ReceivedRequests.Should().ContainSingle();
        var httpRequest = this.MockHandler.ReceivedRequests[0];
        httpRequest.Method.Should().Be(HttpMethod.Post);
        httpRequest.RequestUri!.PathAndQuery.Should().Be("/webhook");

        var content = this.MockHandler.ReceivedRequestContents[0];
        content.Should().Contain("\"url\"");
        content.Should().Contain("https://example.com/webhook");
        content.Should().Contain("\"events\"");
        content.Should().Contain("message");
        content.Should().Contain("status");
    }

    [Fact]
    public async Task GetWebhookAsync_Success_ReturnsConfig()
    {
        // Arrange
        var expectedResponse = new WebhookConfigResponse
        {
            Url = "https://example.com/webhook",
            Events = ["message", "status"]
        };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);

        // Act
        var result = await this.Sut.GetWebhookAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Url.Should().Be("https://example.com/webhook");
        result.Value.Events.Should().BeEquivalentTo(["message", "status"]);
    }

    [Fact]
    public async Task GetWebhookAsync_SendsCorrectEndpoint()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new WebhookConfigResponse());

        // Act
        await this.Sut.GetWebhookAsync();

        // Assert
        this.MockHandler.ReceivedRequests.Should().ContainSingle();
        var request = this.MockHandler.ReceivedRequests[0];
        request.Method.Should().Be(HttpMethod.Get);
        request.RequestUri!.PathAndQuery.Should().Be("/webhook");
    }

    [Fact]
    public async Task SetHmacKeyAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.MockHandler.EnqueueResponse(System.Net.HttpStatusCode.OK, "{}");
        var request = new SetHmacKeyRequest { Key = "secret-hmac-key" };

        // Act
        var result = await this.Sut.SetHmacKeyAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetHmacConfigAsync_Success_ReturnsConfig()
    {
        // Arrange
        var expectedResponse = new HmacConfigResponse
        {
            Enabled = true,
            Key = "***masked***"
        };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);

        // Act
        var result = await this.Sut.GetHmacConfigAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Enabled.Should().BeTrue();
        result.Value.Key.Should().Be("***masked***");
    }

    [Fact]
    public async Task RemoveHmacConfigAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.MockHandler.EnqueueResponse(System.Net.HttpStatusCode.OK, "{}");

        // Act
        var result = await this.Sut.RemoveHmacConfigAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveHmacConfigAsync_SendsDeleteRequest()
    {
        // Arrange
        this.MockHandler.EnqueueResponse(System.Net.HttpStatusCode.OK, "{}");

        // Act
        await this.Sut.RemoveHmacConfigAsync();

        // Assert
        this.MockHandler.ReceivedRequests.Should().ContainSingle();
        var request = this.MockHandler.ReceivedRequests[0];
        request.Method.Should().Be(HttpMethod.Delete);
        request.RequestUri!.PathAndQuery.Should().Be("/session/hmac/config");
    }
}

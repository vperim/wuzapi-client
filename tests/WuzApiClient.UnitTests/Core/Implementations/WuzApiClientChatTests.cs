using System.Net;
using System.Text.Json;
using AwesomeAssertions;
using WuzApiClient.Models.Common;
using WuzApiClient.Models.Requests.Chat;
using WuzApiClient.UnitTests.TestInfrastructure.Fixtures;

namespace WuzApiClient.UnitTests.Core.Implementations;

[Trait("Category", "Unit")]
public sealed class WuzApiClientChatTests : WuzApiClientTestBase
{
    private static readonly Phone TestPhone = Phone.Create("5511999999999");

    #region MarkAsReadAsync Tests

    [Fact]
    public async Task MarkAsReadAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.MockHandler.EnqueueResponse(HttpStatusCode.OK, "{}");
        var request = new MarkAsReadRequest
        {
            ChatPhone = TestPhone,
            MessageIds = ["msg-123"]
        };

        // Act
        var result = await this.Sut.MarkAsReadAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task MarkAsReadAsync_SendsCorrectEndpoint()
    {
        // Arrange
        this.MockHandler.EnqueueResponse(HttpStatusCode.OK, "{}");
        var request = new MarkAsReadRequest
        {
            ChatPhone = TestPhone,
            MessageIds = ["msg-123"]
        };

        // Act
        await this.Sut.MarkAsReadAsync(request);

        // Assert
        this.MockHandler.ReceivedRequests.Should().ContainSingle();
        var httpRequest = this.MockHandler.ReceivedRequests[0];
        httpRequest.Method.Should().Be(HttpMethod.Post);
        httpRequest.RequestUri!.PathAndQuery.Should().Be("/chat/markread");
    }

    [Fact]
    public async Task MarkAsReadAsync_SendsCorrectPayload()
    {
        // Arrange
        this.MockHandler.EnqueueResponse(HttpStatusCode.OK, "{}");
        var request = new MarkAsReadRequest
        {
            ChatPhone = TestPhone,
            MessageIds = ["msg-123", "msg-456"]
        };

        // Act
        await this.Sut.MarkAsReadAsync(request);

        // Assert
        var content = this.MockHandler.ReceivedRequestContents[0];
        content.Should().NotBeNull();

        using var doc = JsonDocument.Parse(content!);
        var root = doc.RootElement;
        root.GetProperty("ChatPhone").GetString().Should().Be("5511999999999");
        root.GetProperty("Id").GetArrayLength().Should().Be(2);
    }

    #endregion

    #region ReactToMessageAsync Tests

    [Fact]
    public async Task ReactToMessageAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.MockHandler.EnqueueResponse(HttpStatusCode.OK, "{}");
        var request = new ReactRequest
        {
            Phone = TestPhone,
            MessageId = "msg-123",
            Emoji = "thumbs_up"
        };

        // Act
        var result = await this.Sut.ReactToMessageAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ReactToMessageAsync_SendsCorrectPayload()
    {
        // Arrange
        this.MockHandler.EnqueueResponse(HttpStatusCode.OK, "{}");
        var request = new ReactRequest
        {
            Phone = TestPhone,
            MessageId = "msg-789",
            Emoji = "heart"
        };

        // Act
        await this.Sut.ReactToMessageAsync(request);

        // Assert
        this.MockHandler.ReceivedRequests.Should().ContainSingle();
        var httpRequest = this.MockHandler.ReceivedRequests[0];
        httpRequest.Method.Should().Be(HttpMethod.Post);
        httpRequest.RequestUri!.PathAndQuery.Should().Be("/chat/react");

        var content = this.MockHandler.ReceivedRequestContents[0];
        content.Should().NotBeNull();

        using var doc = JsonDocument.Parse(content!);
        var root = doc.RootElement;
        root.GetProperty("Id").GetString().Should().Be("msg-789");
        root.GetProperty("Body").GetString().Should().Be("heart");
    }

    #endregion

    #region SetPresenceAsync Tests

    [Fact]
    public async Task SetPresenceAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.MockHandler.EnqueueResponse(HttpStatusCode.OK, "{}");
        var request = new SetPresenceRequest
        {
            Phone = TestPhone,
            State = "composing"
        };

        // Act
        var result = await this.Sut.SetPresenceAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("composing", "composing")]
    [InlineData("recording", "recording")]
    [InlineData("paused", "paused")]
    public async Task SetPresenceAsync_VariousStates_SendsCorrectState(
        string state,
        string expectedStateValue)
    {
        // Arrange
        this.MockHandler.EnqueueResponse(HttpStatusCode.OK, "{}");
        var request = new SetPresenceRequest
        {
            Phone = TestPhone,
            State = state
        };

        // Act
        await this.Sut.SetPresenceAsync(request);

        // Assert
        this.MockHandler.ReceivedRequests.Should().ContainSingle();
        var httpRequest = this.MockHandler.ReceivedRequests[0];
        httpRequest.Method.Should().Be(HttpMethod.Post);
        httpRequest.RequestUri!.PathAndQuery.Should().Be("/chat/presence");

        var content = this.MockHandler.ReceivedRequestContents[0];
        content.Should().NotBeNull();
        content.Should().Contain($"\"{expectedStateValue}\"");
    }

    #endregion
}

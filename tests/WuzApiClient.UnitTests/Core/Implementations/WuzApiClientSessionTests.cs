using System.Net;
using AwesomeAssertions;
using WuzApiClient.Common.Models;
using WuzApiClient.Models.Requests.Session;
using WuzApiClient.UnitTests.TestInfrastructure.Builders;
using WuzApiClient.UnitTests.TestInfrastructure.Fixtures;

namespace WuzApiClient.UnitTests.Core.Implementations;

/// <summary>
/// Unit tests for WaClient session management methods.
/// </summary>
[Trait("Category", "Unit")]
public sealed class WuzApiClientSessionTests : WuzApiClientTestBase
{
    [Fact]
    public async Task ConnectSessionAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new { });
        var request = new ConnectSessionRequest { Immediate = true };

        // Act
        var result = await this.Sut.ConnectSessionAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ConnectSessionAsync_SendsCorrectEndpointAndToken()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new { });
        var request = new ConnectSessionRequest { Immediate = true };

        // Act
        await this.Sut.ConnectSessionAsync(request);

        // Assert
        this.MockHandler.ReceivedRequests.Should().ContainSingle();
        var sentRequest = this.MockHandler.ReceivedRequests[0];
        sentRequest.Method.Should().Be(HttpMethod.Post);
        sentRequest.RequestUri!.PathAndQuery.Should().Be("/session/connect");
        sentRequest.Headers.GetValues("Token").Should().ContainSingle().Which.Should().Be("test-token");
    }

    [Fact]
    public async Task ConnectSessionAsync_HttpError_ReturnsFailure()
    {
        // Arrange
        this.MockHandler.EnqueueErrorResponse(HttpStatusCode.InternalServerError, "Connection failed");
        var request = new ConnectSessionRequest { Immediate = true };

        // Act
        var result = await this.Sut.ConnectSessionAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task DisconnectSessionAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new { });

        // Act
        var result = await this.Sut.DisconnectSessionAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DisconnectSessionAsync_SendsCorrectEndpoint()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new { });

        // Act
        await this.Sut.DisconnectSessionAsync();

        // Assert
        this.MockHandler.ReceivedRequests.Should().ContainSingle();
        var sentRequest = this.MockHandler.ReceivedRequests[0];
        sentRequest.Method.Should().Be(HttpMethod.Post);
        sentRequest.RequestUri!.PathAndQuery.Should().Be("/session/disconnect");
    }

    [Fact]
    public async Task LogoutSessionAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new { });

        // Act
        var result = await this.Sut.LogoutSessionAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetSessionStatusAsync_Success_ReturnsStatus()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(ResponseBuilder.SessionStatus());

        // Act
        var result = await this.Sut.GetSessionStatusAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task GetSessionStatusAsync_ParsesResponseCorrectly()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(ResponseBuilder.SessionStatus(
            connected: true,
            loggedIn: true,
            jid: "5511999999999@s.whatsapp.net"));

        // Act
        var result = await this.Sut.GetSessionStatusAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Connected.Should().BeTrue();
        result.Value.LoggedIn.Should().BeTrue();
        result.Value.Jid?.Value.Should().Be("5511999999999@s.whatsapp.net");
    }

    [Fact]
    public async Task GetQrCodeAsync_Success_ReturnsQrCode()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(ResponseBuilder.QrCode("data:image/png;base64,test"));

        // Act
        var result = await this.Sut.GetQrCodeAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.QrCode.Should().Be("data:image/png;base64,test");
    }

    [Fact]
    public async Task PairPhoneAsync_Success_ReturnsPairingCode()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new { code = "1234-5678" });
        var request = new PairPhoneRequest { Phone = Phone.Create("5511999999999") };

        // Act
        var result = await this.Sut.PairPhoneAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("1234-5678");
    }

    [Fact]
    public async Task SetProxyAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new { });
        var request = new SetProxyRequest { Proxy = "http://proxy.example.com:8080" };

        // Act
        var result = await this.Sut.SetProxyAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}

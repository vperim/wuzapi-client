using AwesomeAssertions;
using WuzApiClient.Models.Responses.Download;
using WuzApiClient.UnitTests.TestInfrastructure.Fixtures;

namespace WuzApiClient.UnitTests.Core.Implementations;

[Trait("Category", "Unit")]
public sealed class WuzApiClientMediaTests : WuzApiClientTestBase
{
    private const string TestMessageId = "msg-123";

    [Fact]
    public async Task DownloadImageAsync_Success_ReturnsMediaData()
    {
        // Arrange
        var expectedResponse = new MediaDownloadResponse
        {
            Data = "base64imagedata",
            MimeType = "image/png",
            FileName = "image.png"
        };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);

        // Act
        var result = await this.Sut.DownloadImageAsync(TestMessageId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Data.Should().Be("base64imagedata");
        result.Value.MimeType.Should().Be("image/png");
        result.Value.FileName.Should().Be("image.png");
    }

    [Fact]
    public async Task DownloadImageAsync_IncludesMessageIdInRequest()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new MediaDownloadResponse { Data = "x" });

        // Act
        await this.Sut.DownloadImageAsync(TestMessageId);

        // Assert
        this.MockHandler.ReceivedRequests.Should().ContainSingle();
        var request = this.MockHandler.ReceivedRequests[0];
        request.RequestUri!.ToString().Should().Contain($"id={TestMessageId}");
        request.RequestUri.PathAndQuery.Should().StartWith("/download/image");
    }

    [Fact]
    public async Task DownloadVideoAsync_Success_ReturnsMediaData()
    {
        // Arrange
        var expectedResponse = new MediaDownloadResponse
        {
            Data = "base64videodata",
            MimeType = "video/mp4",
            FileName = "video.mp4"
        };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);

        // Act
        var result = await this.Sut.DownloadVideoAsync(TestMessageId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Data.Should().Be("base64videodata");
        result.Value.MimeType.Should().Be("video/mp4");
    }

    [Fact]
    public async Task DownloadAudioAsync_Success_ReturnsMediaData()
    {
        // Arrange
        var expectedResponse = new MediaDownloadResponse
        {
            Data = "base64audiodata",
            MimeType = "audio/ogg",
            FileName = "audio.ogg"
        };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);

        // Act
        var result = await this.Sut.DownloadAudioAsync(TestMessageId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Data.Should().Be("base64audiodata");
        result.Value.MimeType.Should().Be("audio/ogg");
    }

    [Fact]
    public async Task DownloadDocumentAsync_Success_ReturnsMediaData()
    {
        // Arrange
        var expectedResponse = new MediaDownloadResponse
        {
            Data = "base64docdata",
            MimeType = "application/pdf",
            FileName = "document.pdf"
        };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);

        // Act
        var result = await this.Sut.DownloadDocumentAsync(TestMessageId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Data.Should().Be("base64docdata");
        result.Value.MimeType.Should().Be("application/pdf");
    }
}

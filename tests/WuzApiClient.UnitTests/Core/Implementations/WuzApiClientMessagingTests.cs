using System.Net;
using AwesomeAssertions;
using WuzApiClient.Common.Results;
using WuzApiClient.Models.Common;
using WuzApiClient.Models.Requests.Chat;
using WuzApiClient.Models.Responses.Chat;
using WuzApiClient.UnitTests.TestInfrastructure.Fixtures;

namespace WuzApiClient.UnitTests.Core.Implementations;

[Trait("Category", "Unit")]
public sealed class WuzApiClientMessagingTests : WuzApiClientTestBase
{
    private static readonly Phone TestPhone = Phone.Create("5511999999999");

    #region SendTextMessageAsync Tests

    [Fact]
    public async Task SendTextMessageAsync_Success_ReturnsSendMessageResponse()
    {
        // Arrange
        var expectedResponse = new SendMessageResponse
        {
            Id = "msg-123",
            Timestamp = 1700000000,
            Details = "sent"
        };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);

        // Act
        var result = await this.Sut.SendTextMessageAsync(TestPhone, "Hello World");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be("msg-123");
        result.Value.Timestamp.Should().Be(1700000000);
    }

    [Fact]
    public async Task SendTextMessageAsync_SendsCorrectPayload()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new SendMessageResponse { Id = "msg-1" });

        // Act
        await this.Sut.SendTextMessageAsync(TestPhone, "Test message", quotedId: "quote-123");

        // Assert
        this.MockHandler.ReceivedRequests.Should().ContainSingle();
        var content = this.MockHandler.ReceivedRequestContents[0];
        content.Should().NotBeNull();
        content.Should().Contain("\"Phone\"");
        content.Should().Contain("5511999999999");
        content.Should().Contain("\"Body\"");
        content.Should().Contain("Test message");
        content.Should().Contain("\"Id\"");
        content.Should().Contain("quote-123");
    }

    [Fact]
    public async Task SendTextMessageAsync_HttpError_ReturnsFailure()
    {
        // Arrange
        this.MockHandler.EnqueueErrorResponse(HttpStatusCode.BadRequest, "Invalid phone number");

        // Act
        var result = await this.Sut.SendTextMessageAsync(TestPhone, "Hello");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(WuzApiErrorCode.BadRequest);
    }

    #endregion

    #region SendImageAsync Tests

    [Fact]
    public async Task SendImageAsync_Success_ReturnsSendMessageResponse()
    {
        // Arrange
        var expectedResponse = new SendMessageResponse { Id = "img-123", Timestamp = 1700000001 };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);
        var request = new SendImageRequest
        {
            Phone = TestPhone,
            Image = "base64imagedata",
            Caption = "Test image"
        };

        // Act
        var result = await this.Sut.SendImageAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be("img-123");
    }

    #endregion

    #region SendDocumentAsync Tests

    [Fact]
    public async Task SendDocumentAsync_Success_ReturnsSendMessageResponse()
    {
        // Arrange
        var expectedResponse = new SendMessageResponse { Id = "doc-123", Timestamp = 1700000002 };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);
        var request = new SendDocumentRequest
        {
            Phone = TestPhone,
            Document = "base64docdata",
            FileName = "test.pdf"
        };

        // Act
        var result = await this.Sut.SendDocumentAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be("doc-123");
    }

    #endregion

    #region SendAudioAsync Tests

    [Fact]
    public async Task SendAudioAsync_Success_ReturnsSendMessageResponse()
    {
        // Arrange
        var expectedResponse = new SendMessageResponse { Id = "audio-123", Timestamp = 1700000003 };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);
        var request = new SendAudioRequest
        {
            Phone = TestPhone,
            Audio = "base64audiodata"
        };

        // Act
        var result = await this.Sut.SendAudioAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be("audio-123");
    }

    #endregion

    #region SendVideoAsync Tests

    [Fact]
    public async Task SendVideoAsync_Success_ReturnsSendMessageResponse()
    {
        // Arrange
        var expectedResponse = new SendMessageResponse { Id = "video-123", Timestamp = 1700000004 };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);
        var request = new SendVideoRequest
        {
            Phone = TestPhone,
            Video = "base64videodata",
            Caption = "Test video"
        };

        // Act
        var result = await this.Sut.SendVideoAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be("video-123");
    }

    #endregion

    #region SendStickerAsync Tests

    [Fact]
    public async Task SendStickerAsync_Success_ReturnsSendMessageResponse()
    {
        // Arrange
        var expectedResponse = new SendMessageResponse { Id = "sticker-123", Timestamp = 1700000005 };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);
        var request = new SendStickerRequest
        {
            Phone = TestPhone,
            Sticker = "base64stickerdata"
        };

        // Act
        var result = await this.Sut.SendStickerAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be("sticker-123");
    }

    #endregion

    #region SendLocationAsync Tests

    [Fact]
    public async Task SendLocationAsync_Success_ReturnsSendMessageResponse()
    {
        // Arrange
        var expectedResponse = new SendMessageResponse { Id = "loc-123", Timestamp = 1700000006 };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);
        var request = new SendLocationRequest
        {
            Phone = TestPhone,
            Latitude = -23.5505,
            Longitude = -46.6333,
            Name = "Sao Paulo"
        };

        // Act
        var result = await this.Sut.SendLocationAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be("loc-123");
    }

    #endregion

    #region SendContactAsync Tests

    [Fact]
    public async Task SendContactAsync_Success_ReturnsSendMessageResponse()
    {
        // Arrange
        var expectedResponse = new SendMessageResponse { Id = "contact-123", Timestamp = 1700000007 };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);
        var request = new SendContactRequest
        {
            Phone = TestPhone,
            VCard = "BEGIN:VCARD\nVERSION:3.0\nFN:Test\nEND:VCARD"
        };

        // Act
        var result = await this.Sut.SendContactAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be("contact-123");
    }

    #endregion

    #region EditMessageAsync Tests

    [Fact]
    public async Task EditMessageAsync_Success_ReturnsSendMessageResponse()
    {
        // Arrange
        var expectedResponse = new SendMessageResponse { Id = "edit-123", Timestamp = 1700000008 };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);
        var request = new EditMessageRequest
        {
            Phone = TestPhone,
            MessageId = "original-msg-123",
            NewBody = "Edited message"
        };

        // Act
        var result = await this.Sut.EditMessageAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be("edit-123");
    }

    #endregion

    #region DeleteMessageAsync Tests

    [Fact]
    public async Task DeleteMessageAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.MockHandler.EnqueueResponse(HttpStatusCode.OK, "{}");
        var request = new DeleteMessageRequest
        {
            Phone = TestPhone,
            MessageId = "msg-to-delete"
        };

        // Act
        var result = await this.Sut.DeleteMessageAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    #endregion
}

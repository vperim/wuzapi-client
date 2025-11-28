using AwesomeAssertions;
using WuzApiClient.IntegrationTests.TestInfrastructure.Configuration;
using WuzApiClient.IntegrationTests.TestInfrastructure.Fixtures;
using WuzApiClient.Models.Common;
using WuzApiClient.Models.Requests.Chat;

namespace WuzApiClient.IntegrationTests.Messaging;

[Collection(WuzApiIntegrationCollection.Name)]
[Trait("Category", "Integration")]
public sealed class MessagingIntegrationTests
{
    private readonly WuzApiIntegrationFixture fixture;

    public MessagingIntegrationTests(WuzApiIntegrationFixture fixture)
    {
        this.fixture = fixture;
        TestConfiguration.Configuration = fixture.Configuration;
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    public async Task SendTextMessage_ValidRecipient_ReturnsMessageId()
    {
        // Arrange
        await this.EnsureSessionReadyAsync();
        var phone = Phone.Create(TestConfiguration.TestPhoneNumber);

        // Act
        var result = await this.fixture.Client.SendTextMessageAsync(phone, "Integration test message");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeNullOrEmpty();
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    public async Task SendImage_ValidUrl_ReturnsMessageId()
    {
        // Arrange
        await this.EnsureSessionReadyAsync();
        var phone = Phone.Create(TestConfiguration.TestPhoneNumber);
        var request = new SendImageRequest
        {
            Phone = phone,
            Image = CreateTestImageBase64(),
            Caption = "Integration test image",
            MimeType = "image/png"
        };

        // Act
        var result = await this.fixture.Client.SendImageAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeNullOrEmpty();
    }

    private async Task EnsureSessionReadyAsync()
    {
        var status = await this.fixture.Client.GetSessionStatusAsync();

        if (!status.IsSuccess || !status.Value.Connected || !status.Value.LoggedIn)
        {
            Assert.Fail("Session not ready: requires an active, logged-in WhatsApp session. " +
                        "This test is skipped when no live session is available.");
        }
    }

    private static string CreateTestImageBase64()
    {
        // Minimal 1x1 red PNG (67 bytes)
        var pngBytes = new byte[]
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D,
            0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
            0x08, 0x02, 0x00, 0x00, 0x00, 0x90, 0x77, 0x53, 0xDE, 0x00, 0x00, 0x00,
            0x0C, 0x49, 0x44, 0x41, 0x54, 0x08, 0xD7, 0x63, 0xF8, 0xCF, 0xC0, 0x00,
            0x00, 0x00, 0x03, 0x00, 0x01, 0x00, 0x05, 0xFE, 0xD4, 0xEF, 0x00, 0x00,
            0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
        };
        return Convert.ToBase64String(pngBytes);
    }
}

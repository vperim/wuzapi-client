using AwesomeAssertions;
using WuzApiClient.IntegrationTests.TestInfrastructure.Configuration;
using WuzApiClient.IntegrationTests.TestInfrastructure.Fixtures;
using WuzApiClient.IntegrationTests.TestInfrastructure.Ordering;
using WuzApiClient.IntegrationTests.TestInfrastructure.RateLimiting;
using WuzApiClient.Models.Common;
using WuzApiClient.Models.Requests.Chat;
using Xunit.Abstractions;

namespace WuzApiClient.IntegrationTests.Tier1.Messaging;

/// <summary>
/// Tier 1 (Messaging) tests for messaging operations.
/// These tests send messages and are rate-limited to prevent WhatsApp throttling.
/// </summary>
[Collection(WuzApiIntegrationCollection.Name)]
[Trait("Category", "Integration")]
[Trait("Tier", "1")]
public sealed class MessagingTests : ThrottledTestBase
{
    private readonly WuzApiIntegrationFixture fixture;

    protected override int TestTier => TestTiers.Messaging;

    public MessagingTests(WuzApiIntegrationFixture fixture, ITestOutputHelper output)
        : base(output)
    {
        this.fixture = fixture;
        TestConfiguration.Configuration = fixture.Configuration;
    }

    [Fact]
    [TestTier(TestTiers.Messaging, order: 0)]
    [Trait("Category", "LiveApi")]
    public async Task SendTextMessage_ValidRecipient_ReturnsMessageId()
    {
        // Arrange
        await this.EnsureSessionReadyAsync();
        var phone = Phone.Create(TestConfiguration.TestPhoneNumber);

        // Act - use throttled execution
        var result = await this.ThrottledExecuteAsync(async () => await this.fixture.Client.SendTextMessageAsync(phone, "Integration test message"));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeNullOrEmpty();
    }

    [Fact]
    [TestTier(TestTiers.Messaging, order: 1)]
    [Trait("Category", "LiveApi")]
    public async Task SendImage_ValidUrl_ReturnsMessageId()
    {
        // Arrange
        await this.EnsureSessionReadyAsync();
        var phone = Phone.Create(TestConfiguration.TestPhoneNumber);
        var request = new SendImageRequest
        {
            Phone = phone,
            Image = CreateTestImageDataUrl(),
            Caption = "Integration test image"
        };

        // Act - use throttled execution
        var result = await this.ThrottledExecuteAsync(async () => await this.fixture.Client.SendImageAsync(request));

        // Assert
        result.IsSuccess.Should().BeTrue(result.IsSuccess ? string.Empty : $"Expected success but got error: {result.Error}");
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

    [Fact]
    [TestTier(TestTiers.Messaging, order: 2)]
    [Trait("Category", "LiveApi")]
    public async Task SendDocument_ValidPdf_ReturnsMessageId()
    {
        // Arrange
        await this.EnsureSessionReadyAsync();
        var phone = Phone.Create(TestConfiguration.TestPhoneNumber);
        var request = new SendDocumentRequest
        {
            Phone = phone,
            Document = CreateTestPdfDataUrl(),
            FileName = "test-document.pdf",
            MimeType = "application/pdf",
            Caption = "Integration test PDF document"
        };

        // Act - use throttled execution
        var result = await this.ThrottledExecuteAsync(async () => await this.fixture.Client.SendDocumentAsync(request));

        // Assert
        result.IsSuccess.Should().BeTrue(result.IsSuccess ? string.Empty : $"Expected success but got error: {result.Error}");
        result.Value.Id.Should().NotBeNullOrEmpty();
    }

    private static string CreateTestImageDataUrl()
    {
        // Valid 1x1 transparent PNG - well-known minimal valid PNG
        // This is the smallest valid PNG file possible (68 bytes)
        const string validPngBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
        return $"data:image/png;base64,{validPngBase64}";
    }

    private static string CreateTestPdfDataUrl()
    {
        // Minimal valid PDF (displays blank page)
        const string minimalPdf = "%PDF-1.0\n1 0 obj<</Pages 2 0 R>>endobj 2 0 obj<</Kids[3 0 R]/Count 1>>endobj 3 0 obj<</MediaBox[0 0 612 792]>>endobj\ntrailer<</Root 1 0 R>>";
        var base64 = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(minimalPdf));
        return $"data:application/octet-stream;base64,{base64}";
    }
}

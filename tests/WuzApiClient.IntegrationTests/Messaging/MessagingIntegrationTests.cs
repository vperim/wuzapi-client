using AwesomeAssertions;
using WuzApiClient.Common.Results;
using WuzApiClient.IntegrationTests.TestInfrastructure.Configuration;
using WuzApiClient.IntegrationTests.TestInfrastructure.Fixtures;
using WuzApiClient.Common.Models;
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
            Image = CreateTestImageDataUrl(),
            Caption = "Integration test image"
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

    [Fact]
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

        // Act
        var result = await this.fixture.Client.SendDocumentAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue(result.IsSuccess ? string.Empty : $"Expected success but got error: {result.Error}");
        result.Value.Id.Should().NotBeNullOrEmpty();
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    public async Task SendImageFromFileAsync_ValidImageFile_ReturnsMessageId()
    {
        // Arrange
        await this.EnsureSessionReadyAsync();
        var phone = Phone.Create(TestConfiguration.TestPhoneNumber);
        string? tempFilePath = null;

        try
        {
            tempFilePath = CreateTempImageFile();

            // Act
            var result = await this.fixture.Client.SendImageFromFileAsync(
                phone,
                tempFilePath,
                caption: "Integration test image from file");

            // Assert
            result.IsSuccess.Should().BeTrue(result.IsSuccess ? string.Empty : $"Expected success but got error: {result.Error}");
            result.Value.Id.Should().NotBeNullOrEmpty();
        }
        finally
        {
            if (tempFilePath != null && File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    public async Task SendImageFromStreamAsync_ValidImageStream_ReturnsMessageId()
    {
        // Arrange
        await this.EnsureSessionReadyAsync();
        var phone = Phone.Create(TestConfiguration.TestPhoneNumber);
        using var stream = new MemoryStream(CreateTestImageBytes());

        // Act
        var result = await this.fixture.Client.SendImageFromStreamAsync(
            phone,
            stream,
            caption: "Integration test image from stream");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeNullOrEmpty();
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    public async Task SendImageFromFileAsync_NonExistentFile_ReturnsFailureWithInvalidRequest()
    {
        // Arrange
        await this.EnsureSessionReadyAsync();
        var phone = Phone.Create(TestConfiguration.TestPhoneNumber);
        var nonExistentPath = "non-existent-file.png";

        // Act
        var result = await this.fixture.Client.SendImageFromFileAsync(phone, nonExistentPath);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(WuzApiErrorCode.InvalidRequest);
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    public async Task SendImageFromFileAsync_FileTooLarge_ReturnsFailureWithInvalidRequest()
    {
        // Arrange
        await this.EnsureSessionReadyAsync();
        var phone = Phone.Create(TestConfiguration.TestPhoneNumber);
        string? tempFilePath = null;

        try
        {
            // Create a 17MB file (exceeds 16MB limit)
            tempFilePath = Path.GetTempFileName();
            var largeData = new byte[17 * 1024 * 1024];
            await File.WriteAllBytesAsync(tempFilePath, largeData);

            // Act
            var result = await this.fixture.Client.SendImageFromFileAsync(phone, tempFilePath);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be(WuzApiErrorCode.InvalidRequest);
        }
        finally
        {
            if (tempFilePath != null && File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    public async Task SendDocumentFromFileAsync_ValidDocumentFile_ReturnsMessageId()
    {
        // Arrange
        await this.EnsureSessionReadyAsync();
        var phone = Phone.Create(TestConfiguration.TestPhoneNumber);
        string? tempFilePath = null;

        try
        {
            tempFilePath = CreateTempPdfFile();

            // Act
            var result = await this.fixture.Client.SendDocumentFromFileAsync(
                phone,
                tempFilePath,
                caption: "Integration test document from file");

            // Assert
            result.IsSuccess.Should().BeTrue(result.IsSuccess ? string.Empty : $"Expected success but got error: {result.Error}");
            result.Value.Id.Should().NotBeNullOrEmpty();
        }
        finally
        {
            if (tempFilePath != null && File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    public async Task SendDocumentFromStreamAsync_ValidDocumentStream_ReturnsMessageId()
    {
        // Arrange
        await this.EnsureSessionReadyAsync();
        var phone = Phone.Create(TestConfiguration.TestPhoneNumber);
        using var stream = new MemoryStream(CreateTestPdfBytes());

        // Act
        var result = await this.fixture.Client.SendDocumentFromStreamAsync(
            phone,
            stream,
            fileName: "test.pdf",
            caption: "Integration test document from stream");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeNullOrEmpty();
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    public async Task SendDocumentFromFileAsync_NonExistentFile_ReturnsFailureWithInvalidRequest()
    {
        // Arrange
        await this.EnsureSessionReadyAsync();
        var phone = Phone.Create(TestConfiguration.TestPhoneNumber);
        var nonExistentPath = "non-existent-document.pdf";

        // Act
        var result = await this.fixture.Client.SendDocumentFromFileAsync(phone, nonExistentPath);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(WuzApiErrorCode.InvalidRequest);
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    public async Task SendDocumentFromFileAsync_FileTooLarge_ReturnsFailureWithInvalidRequest()
    {
        // Arrange
        await this.EnsureSessionReadyAsync();
        var phone = Phone.Create(TestConfiguration.TestPhoneNumber);
        string? tempFilePath = null;

        try
        {
            // Create a 101MB file (exceeds 100MB limit)
            tempFilePath = Path.GetTempFileName();
            var largeData = new byte[101 * 1024 * 1024];
            await File.WriteAllBytesAsync(tempFilePath, largeData);

            // Act
            var result = await this.fixture.Client.SendDocumentFromFileAsync(phone, tempFilePath);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be(WuzApiErrorCode.InvalidRequest);
        }
        finally
        {
            if (tempFilePath != null && File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    public async Task SendImageFromStreamAsync_WithNullStream_ReturnsFailureWithInvalidRequest()
    {
        // Arrange
        await this.EnsureSessionReadyAsync();
        var phone = Phone.Create(TestConfiguration.TestPhoneNumber);

        // Act
        var result = await this.fixture.Client.SendImageFromStreamAsync(phone, null!);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(WuzApiErrorCode.InvalidRequest);
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    public async Task SendDocumentFromStreamAsync_WithNullFileName_ReturnsFailureWithInvalidRequest()
    {
        // Arrange
        await this.EnsureSessionReadyAsync();
        var phone = Phone.Create(TestConfiguration.TestPhoneNumber);
        using var stream = new MemoryStream(CreateTestPdfBytes());

        // Act
        var result = await this.fixture.Client.SendDocumentFromStreamAsync(
            phone,
            stream,
            fileName: null!);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(WuzApiErrorCode.InvalidRequest);
    }

    private static string CreateTestImageDataUrl()
    {
        // Valid 1x1 transparent PNG - well-known minimal valid PNG
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

    private static string CreateTempImageFile()
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), $"wuzapi-test-{Guid.NewGuid()}.png");
        File.WriteAllBytes(tempFilePath, CreateTestImageBytes());
        return tempFilePath;
    }

    private static string CreateTempPdfFile()
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), $"wuzapi-test-{Guid.NewGuid()}.pdf");
        File.WriteAllBytes(tempFilePath, CreateTestPdfBytes());
        return tempFilePath;
    }

    private static byte[] CreateTestImageBytes()
    {
        const string validPngBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
        return Convert.FromBase64String(validPngBase64);
    }

    private static byte[] CreateTestPdfBytes()
    {
        const string minimalPdf = "%PDF-1.0\n1 0 obj<</Pages 2 0 R>>endobj 2 0 obj<</Kids[3 0 R]/Count 1>>endobj 3 0 obj<</MediaBox[0 0 612 792]>>endobj\ntrailer<</Root 1 0 R>>";
        return System.Text.Encoding.ASCII.GetBytes(minimalPdf);
    }
}

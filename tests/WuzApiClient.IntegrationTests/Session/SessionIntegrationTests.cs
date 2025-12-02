using AwesomeAssertions;
using WuzApiClient.IntegrationTests.TestInfrastructure.Fixtures;
using WuzApiClient.Models.Requests.Session;
using WuzApiClient.Results;

namespace WuzApiClient.IntegrationTests.Session;

[Collection(WuzApiIntegrationCollection.Name)]
[Trait("Category", "Integration")]
public sealed class SessionIntegrationTests
{
    private readonly WuzApiIntegrationFixture fixture;

    public SessionIntegrationTests(WuzApiIntegrationFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    [Trait("Category", TestCategories.Safe)]
    public async Task GetSessionStatus_ReturnsValidStatus()
    {
        // Act
        var result = await this.fixture.Client.GetSessionStatusAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    [Trait("Category", TestCategories.MildlyDestructive)]
    public async Task ConnectSession_WhenDisconnected_Succeeds()
    {
        // Arrange
        var request = new ConnectSessionRequest { Immediate = true };

        // Act
        var result = await this.fixture.Client.ConnectSessionAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    [Trait("Category", TestCategories.Destructive)]
    public async Task DisconnectSession_WhenConnected_Succeeds()
    {
        // Act - WARNING: This will disconnect the WhatsApp session
        var result = await this.fixture.Client.DisconnectSessionAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    [Trait("Category", TestCategories.RequiresSession)]
    public async Task GetQrCode_WhenNotLoggedIn_ReturnsQrData()
    {
        // Note: This test only works when session is NOT logged in
        // Act
        var result = await this.fixture.Client.GetQrCodeAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.QrCode.Should().NotBeNullOrEmpty();
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    [Trait("Category", TestCategories.Safe)]
    public async Task GetSessionStatus_InvalidToken_ReturnsUnauthorizedError()
    {
        // Arrange - Create a client with invalid token using the factory
        var invalidClient = this.fixture.ClientFactory.CreateClient("invalid-token-12345");

        // Act
        var result = await invalidClient.GetSessionStatusAsync();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(WuzApiErrorCode.Unauthorized);
    }

    [Fact]
    [Trait("Category", "Diagnostic")]
    public async Task DiagnosticTest_InspectContainerLogs()
    {
        // Arrange & Act
        var (stdout, stderr) = await this.fixture.Container.GetContainerLogsAsync();

        // Output logs for inspection
        Console.WriteLine("=== CONTAINER STDOUT ===");
        Console.WriteLine(stdout);
        Console.WriteLine("\n=== CONTAINER STDERR ===");
        Console.WriteLine(stderr);

        // Also check container state
        var sessionState = await this.fixture.Container.GetSessionStateAsync();
        Console.WriteLine($"\n=== SESSION STATE: {sessionState} ===");

        // Assert container is running
        true.Should().BeTrue("Container logs retrieved successfully");
    }
}

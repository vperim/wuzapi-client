using AwesomeAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WuzApiClient.Configuration;
using WuzApiClient.Core.Interfaces;
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
        // Arrange
        var invalidConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["WuzApi:BaseUrl"] = this.fixture.Configuration["WuzApi:BaseUrl"],
                ["WuzApi:UserToken"] = "invalid-token-12345"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(invalidConfig);
        services.AddWuzApiClient(invalidConfig);

        await using var provider = services.BuildServiceProvider();
        var invalidClient = provider.GetRequiredService<IWuzApiClient>();

        // Act
        var result = await invalidClient.GetSessionStatusAsync();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(WuzApiErrorCode.Unauthorized);
    }
}

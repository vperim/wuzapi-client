using AwesomeAssertions;
using WuzApiClient.IntegrationTests.TestInfrastructure.Fixtures;
using WuzApiClient.IntegrationTests.TestInfrastructure.Ordering;

namespace WuzApiClient.IntegrationTests.Tier3.Destructive;

/// <summary>
/// Tier 3 (Destructive) tests for session disconnect operations.
/// These tests perform destructive operations that invalidate the session.
/// Always run last to avoid disrupting other tests.
/// </summary>
[Collection(WuzApiIntegrationCollection.Name)]
[Trait("Category", "Integration")]
[Trait("Tier", "3")]
public sealed class SessionDisconnectTests
{
    private readonly WuzApiIntegrationFixture fixture;

    public SessionDisconnectTests(WuzApiIntegrationFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    [TestTier(TestTiers.Destructive, order: 0)]
    [Trait("Category", "LiveApi")]
    [Trait("Category", TestCategories.Destructive)]
    public async Task DisconnectSession_WhenConnected_Succeeds()
    {
        // Act - WARNING: This will disconnect the WhatsApp session
        var result = await this.fixture.Client.DisconnectSessionAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}

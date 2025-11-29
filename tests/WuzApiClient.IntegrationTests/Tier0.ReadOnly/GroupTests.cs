using AwesomeAssertions;
using WuzApiClient.IntegrationTests.TestInfrastructure.Configuration;
using WuzApiClient.IntegrationTests.TestInfrastructure.Fixtures;
using WuzApiClient.IntegrationTests.TestInfrastructure.Helpers;
using WuzApiClient.IntegrationTests.TestInfrastructure.Ordering;

namespace WuzApiClient.IntegrationTests.Tier0.ReadOnly;

/// <summary>
/// Tier 0 (ReadOnly) tests for group information operations.
/// These tests retrieve group data without modifying any state.
/// </summary>
[Collection(WuzApiIntegrationCollection.Name)]
[Trait("Category", "Integration")]
[Trait("Tier", "0")]
public sealed class GroupTests
{
    private readonly WuzApiIntegrationFixture fixture;
    private readonly TestGroupManager groupManager;

    public GroupTests(WuzApiIntegrationFixture fixture)
    {
        this.fixture = fixture;
        this.groupManager = new TestGroupManager(fixture.Client);
        TestConfiguration.Configuration = fixture.Configuration;
    }

    [Fact]
    [TestTier(TestTiers.ReadOnly, order: 20)]
    [Trait("Category", "LiveApi")]
    public async Task GetGroups_ReturnsGroupList()
    {
        // Act
        var result = await this.fixture.Client.GetGroupsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue($"Expected success but got error: {(result.IsFailure ? result.Error.ToString() : "N/A")}");
        result.Value.Should().NotBeNull();
    }

    [Fact]
    [TestTier(TestTiers.ReadOnly, order: 21)]
    [Trait("Category", "LiveApi")]
    [Trait("Category", TestCategories.RequiresRealData)]
    public async Task GetGroupInfo_ValidGroupId_ReturnsInfo()
    {
        // Arrange - get or create test group
        var groupId = await this.groupManager.GetOrCreateTestGroupAsync();

        // Act
        var result = await this.fixture.Client.GetGroupInfoAsync(groupId);

        // Assert
        result.IsSuccess.Should().BeTrue($"Expected success but got error: {(result.IsFailure ? result.Error.ToString() : "N/A")}");
        result.Value.Should().NotBeNull();
        result.Value!.Jid.Should().NotBeNullOrEmpty();
    }

    [Fact]
    [TestTier(TestTiers.ReadOnly, order: 22)]
    [Trait("Category", "LiveApi")]
    [Trait("Category", TestCategories.RequiresRealData)]
    public async Task GetGroupInviteLink_ValidGroup_ReturnsLink()
    {
        // Arrange - get or create test group
        var groupId = await this.groupManager.GetOrCreateTestGroupAsync();

        // Act
        var result = await this.fixture.Client.GetGroupInviteLinkAsync(groupId);

        // Assert
        result.IsSuccess.Should().BeTrue($"Expected success but got error: {(result.IsFailure ? result.Error.ToString() : "N/A")}");
        result.Value.Should().NotBeNull();
        result.Value!.Link.Should().NotBeNullOrEmpty();
        result.Value.Link.Should().StartWith("https://chat.whatsapp.com/");
    }
}

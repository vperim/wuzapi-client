using AwesomeAssertions;
using WuzApiClient.IntegrationTests.TestInfrastructure.Configuration;
using WuzApiClient.IntegrationTests.TestInfrastructure.Fixtures;
using WuzApiClient.Common.Models;
using WuzApiClient.Models.Requests.Group;

namespace WuzApiClient.IntegrationTests.Group;

[Collection(WuzApiIntegrationCollection.Name)]
[Trait("Category", "Integration")]
public sealed class GroupIntegrationTests
{
    private readonly WuzApiIntegrationFixture fixture;

    public GroupIntegrationTests(WuzApiIntegrationFixture fixture)
    {
        this.fixture = fixture;
        TestConfiguration.Configuration = fixture.Configuration;
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    [Trait("Category", TestCategories.Safe)]
    public async Task GetGroups_ReturnsGroupList()
    {
        // Act
        var result = await this.fixture.Client.GetGroupsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue($"Expected success but got error: {(result.IsFailure ? result.Error.ToString() : "N/A")}");
        result.Value.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    [Trait("Category", TestCategories.Safe)]
    [Trait("Category", TestCategories.RequiresRealData)]
    public async Task GetGroupInfo_ValidGroupId_ReturnsInfo()
    {
        // Arrange - requires a real group JID
        var groupId = TestConfiguration.TestGroupId!;

        // Act
        var result = await this.fixture.Client.GetGroupInfoAsync(groupId);

        // Assert
        result.IsSuccess.Should().BeTrue($"Expected success but got error: {(result.IsFailure ? result.Error.ToString() : "N/A")}");
        result.Value.Should().NotBeNull();
        result.Value!.Jid.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    [Trait("Category", TestCategories.MildlyDestructive)]
    [Trait("Category", TestCategories.RequiresRealData)]
    public async Task CreateGroup_ValidRequest_ReturnsGroupInfo()
    {
        // Arrange - requires real phone numbers that exist on WhatsApp
        var request = new CreateGroupRequest
        {
            Name = $"Test Group {DateTime.UtcNow:yyyyMMddHHmmss}",
            Participants = [Jid.FromPhone(Phone.Create(TestConfiguration.TestPhoneNumber))]
        };

        // Act
        var result = await this.fixture.Client.CreateGroupAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue($"Expected success but got error: {(result.IsFailure ? result.Error.ToString() : "N/A")}");
        result.Value.Should().NotBeNull();
        result.Value!.Jid.Should().NotBeNull();
        result.Value.Name.Should().Be(request.Name);
    }

    [Fact]
    [Trait("Category", "LiveApi")]
    [Trait("Category", TestCategories.Safe)]
    [Trait("Category", TestCategories.RequiresRealData)]
    public async Task GetGroupInviteLink_ValidGroup_ReturnsLink()
    {
        // Arrange - requires a real group JID where the user is admin
        var groupId = TestConfiguration.TestGroupId!;

        // Act
        var result = await this.fixture.Client.GetGroupInviteLinkAsync(groupId);

        // Assert
        result.IsSuccess.Should().BeTrue($"Expected success but got error: {(result.IsFailure ? result.Error.ToString() : "N/A")}");
        result.Value.Should().NotBeNull();
        result.Value!.Link.Should().NotBeNullOrEmpty();
        result.Value.Link.Should().StartWith("https://chat.whatsapp.com/");
    }
}

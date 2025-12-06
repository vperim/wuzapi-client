using AwesomeAssertions;
using WuzApiClient.Common.Models;
using WuzApiClient.Models.Common;
using WuzApiClient.Models.Requests.Group;
using WuzApiClient.Models.Responses.Group;
using WuzApiClient.UnitTests.TestInfrastructure.Fixtures;

namespace WuzApiClient.UnitTests.Core.Implementations;

[Trait("Category", "Unit")]
public sealed class WuzApiClientGroupTests : WuzApiClientTestBase
{
    #region GetGroupsAsync

    [Fact]
    public async Task GetGroupsAsync_Success_ReturnsGroupList()
    {
        // Arrange
        var expected = new GroupListResponse
        {
            Groups =
            [
                new GroupSummary { Jid = "123456@g.us", Name = "Test Group", Topic = "Test Topic" }
            ]
        };
        this.MockHandler.EnqueueSuccessResponse(expected);

        // Act
        var result = await this.Sut.GetGroupsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Groups.Should().HaveCount(1);
        result.Value.Groups[0].Jid?.Value.Should().Be("123456@g.us");
        result.Value.Groups[0].Name.Should().Be("Test Group");
    }

    [Fact]
    public async Task GetGroupsAsync_SendsCorrectEndpoint()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new GroupListResponse());

        // Act
        await this.Sut.GetGroupsAsync();

        // Assert
        var request = this.MockHandler.ReceivedRequests[0];
        request.Method.Should().Be(HttpMethod.Get);
        request.RequestUri!.PathAndQuery.Should().Be("/group/list");
    }

    #endregion

    #region CreateGroupAsync

    [Fact]
    public async Task CreateGroupAsync_Success_ReturnsGroupInfo()
    {
        // Arrange
        var expected = new GroupInfoResponse
        {
            Jid = "newgroup@g.us",
            Name = "New Group",
            OwnerJid = "owner@s.whatsapp.net"
        };
        this.MockHandler.EnqueueSuccessResponse(expected);

        var request = new CreateGroupRequest
        {
            Name = "New Group",
            Participants = [Jid.FromPhone(Phone.Create("5511999999999"))]
        };

        // Act
        var result = await this.Sut.CreateGroupAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Jid?.Value.Should().Be("newgroup@g.us");
        result.Value.Name.Should().Be("New Group");
    }

    [Fact]
    public async Task CreateGroupAsync_SendsNameAndParticipants()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new GroupInfoResponse());

        var request = new CreateGroupRequest
        {
            Name = "Test Group",
            Participants = [Jid.FromPhone(Phone.Create("5511888888888"))]
        };

        // Act
        await this.Sut.CreateGroupAsync(request);

        // Assert
        var httpRequest = this.MockHandler.ReceivedRequests[0];
        httpRequest.Method.Should().Be(HttpMethod.Post);
        httpRequest.RequestUri!.PathAndQuery.Should().Be("/group/create");

        var content = this.MockHandler.ReceivedRequestContents[0];
        content.Should().Contain("\"name\"");
        content.Should().Contain("\"Test Group\"");
        content.Should().Contain("\"participants\"");
        content.Should().Contain("5511888888888");
    }

    #endregion

    #region GetGroupInfoAsync

    [Fact]
    public async Task GetGroupInfoAsync_Success_ReturnsGroupInfo()
    {
        // Arrange
        var expected = new GroupInfoResponse
        {
            Jid = "group123@g.us",
            Name = "Info Group",
            Topic = "Group Topic",
            GroupCreated = "2023-11-14T00:00:00Z",
            OwnerJid = "owner@s.whatsapp.net",
            Participants =
            [
                new GroupParticipant { Jid = "user1@s.whatsapp.net", IsAdmin = true }
            ]
        };
        this.MockHandler.EnqueueSuccessResponse(expected);

        // Act
        var result = await this.Sut.GetGroupInfoAsync("group123@g.us");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Jid?.Value.Should().Be("group123@g.us");
        result.Value.Name.Should().Be("Info Group");
        result.Value.Participants.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetGroupInfoAsync_IncludesGroupIdInRequest()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new GroupInfoResponse());

        // Act
        await this.Sut.GetGroupInfoAsync("mygroup@g.us");

        // Assert
        var request = this.MockHandler.ReceivedRequests[0];
        request.Method.Should().Be(HttpMethod.Get);
        request.RequestUri!.PathAndQuery.Should().Be("/group/info?groupJID=mygroup@g.us");
    }

    #endregion

    #region GetGroupInviteLinkAsync

    [Fact]
    public async Task GetGroupInviteLinkAsync_Success_ReturnsLink()
    {
        // Arrange
        var expected = new GroupInviteLinkResponse { Link = "https://chat.whatsapp.com/abc123" };
        this.MockHandler.EnqueueSuccessResponse(expected);

        // Act
        var result = await this.Sut.GetGroupInviteLinkAsync("group@g.us");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Link.Should().Be("https://chat.whatsapp.com/abc123");
    }

    #endregion

    #region UpdateGroupPhotoAsync

    [Fact]
    public async Task UpdateGroupPhotoAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new object());

        var request = new UpdateGroupPhotoRequest
        {
            GroupId = "group@g.us",
            Photo = "base64encodedphoto"
        };

        // Act
        var result = await this.Sut.UpdateGroupPhotoAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var httpRequest = this.MockHandler.ReceivedRequests[0];
        httpRequest.RequestUri!.PathAndQuery.Should().Be("/group/photo");
    }

    #endregion

    #region RemoveGroupPhotoAsync

    [Fact]
    public async Task RemoveGroupPhotoAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new object());

        // Act
        var result = await this.Sut.RemoveGroupPhotoAsync("group@g.us");

        // Assert
        result.IsSuccess.Should().BeTrue();
        var httpRequest = this.MockHandler.ReceivedRequests[0];
        httpRequest.RequestUri!.PathAndQuery.Should().Be("/group/photo/remove");

        var content = this.MockHandler.ReceivedRequestContents[0];
        content.Should().Contain("\"groupId\"");
        content.Should().Contain("group@g.us");
    }

    #endregion

    #region UpdateGroupNameAsync

    [Fact]
    public async Task UpdateGroupNameAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new object());

        var request = new UpdateGroupNameRequest
        {
            GroupId = "group@g.us",
            Name = "New Name"
        };

        // Act
        var result = await this.Sut.UpdateGroupNameAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var httpRequest = this.MockHandler.ReceivedRequests[0];
        httpRequest.RequestUri!.PathAndQuery.Should().Be("/group/name");
    }

    #endregion

    #region UpdateGroupTopicAsync

    [Fact]
    public async Task UpdateGroupTopicAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new object());

        var request = new UpdateGroupTopicRequest
        {
            GroupId = "group@g.us",
            Topic = "New Topic"
        };

        // Act
        var result = await this.Sut.UpdateGroupTopicAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var httpRequest = this.MockHandler.ReceivedRequests[0];
        httpRequest.RequestUri!.PathAndQuery.Should().Be("/group/topic");
    }

    #endregion

    #region SetGroupAnnounceAsync

    [Fact]
    public async Task SetGroupAnnounceAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new object());

        var request = new SetGroupAnnounceRequest
        {
            GroupId = "group@g.us",
            Announce = true
        };

        // Act
        var result = await this.Sut.SetGroupAnnounceAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var httpRequest = this.MockHandler.ReceivedRequests[0];
        httpRequest.RequestUri!.PathAndQuery.Should().Be("/group/announce");
    }

    #endregion

    #region ManageParticipantsAsync

    [Fact]
    public async Task ManageParticipantsAsync_Add_SendsCorrectAction()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new object());

        var request = new ManageParticipantsRequest
        {
            GroupId = new Jid("group@g.us"),
            Participants = [Jid.FromPhone(Phone.Create("5511999999999"))],
            Action = ParticipantAction.Add
        };

        // Act
        var result = await this.Sut.ManageParticipantsAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var httpRequest = this.MockHandler.ReceivedRequests[0];
        httpRequest.RequestUri!.PathAndQuery.Should().Be("/group/participants");

        var content = this.MockHandler.ReceivedRequestContents[0];
        content.Should().Contain("\"action\"");
        content.Should().Contain("add", because: "action should serialize as 'add'");
    }

    [Fact]
    public async Task ManageParticipantsAsync_Remove_SendsCorrectAction()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new object());

        var request = new ManageParticipantsRequest
        {
            GroupId = new Jid("group@g.us"),
            Participants = [Jid.FromPhone(Phone.Create("5511888888888"))],
            Action = ParticipantAction.Remove
        };

        // Act
        var result = await this.Sut.ManageParticipantsAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var content = this.MockHandler.ReceivedRequestContents[0];
        content.Should().Contain("\"action\"");
        content.Should().Contain("remove", because: "action should serialize as 'remove'");
    }

    #endregion

    #region SetGroupLockedAsync

    [Fact]
    public async Task SetGroupLockedAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new object());

        var request = new SetGroupLockRequest
        {
            GroupId = "group@g.us",
            Locked = true
        };

        // Act
        var result = await this.Sut.SetGroupLockedAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var httpRequest = this.MockHandler.ReceivedRequests[0];
        httpRequest.RequestUri!.PathAndQuery.Should().Be("/group/lock");
    }

    #endregion

    #region SetDisappearingMessagesAsync

    [Fact]
    public async Task SetDisappearingMessagesAsync_Success_ReturnsSuccess()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new object());

        var request = new SetDisappearingMessagesRequest
        {
            GroupId = "group@g.us",
            Timer = 86400
        };

        // Act
        var result = await this.Sut.SetDisappearingMessagesAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var httpRequest = this.MockHandler.ReceivedRequests[0];
        httpRequest.RequestUri!.PathAndQuery.Should().Be("/group/disappearing");
    }

    #endregion

    #region JoinGroupAsync

    [Fact]
    public async Task JoinGroupAsync_Success_ReturnsGroupInfo()
    {
        // Arrange
        var expected = new GroupInfoResponse
        {
            Jid = "joinedgroup@g.us",
            Name = "Joined Group"
        };
        this.MockHandler.EnqueueSuccessResponse(expected);

        var request = new JoinGroupRequest { Code = "invite123" };

        // Act
        var result = await this.Sut.JoinGroupAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Jid?.Value.Should().Be("joinedgroup@g.us");
        result.Value.Name.Should().Be("Joined Group");

        var httpRequest = this.MockHandler.ReceivedRequests[0];
        httpRequest.RequestUri!.PathAndQuery.Should().Be("/group/join");
    }

    #endregion

    #region GetInviteInfoAsync

    [Fact]
    public async Task GetInviteInfoAsync_Success_ReturnsInviteInfo()
    {
        // Arrange
        var expected = new GroupInviteInfoResponse
        {
            GroupJid = "targetgroup@g.us",
            GroupName = "Target Group",
            Participants = 25,
            Creator = "creator@s.whatsapp.net"
        };
        this.MockHandler.EnqueueSuccessResponse(expected);

        var request = new GetInviteInfoRequest { InviteLink = "https://chat.whatsapp.com/xyz789" };

        // Act
        var result = await this.Sut.GetInviteInfoAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.GroupJid?.Value.Should().Be("targetgroup@g.us");
        result.Value.GroupName.Should().Be("Target Group");
        result.Value.Participants.Should().Be(25);
        result.Value.Creator?.Value.Should().Be("creator@s.whatsapp.net");

        var httpRequest = this.MockHandler.ReceivedRequests[0];
        httpRequest.RequestUri!.PathAndQuery.Should().Be("/group/inviteinfo");
    }

    #endregion
}

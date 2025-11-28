using System.Net;
using AwesomeAssertions;
using WuzApiClient.Models.Common;
using WuzApiClient.Models.Responses.User;
using WuzApiClient.Results;
using WuzApiClient.UnitTests.TestInfrastructure.Fixtures;

namespace WuzApiClient.UnitTests.Core.Implementations;

[Trait("Category", "Unit")]
public sealed class WuzApiClientUserTests : WuzApiClientTestBase
{
    private static readonly Phone TestPhone = Phone.Create("5511999999999");

    #region GetUserInfoAsync Tests

    [Fact]
    public async Task GetUserInfoAsync_Success_ReturnsUserInfo()
    {
        // Arrange
        var jid = "5511999999999@s.whatsapp.net";
        var expectedResponse = new UserInfoResponse
        {
            Users = new Dictionary<string, UserInfo>
            {
                [jid] = new UserInfo
                {
                    Status = "Available",
                    PictureId = "profile_pic_123"
                }
            }
        };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);

        // Act
        var result = await this.Sut.GetUserInfoAsync(TestPhone);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Users.Should().ContainKey(jid);
        result.Value.Users[jid].Status.Should().Be("Available");
        result.Value.Users[jid].PictureId.Should().Be("profile_pic_123");
    }

    [Fact]
    public async Task GetUserInfoAsync_SendsPhoneInRequest()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new UserInfoResponse());

        // Act
        await this.Sut.GetUserInfoAsync(TestPhone);

        // Assert
        this.MockHandler.ReceivedRequests.Should().ContainSingle();
        var request = this.MockHandler.ReceivedRequests[0];
        request.Method.Should().Be(HttpMethod.Post);
        request.RequestUri!.PathAndQuery.Should().Be("/user/info");

        var content = this.MockHandler.ReceivedRequestContents[0];
        content.Should().Contain("\"Phone\"");
        content.Should().Contain("5511999999999@s.whatsapp.net");
    }

    [Fact]
    public async Task GetUserInfoAsync_HttpError_ReturnsFailure()
    {
        // Arrange
        this.MockHandler.EnqueueErrorResponse(HttpStatusCode.InternalServerError, "Server error");

        // Act
        var result = await this.Sut.GetUserInfoAsync(TestPhone);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(WuzApiErrorCode.InternalServerError);
    }

    #endregion

    #region CheckPhonesAsync Tests

    [Fact]
    public async Task CheckPhonesAsync_Success_ReturnsCheckResults()
    {
        // Arrange
        var expectedResponse = new CheckPhonesResponse
        {
            Users =
            [
                new CheckPhoneResult
                {
                    Query = "5511999999999",
                    IsInWhatsapp = true,
                    Jid = "5511999999999@s.whatsapp.net"
                }
            ]
        };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);

        // Act
        var result = await this.Sut.CheckPhonesAsync([TestPhone]);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Users.Should().HaveCount(1);
        result.Value.Users[0].IsInWhatsapp.Should().BeTrue();
    }

    [Fact]
    public async Task CheckPhonesAsync_MultiplePhones_SendsArray()
    {
        // Arrange
        var phone1 = Phone.Create("5511999999991");
        var phone2 = Phone.Create("5511999999992");
        var phone3 = Phone.Create("5511999999993");
        this.MockHandler.EnqueueSuccessResponse(new CheckPhonesResponse());

        // Act
        await this.Sut.CheckPhonesAsync([phone1, phone2, phone3]);

        // Assert
        this.MockHandler.ReceivedRequests.Should().ContainSingle();
        var request = this.MockHandler.ReceivedRequests[0];
        request.Method.Should().Be(HttpMethod.Post);
        request.RequestUri!.PathAndQuery.Should().Be("/user/check");

        var content = this.MockHandler.ReceivedRequestContents[0];
        content.Should().Contain("\"Phone\"");
        content.Should().Contain("5511999999991");
        content.Should().Contain("5511999999992");
        content.Should().Contain("5511999999993");
    }

    #endregion

    #region GetAvatarAsync Tests

    [Fact]
    public async Task GetAvatarAsync_Success_ReturnsAvatar()
    {
        // Arrange
        var expectedResponse = new AvatarResponse
        {
            Url = "https://example.com/avatar.jpg",
            Id = "avatar_id_123",
            Type = "full"
        };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);

        // Act
        var result = await this.Sut.GetAvatarAsync(TestPhone);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Url.Should().Be(expectedResponse.Url);
        result.Value.Id.Should().Be(expectedResponse.Id);
    }

    [Fact]
    public async Task GetAvatarAsync_IncludesPhoneInRequest()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new AvatarResponse());

        // Act
        await this.Sut.GetAvatarAsync(TestPhone);

        // Assert
        this.MockHandler.ReceivedRequests.Should().ContainSingle();
        var request = this.MockHandler.ReceivedRequests[0];
        request.Method.Should().Be(HttpMethod.Get);
        request.RequestUri!.PathAndQuery.Should().Be("/user/avatar?Phone=5511999999999");
    }

    #endregion

    #region GetContactsAsync Tests

    [Fact]
    public async Task GetContactsAsync_Success_ReturnsContacts()
    {
        // Arrange
        var jid = "5511999999999@s.whatsapp.net";
        var expectedResponse = new Dictionary<string, ContactInfo>
        {
            [jid] = new ContactInfo
            {
                FullName = "Contact Name",
                PushName = "Push Name"
            }
        };
        this.MockHandler.EnqueueSuccessResponse(expectedResponse);

        // Act
        var result = await this.Sut.GetContactsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Contacts.Should().HaveCount(1);
        result.Value.Contacts.Should().ContainKey(jid);
        result.Value.Contacts[jid].FullName.Should().Be("Contact Name");
    }

    [Fact]
    public async Task GetContactsAsync_SendsCorrectEndpoint()
    {
        // Arrange
        this.MockHandler.EnqueueSuccessResponse(new Dictionary<string, ContactInfo>());

        // Act
        await this.Sut.GetContactsAsync();

        // Assert
        this.MockHandler.ReceivedRequests.Should().ContainSingle();
        var request = this.MockHandler.ReceivedRequests[0];
        request.Method.Should().Be(HttpMethod.Get);
        request.RequestUri!.PathAndQuery.Should().Be("/user/contacts");
    }

    #endregion
}

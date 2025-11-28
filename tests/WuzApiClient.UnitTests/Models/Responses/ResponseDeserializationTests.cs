using System.Text.Json;
using AwesomeAssertions;
using WuzApiClient.Json;
using WuzApiClient.Models.Responses.Admin;
using WuzApiClient.Models.Responses.Chat;
using WuzApiClient.Models.Responses.Download;
using WuzApiClient.Models.Responses.Group;
using WuzApiClient.Models.Responses.Session;
using WuzApiClient.Models.Responses.User;

namespace WuzApiClient.UnitTests.Models.Responses;

/// <summary>
/// Unit tests for response model deserialization.
/// </summary>
[Trait("Category", "Unit")]
public sealed class ResponseDeserializationTests
{
    private readonly JsonSerializerOptions options = WuzApiJsonSerializerOptions.Default;

    [Fact]
    public void SessionStatusResponse_Deserialization_MapsAllProperties()
    {
        var json = """{"connected":true,"loggedIn":true,"jid":"5511999999999@s.whatsapp.net"}""";

        var response = JsonSerializer.Deserialize<SessionStatusResponse>(json, this.options);

        response.Should().NotBeNull();
        response!.Connected.Should().BeTrue();
        response.LoggedIn.Should().BeTrue();
        response.Jid.Should().Be("5511999999999@s.whatsapp.net");
    }

    [Fact]
    public void SendMessageResponse_Deserialization_MapsAllProperties()
    {
        var json = """{"id":"ABC123","timestamp":1700000000,"details":"Message sent"}""";

        var response = JsonSerializer.Deserialize<SendMessageResponse>(json, this.options);

        response.Should().NotBeNull();
        response!.Id.Should().Be("ABC123");
        response.Timestamp.Should().Be(1700000000);
        response.Details.Should().Be("Message sent");
        response.MessageId.Should().Be("ABC123");
    }

    [Fact]
    public void GroupInfoResponse_Deserialization_MapsNestedParticipants()
    {
        var json = """
            {
                "JID":"123456789@g.us",
                "Name":"Test Group",
                "Topic":"Group topic",
                "GroupCreated":"2023-11-14T00:00:00Z",
                "OwnerJID":"5511999999999@s.whatsapp.net",
                "Participants":[
                    {"JID":"5511999999999@s.whatsapp.net","IsAdmin":true,"IsSuperAdmin":true},
                    {"JID":"5511888888888@s.whatsapp.net","IsAdmin":false,"IsSuperAdmin":false}
                ]
            }
            """;

        var response = JsonSerializer.Deserialize<GroupInfoResponse>(json, this.options);

        response.Should().NotBeNull();
        response!.Jid.Should().Be("123456789@g.us");
        response.Name.Should().Be("Test Group");
        response.Topic.Should().Be("Group topic");
        response.GroupCreated.Should().Be("2023-11-14T00:00:00Z");
        response.OwnerJid.Should().Be("5511999999999@s.whatsapp.net");
        response.Participants.Should().HaveCount(2);
        response.Participants[0].Jid.Should().Be("5511999999999@s.whatsapp.net");
        response.Participants[0].IsAdmin.Should().BeTrue();
        response.Participants[0].IsSuperAdmin.Should().BeTrue();
        response.Participants[1].IsAdmin.Should().BeFalse();
    }

    [Fact]
    public void CheckPhonesResponse_Deserialization_MapsUserArray()
    {
        var json = """
            {
                "Users":[
                    {"Query":"5511999999999","IsInWhatsapp":true,"JID":"5511999999999@s.whatsapp.net"},
                    {"Query":"5511888888888","IsInWhatsapp":false}
                ]
            }
            """;

        var response = JsonSerializer.Deserialize<CheckPhonesResponse>(json, this.options);

        response.Should().NotBeNull();
        response!.Users.Should().HaveCount(2);
        response.Users[0].Query.Should().Be("5511999999999");
        response.Users[0].IsInWhatsapp.Should().BeTrue();
        response.Users[0].Jid.Should().Be("5511999999999@s.whatsapp.net");
        response.Users[1].IsInWhatsapp.Should().BeFalse();
        response.Users[1].Jid.Should().BeNull();
    }

    [Fact]
    public void MediaDownloadResponse_Deserialization_MapsAllProperties()
    {
        var json = """{"data":"base64encodeddata","mimetype":"image/jpeg","filename":"image.jpg"}""";

        var response = JsonSerializer.Deserialize<MediaDownloadResponse>(json, this.options);

        response.Should().NotBeNull();
        response!.Data.Should().Be("base64encodeddata");
        response.MimeType.Should().Be("image/jpeg");
        response.FileName.Should().Be("image.jpg");
    }

    [Fact]
    public void UserListResponse_Deserialization_MapsUserArray()
    {
        var json = """
            {
                "users":[
                    {"id":"hash1","name":"user1","token":"token1","webhook":"https://example.com/hook1","events":"message","connected":true},
                    {"id":"hash2","name":"user2","token":"token2","connected":false}
                ]
            }
            """;

        var response = JsonSerializer.Deserialize<UserListResponse>(json, this.options);

        response.Should().NotBeNull();
        response!.Users.Should().HaveCount(2);
        response.Users[0].Id.Should().Be("hash1");
        response.Users[0].Name.Should().Be("user1");
        response.Users[0].Token.Should().Be("token1");
        response.Users[0].Webhook.Should().Be("https://example.com/hook1");
        response.Users[0].Events.Should().Be("message");
        response.Users[0].Connected.Should().BeTrue();
        response.Users[1].Id.Should().Be("hash2");
        response.Users[1].Connected.Should().BeFalse();
    }

    [Fact]
    public void QrCodeResponse_Deserialization_MapsAllProperties()
    {
        var json = """{"qrcode":"data:image/png;base64,iVBORw0KGgo="}""";

        var response = JsonSerializer.Deserialize<QrCodeResponse>(json, this.options);

        response.Should().NotBeNull();
        response!.QrCode.Should().Be("data:image/png;base64,iVBORw0KGgo=");
    }

    [Fact]
    public void SessionStatusResponse_CaseInsensitive_DeserializesCorrectly()
    {
        var json = """{"Connected":true,"LoggedIn":false}""";

        var response = JsonSerializer.Deserialize<SessionStatusResponse>(json, this.options);

        response.Should().NotBeNull();
        response!.Connected.Should().BeTrue();
        response.LoggedIn.Should().BeFalse();
    }

    [Fact]
    public void SendMessageResponse_TimestampFromString_DeserializesCorrectly()
    {
        var json = """{"id":"ABC123","timestamp":"1700000000"}""";

        var response = JsonSerializer.Deserialize<SendMessageResponse>(json, this.options);

        response.Should().NotBeNull();
        response!.Timestamp.Should().Be(1700000000);
    }
}

using System.Text.Json;
using AwesomeAssertions;
using WuzApiClient.Json;
using WuzApiClient.Models.Common;
using WuzApiClient.Models.Requests.Admin;
using WuzApiClient.Models.Requests.Chat;
using WuzApiClient.Models.Requests.Group;
using WuzApiClient.Models.Requests.Session;
using WuzApiClient.Models.Requests.Webhook;

namespace WuzApiClient.UnitTests.Models.Requests;

/// <summary>
/// Unit tests for request model serialization.
/// </summary>
[Trait("Category", "Unit")]
public sealed class RequestSerializationTests
{
    private readonly JsonSerializerOptions options = WuzApiJsonSerializerOptions.Default;

    [Fact]
    public void SendTextMessageRequest_Serialization_ProducesExpectedJson()
    {
        var request = new SendTextMessageRequest
        {
            Phone = Phone.Create("5511999999999"),
            Body = "Hello, World!"
        };

        var json = JsonSerializer.Serialize(request, this.options);

        json.Should().Contain("\"Phone\":\"5511999999999\"");
        json.Should().Contain("\"Body\":\"Hello, World!\"");
    }

    [Fact]
    public void SendImageRequest_Serialization_ProducesExpectedJson()
    {
        var request = new SendImageRequest
        {
            Phone = Phone.Create("5511999999999"),
            Image = "base64data",
            Caption = "Test caption",
            MimeType = "image/png"
        };

        var json = JsonSerializer.Serialize(request, this.options);

        json.Should().Contain("\"Phone\":\"5511999999999\"");
        json.Should().Contain("\"Image\":\"base64data\"");
        json.Should().Contain("\"Caption\":\"Test caption\"");
        json.Should().Contain("\"MimeType\":\"image/png\"");
    }

    [Fact]
    public void CreateGroupRequest_Serialization_ProducesExpectedJson()
    {
        var request = new CreateGroupRequest
        {
            Name = "Test Group",
            Participants = [Phone.Create("5511999999999"), Phone.Create("5511888888888")]
        };

        var json = JsonSerializer.Serialize(request, this.options);

        json.Should().Contain("\"name\":\"Test Group\"");
        json.Should().Contain("\"participants\":[\"5511999999999\",\"5511888888888\"]");
    }

    [Fact]
    public void SetWebhookRequest_Serialization_ProducesExpectedJson()
    {
        var request = new SetWebhookRequest
        {
            Url = "https://example.com/webhook",
            Events = [SubscribableEvent.Message, SubscribableEvent.Presence]
        };

        var json = JsonSerializer.Serialize(request, this.options);

        json.Should().Contain("\"url\":\"https://example.com/webhook\"");
        json.Should().Contain("\"events\":[\"Message\",\"Presence\"]");
    }

    [Fact]
    public void CreateUserRequest_Serialization_ProducesExpectedJson()
    {
        var request = new CreateUserRequest
        {
            Name = "testuser",
            Token = "secret-token",
            Webhook = "https://example.com/hook",
            Events = "message,status"
        };

        var json = JsonSerializer.Serialize(request, this.options);

        json.Should().Contain("\"name\":\"testuser\"");
        json.Should().Contain("\"token\":\"secret-token\"");
        json.Should().Contain("\"webhook\":\"https://example.com/hook\"");
        json.Should().Contain("\"events\":\"message,status\"");
    }

    [Fact]
    public void ConnectSessionRequest_Serialization_ProducesExpectedJson()
    {
        var request = new ConnectSessionRequest
        {
            Subscribe = [SubscribableEvent.Message, SubscribableEvent.Presence],
            Immediate = true
        };

        var json = JsonSerializer.Serialize(request, this.options);

        json.Should().Contain("\"subscribe\":[\"Message\",\"Presence\"]");
        json.Should().Contain("\"immediate\":true");
    }

    [Fact]
    public void SendTextMessageRequest_NullOptionalProperties_OmitsFromJson()
    {
        var request = new SendTextMessageRequest
        {
            Phone = Phone.Create("5511999999999"),
            Body = "Hello"
        };

        var json = JsonSerializer.Serialize(request, this.options);

        json.Should().NotContain("\"Id\"");
    }

    [Fact]
    public void SetWebhookRequest_NullEvents_OmitsFromJson()
    {
        var request = new SetWebhookRequest
        {
            Url = "https://example.com/webhook"
        };

        var json = JsonSerializer.Serialize(request, this.options);

        json.Should().NotContain("\"events\"");
    }

    [Fact]
    public void ReactRequest_Serialization_SerializesEmojiAsBody()
    {
        var request = new ReactRequest
        {
            Phone = Phone.Create("5511999999999"),
            MessageId = "ABC123",
            Emoji = "👍"
        };

        var json = JsonSerializer.Serialize(request, this.options);

        json.Should().Contain("\"Phone\":\"5511999999999\"");
        json.Should().Contain("\"Id\":\"ABC123\"");
        json.Should().Contain("\"Body\":");
        json.Should().NotContain("\"Emoji\"");

        // Verify the emoji value by parsing the JSON
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.GetProperty("Body").GetString().Should().Be("👍");
    }

    [Fact]
    public void MarkAsReadRequest_Serialization_ProducesExpectedJson()
    {
        var request = new MarkAsReadRequest
        {
            ChatPhone = Phone.Create("5511999999999"),
            MessageIds = ["msg1", "msg2"]
        };

        var json = JsonSerializer.Serialize(request, this.options);

        json.Should().Contain("\"ChatPhone\":\"5511999999999\"");
        json.Should().Contain("\"Id\":[\"msg1\",\"msg2\"]");
        json.Should().NotContain("\"Phone\"");
    }

    [Fact]
    public void SetPresenceRequest_Serialization_ProducesExpectedJson()
    {
        var request = new SetPresenceRequest
        {
            Phone = Phone.Create("5511999999999"),
            State = "composing"
        };

        var json = JsonSerializer.Serialize(request, this.options);

        json.Should().Contain("\"Phone\":\"5511999999999\"");
        json.Should().Contain("\"State\":\"composing\"");
    }

    [Fact]
    public void JoinGroupRequest_Serialization_ProducesExpectedJson()
    {
        var request = new JoinGroupRequest
        {
            Code = "ABC123XYZ"
        };

        var json = JsonSerializer.Serialize(request, this.options);

        json.Should().Contain("\"Code\":\"ABC123XYZ\"");
        json.Should().NotContain("\"InviteLink\"");
    }

    [Fact]
    public void SendListRequest_Serialization_ProducesExpectedJson()
    {
        var request = new SendListRequest
        {
            Phone = Phone.Create("5511999999999"),
            ButtonText = "Select",
            Description = "Choose an option",
            Sections =
            [
                new ListSection
                {
                    Title = "Section 1",
                    Rows =
                    [
                        new ListRow { RowId = "1", Title = "Option 1" }
                    ]
                }
            ]
        };

        var json = JsonSerializer.Serialize(request, this.options);

        json.Should().Contain("\"Phone\":\"5511999999999\"");
        json.Should().Contain("\"ButtonText\":\"Select\"");
        json.Should().Contain("\"Desc\":\"Choose an option\"");
        json.Should().Contain("\"Sections\":");
        json.Should().Contain("\"Title\":\"Section 1\"");
        json.Should().Contain("\"RowId\":\"1\"");
    }

    [Fact]
    public void SendPollRequest_Serialization_ProducesExpectedJson()
    {
        var request = new SendPollRequest
        {
            Phone = Phone.Create("5511999999999"),
            Question = "What's your favorite color?",
            Options = ["Red", "Blue", "Green"],
            MaxSelections = 1
        };

        var json = JsonSerializer.Serialize(request, this.options);

        json.Should().Contain("\"Phone\":\"5511999999999\"");
        json.Should().Contain("\"Question\":");
        json.Should().Contain("\"Options\":[\"Red\",\"Blue\",\"Green\"]");
        json.Should().Contain("\"MaxSelections\":1");

        // Verify the question value by parsing the JSON (handles unicode escaping)
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.GetProperty("Question").GetString().Should().Be("What's your favorite color?");
    }

    [Fact]
    public void DownloadImageRequest_Serialization_ProducesExpectedJson()
    {
        var request = new DownloadImageRequest
        {
            Url = "https://example.com/image",
            MediaKey = "abc123",
            MimeType = "image/jpeg",
            FileSha256 = "sha256hash",
            FileLength = 1024
        };

        var json = JsonSerializer.Serialize(request, this.options);

        json.Should().Contain("\"Url\":\"https://example.com/image\"");
        json.Should().Contain("\"Mediakey\":\"abc123\"");
        json.Should().Contain("\"Mimetype\":\"image/jpeg\"");
        json.Should().Contain("\"FileSHA256\":\"sha256hash\"");
        json.Should().Contain("\"FileLength\":1024");
    }
}

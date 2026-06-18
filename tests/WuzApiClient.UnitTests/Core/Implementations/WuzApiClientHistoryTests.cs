using System.Net;
using AwesomeAssertions;
using WuzApiClient.UnitTests.TestInfrastructure.Fixtures;

namespace WuzApiClient.UnitTests.Core.Implementations;

[Trait("Category", "Unit")]
public sealed class WuzApiClientHistoryTests : WuzApiClientTestBase
{
    private const string TestChatJid = "5511999999999@s.whatsapp.net";

    #region GetChatHistoryAsync

    [Fact]
    public async Task GetChatHistoryAsync_SendsGetToChatHistory()
    {
        // Arrange
        this.MockHandler.EnqueueResponse(HttpStatusCode.OK, "[]");

        // Act
        await this.Sut.GetChatHistoryAsync(TestChatJid, 25);

        // Assert
        this.MockHandler.ReceivedRequests.Should().ContainSingle();
        var request = this.MockHandler.ReceivedRequests[0];
        request.Method.Should().Be(HttpMethod.Get);
        request.RequestUri!.AbsolutePath.Should().Be("/chat/history");
        request.RequestUri!.Query.Should().Contain("chat_jid=");
        request.RequestUri!.Query.Should().Contain("limit=25");
    }

    [Fact]
    public async Task GetChatHistoryAsync_DeserializesSnakeCaseFields()
    {
        // Arrange
        const string json = """
        [
          {
            "id": 7,
            "user_id": "u1",
            "chat_jid": "5511999999999@s.whatsapp.net",
            "sender_jid": "5511888888888@s.whatsapp.net",
            "message_id": "ABC123",
            "timestamp": "2026-06-18T10:30:00Z",
            "message_type": "text",
            "text_content": "Olá",
            "media_link": "",
            "quoted_message_id": "",
            "data_json": "{}"
          }
        ]
        """;
        this.MockHandler.EnqueueResponse(HttpStatusCode.OK, json);

        // Act
        var result = await this.Sut.GetChatHistoryAsync(TestChatJid);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        var msg = result.Value[0];
        msg.Id.Should().Be(7);
        msg.ChatJid.Should().Be("5511999999999@s.whatsapp.net");
        msg.MessageId.Should().Be("ABC123");
        msg.MessageType.Should().Be("text");
        msg.TextContent.Should().Be("Olá");
    }

    #endregion

    #region GetChatHistoryIndexAsync

    [Fact]
    public async Task GetChatHistoryIndexAsync_SendsIndexQuery()
    {
        // Arrange
        this.MockHandler.EnqueueResponse(HttpStatusCode.OK, "{}");

        // Act
        await this.Sut.GetChatHistoryIndexAsync();

        // Assert
        var request = this.MockHandler.ReceivedRequests[0];
        request.Method.Should().Be(HttpMethod.Get);
        request.RequestUri!.AbsolutePath.Should().Be("/chat/history");
        request.RequestUri!.Query.Should().Contain("chat_jid=index");
    }

    [Fact]
    public async Task GetChatHistoryIndexAsync_DeserializesUserKeyedMap()
    {
        // Arrange
        const string json = """
        {
          "u1": [ { "chat_jid": "5511999999999@s.whatsapp.net", "last_updated": "2026-06-18T10:30:00Z" } ]
        }
        """;
        this.MockHandler.EnqueueResponse(HttpStatusCode.OK, json);

        // Act
        var result = await this.Sut.GetChatHistoryIndexAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainKey("u1");
        result.Value["u1"].Should().ContainSingle();
        result.Value["u1"][0].ChatJid.Should().Be("5511999999999@s.whatsapp.net");
    }

    #endregion

    #region RequestHistorySyncAsync

    [Fact]
    public async Task RequestHistorySyncAsync_SendsCountAndChatJid()
    {
        // Arrange
        this.MockHandler.EnqueueResponse(HttpStatusCode.OK, "{}");

        // Act
        await this.Sut.RequestHistorySyncAsync(30, TestChatJid);

        // Assert
        var request = this.MockHandler.ReceivedRequests[0];
        request.Method.Should().Be(HttpMethod.Get);
        request.RequestUri!.AbsolutePath.Should().Be("/session/history");
        request.RequestUri!.Query.Should().Contain("count=30");
        request.RequestUri!.Query.Should().Contain("chat_jid=");
    }

    [Fact]
    public async Task RequestHistorySyncAsync_OmitsChatJidWhenNull()
    {
        // Arrange
        this.MockHandler.EnqueueResponse(HttpStatusCode.OK, "{}");

        // Act
        await this.Sut.RequestHistorySyncAsync(10);

        // Assert
        var request = this.MockHandler.ReceivedRequests[0];
        request.RequestUri!.Query.Should().Contain("count=10");
        request.RequestUri!.Query.Should().NotContain("chat_jid");
    }

    #endregion

    #region SetHistoryAsync

    [Fact]
    public async Task SetHistoryAsync_PostsHistoryBody()
    {
        // Arrange
        this.MockHandler.EnqueueResponse(HttpStatusCode.OK, """{"Details":"ok","History":100}""");

        // Act
        var result = await this.Sut.SetHistoryAsync(100);

        // Assert
        var request = this.MockHandler.ReceivedRequests[0];
        request.Method.Should().Be(HttpMethod.Post);
        request.RequestUri!.AbsolutePath.Should().Be("/session/history");
        this.MockHandler.ReceivedRequestContents[0].Should().Contain("\"history\"");
        result.IsSuccess.Should().BeTrue();
        result.Value.History.Should().Be(100);
    }

    #endregion
}

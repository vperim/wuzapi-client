using System.Text.Json;
using AwesomeAssertions;
using WuzApiClient.RabbitMq.Models;
using WuzApiClient.RabbitMq.Models.Events;
using WuzApiClient.RabbitMq.Serialization;

namespace WuzApiClient.RabbitMq.UnitTests.Serialization;

/// <summary>
/// Unit tests for <see cref="WuzEventJsonConverter"/> JSON deserialization.
/// </summary>
[Trait("Category", "Unit")]
public sealed class WuzEventJsonConverterTests
{
    [Fact]
    public void Read_MessageEvent_DeserializesCorrectly()
    {
        // Arrange
        const string json = """{"type":"Message","userID":"user-123","instanceName":"instance-1","event":{}}""";

        // Act
        var result = JsonSerializer.Deserialize<WuzEvent>(json, WuzEventJsonSerializerOptions.Default);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<MessageEvent>();
        result!.Type.Should().Be("Message");
        result.UserId.Should().Be("user-123");
        result.InstanceName.Should().Be("instance-1");
    }

    [Fact]
    public void Read_PresenceEvent_DeserializesCorrectly()
    {
        // Arrange - wuzapi envelope format with event data inside "event" field
        // The "state" field is added at root level by wuzapi (online/offline)
        const string json = """{"type":"Presence","userID":"user-456","instanceName":"instance-2","state":"online","event":{"From":"contact@s.whatsapp.net","Unavailable":false,"LastSeen":"2025-01-15T10:30:00Z"}}""";

        // Act
        var result = JsonSerializer.Deserialize<WuzEvent>(json, WuzEventJsonSerializerOptions.Default);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PresenceEvent>();
        var presenceEvent = (PresenceEvent)result!;
        presenceEvent.Type.Should().Be("Presence");
        presenceEvent.UserId.Should().Be("user-456");
        presenceEvent.InstanceName.Should().Be("instance-2");
        presenceEvent.From.Should().Be("contact@s.whatsapp.net");
        presenceEvent.Unavailable.Should().BeFalse();
    }

    [Fact]
    public void Read_ConnectedEvent_DeserializesCorrectly()
    {
        // Arrange
        const string json = """{"type":"Connected","userID":"user-789","instanceName":"instance-3","event":{}}""";

        // Act
        var result = JsonSerializer.Deserialize<WuzEvent>(json, WuzEventJsonSerializerOptions.Default);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ConnectedEvent>();
        result!.Type.Should().Be("Connected");
        result.UserId.Should().Be("user-789");
        result.InstanceName.Should().Be("instance-3");
    }

    [Fact]
    public void Read_UnknownEventType_DeserializesToUnknownEvent()
    {
        // Arrange
        const string json = """{"type":"SomeNewEventType","userID":"user-000","instanceName":"instance-x","event":{}}""";

        // Act
        var result = JsonSerializer.Deserialize<WuzEvent>(json, WuzEventJsonSerializerOptions.Default);

        // Assert
        result.Should().NotBeNull();
        result!.GetType().Name.Should().Be("UnknownEvent");
        result.Type.Should().Be("SomeNewEventType");
        result.UserId.Should().Be("user-000");
        result.InstanceName.Should().Be("instance-x");
    }

    [Theory]
    [InlineData("message")]
    [InlineData("MESSAGE")]
    [InlineData("Message")]
    [InlineData("mEsSaGe")]
    public void Read_CaseInsensitiveTypeMatching_DeserializesToCorrectType(string eventType)
    {
        // Arrange
        var json = $$$"""{"type":"{{{eventType}}}","userID":"user-123","instanceName":"instance-1","event":{}}""";

        // Act
        var result = JsonSerializer.Deserialize<WuzEvent>(json, WuzEventJsonSerializerOptions.Default);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<MessageEvent>();
    }

    [Fact]
    public void Read_MissingTypeField_ThrowsJsonException()
    {
        // Arrange
        const string json = """{"userID":"user-123","instanceName":"instance-1","event":{}}""";

        // Act
        var act = () => JsonSerializer.Deserialize<WuzEvent>(json, WuzEventJsonSerializerOptions.Default);

        // Assert
        act.Should().Throw<JsonException>()
            .WithMessage("*missing*'type'*");
    }

    [Theory]
    [InlineData("""{"type":"","userID":"user-123","instanceName":"instance-1"}""")]
    [InlineData("""{"type":"   ","userID":"user-123","instanceName":"instance-1"}""")]
    public void Read_EmptyTypeField_ThrowsJsonException(string json)
    {
        // Act
        var act = () => JsonSerializer.Deserialize<WuzEvent>(json, WuzEventJsonSerializerOptions.Default);

        // Assert
        act.Should().Throw<JsonException>()
            .WithMessage("*'type'*null or empty*");
    }

    [Fact]
    public void Read_NullTypeField_ThrowsJsonException()
    {
        // Arrange
        const string json = """{"type":null,"userID":"user-123","instanceName":"instance-1"}""";

        // Act
        var act = () => JsonSerializer.Deserialize<WuzEvent>(json, WuzEventJsonSerializerOptions.Default);

        // Assert
        act.Should().Throw<JsonException>()
            .WithMessage("*'type'*null or empty*");
    }

    [Fact]
    public void Read_PropertiesAreMappedCorrectly()
    {
        // Arrange
        const string json = """{"type":"Message","userID":"test-user-id","instanceName":"test-instance-name","event":{"key":"value"}}""";

        // Act
        var result = JsonSerializer.Deserialize<WuzEvent>(json, WuzEventJsonSerializerOptions.Default);

        // Assert
        result.Should().NotBeNull();
        result!.Type.Should().Be("Message");
        result.UserId.Should().Be("test-user-id");
        result.InstanceName.Should().Be("test-instance-name");
        result.RawEvent.Should().NotBeNull();
        result.RawEvent!.Value.GetProperty("key").GetString().Should().Be("value");
    }
}

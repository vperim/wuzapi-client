using AwesomeAssertions;
using WuzApiClient.RabbitMq.Models.Events;
using WuzApiClient.RabbitMq.Models.Wuz;

namespace WuzApiClient.RabbitMq.UnitTests.Events;

/// <summary>
/// Tests event deserialization using real webhook data from wuzapi.
/// These tests validate the full deserialization pipeline from raw JSON to typed events.
/// </summary>
[Trait("Category", "Unit")]
public sealed class DeserializationTests
{
    /// <summary>
    /// Provides test data for all event payloads.
    /// Each entry contains: filename, expected event type name.
    /// </summary>
    public static IEnumerable<object[]> AllEventPayloads =>
        new List<object[]>
        {
            new object[] { "QR-1.json", "QR" },
            new object[] { "PairSuccess-1.json", "PairSuccess" },
            new object[] { "Message-1.json", "Message" },
            new object[] { "Message-2.json", "Message" },
            new object[] { "Message-3.json", "Message" },
            new object[] { "UndecryptableMessage-1.json", "UndecryptableMessage" },
            new object[] { "UndecryptableMessage-2.json", "UndecryptableMessage" },
            new object[] { "UndecryptableMessage-3.json", "UndecryptableMessage" },
            new object[] { "ReadReceipt-1.json", "ReadReceipt" },
            new object[] { "HistorySync-1.json", "HistorySync" }
        };

    /// <summary>
    /// Smoke test: Validates that all event payloads can be parsed without exceptions.
    /// Tests the Parse() method and metadata structure for all real webhook payloads.
    /// </summary>
    [Theory]
    [MemberData(nameof(AllEventPayloads))]
    public async Task EventPayload_ShouldDeserializeWithoutExceptions(string fileName, string expectedEventType)
    {
        var bytes = await LoadAsset(fileName);

        var metadata = WuzEventMetadata.Parse(bytes);

        metadata.WuzEnvelope.Should().NotBeNull();
        metadata.WuzEnvelope.UserId.Should().NotBeNullOrEmpty();
        metadata.WuzEnvelope.InstanceName.Should().Be("DashboardUser");
        metadata.RawJson.Should().NotBeNullOrEmpty();

        metadata.WaEventMetadata.Should().NotBeNull();
        metadata.WaEventMetadata.Type.ToString().Should().Be(expectedEventType);
        metadata.WaEventMetadata.Event.Should().NotBeNullOrEmpty();
    }

    // === Event-Specific Validation Tests ===

    [Fact]
    public async Task QrCodeEvent_ShouldDeserializeQrCodeData()
    {
        var bytes = await LoadAsset("QR-1.json");
        var metadata = WuzEventMetadata.Parse(bytes);
        var envelope = metadata.ToEnvelope<QrCodeEventEnvelope>();

        envelope.Payload.QrCode.Should().NotBeNull();
        envelope.Payload.QrCode.MediaType.Should().Be("image/png");
        envelope.Payload.QrCode.Base64Data.Should().NotBeNullOrEmpty();

        envelope.Metadata.Should().Be(metadata);
        envelope.ReceivedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task PairSuccessEvent_ShouldDeserializeSuccessfully()
    {
        var bytes = await LoadAsset("PairSuccess-1.json");
        var metadata = WuzEventMetadata.Parse(bytes);
        var envelope = metadata.ToEnvelope<PairSuccessEventEnvelope>();

        envelope.Payload.Event.Should().NotBeNull();
        envelope.Metadata.Should().Be(metadata);
        envelope.ReceivedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("Message-1.json")]
    [InlineData("Message-2.json")]
    [InlineData("Message-3.json")]
    public async Task MessageEvent_ShouldDeserializeMessageInfo(string fileName)
    {
        var bytes = await LoadAsset(fileName);
        var metadata = WuzEventMetadata.Parse(bytes);
        var envelope = metadata.ToEnvelope<MessageEventEnvelope>();

        envelope.Payload.Event.Info.Should().NotBeNull();
        envelope.Payload.Event.Info!.Chat.Should().NotBeNull();
        envelope.Payload.Event.Info.Chat!.Value.Value.Should().NotBeNullOrEmpty();
        envelope.Payload.Event.Info.Sender.Should().NotBeNull();
        envelope.Payload.Event.Info.Sender!.Value.Value.Should().NotBeNullOrEmpty();
        envelope.Payload.Event.Info.Id.Should().NotBeNullOrEmpty();
        envelope.Payload.Event.Info.Timestamp.Should().NotBe(default);

        envelope.Metadata.Should().Be(metadata);
        envelope.ReceivedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("UndecryptableMessage-1.json")]
    [InlineData("UndecryptableMessage-2.json")]
    [InlineData("UndecryptableMessage-3.json")]
    public async Task UndecryptableMessageEvent_ShouldDeserializeMessageInfo(string fileName)
    {
        var bytes = await LoadAsset(fileName);
        var metadata = WuzEventMetadata.Parse(bytes);
        var envelope = metadata.ToEnvelope<UndecryptableMessageEventEnvelope>();

        envelope.Payload.Event.Info.Should().NotBeNull();
        envelope.Payload.Event.Info!.Chat.Should().NotBeNull();
        envelope.Payload.Event.Info.Chat!.Value.Value.Should().NotBeNullOrEmpty();
        envelope.Payload.Event.Info.Sender.Should().NotBeNull();
        envelope.Payload.Event.Info.Sender!.Value.Value.Should().NotBeNullOrEmpty();
        envelope.Payload.Event.Info.Id.Should().NotBeNullOrEmpty();

        envelope.Metadata.Should().Be(metadata);
        envelope.ReceivedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ReceiptEvent_ShouldDeserializeReceiptData()
    {
        var bytes = await LoadAsset("ReadReceipt-1.json");
        var metadata = WuzEventMetadata.Parse(bytes);
        var envelope = metadata.ToEnvelope<ReceiptEventEnvelope>();

        envelope.Payload.Event.Chat.Should().NotBeNull();
        envelope.Payload.Event.Sender.Should().NotBeNull();
        envelope.Payload.Event.MessageIDs.Should().NotBeNullOrEmpty();
        envelope.Payload.Event.MessageIDs!.Should().Contain(id => !string.IsNullOrEmpty(id));
        envelope.Payload.State.Should().Be("Delivered");

        envelope.Metadata.Should().Be(metadata);
        envelope.ReceivedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task HistorySyncEvent_ShouldDeserializeSuccessfully()
    {
        var bytes = await LoadAsset("HistorySync-1.json");
        var metadata = WuzEventMetadata.Parse(bytes);
        var envelope = metadata.ToEnvelope<HistorySyncEventEnvelope>();

        envelope.Payload.Event.Should().NotBeNull();
        envelope.Metadata.Should().Be(metadata);
        envelope.ReceivedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    // === Helper Methods ===

    private static async Task<byte[]> LoadAsset(string fileName) =>
        await File.ReadAllBytesAsync(Path.Combine(AppContext.BaseDirectory, "Assets", fileName));
}

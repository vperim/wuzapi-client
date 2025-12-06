using System.Text.Json;
using AwesomeAssertions;
using WuzApiClient.Common.Enums;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.Common.UnitTests.Serialization;

[Trait("Category", "Unit")]
public sealed class WhatsAppEventTypeConverterTests
{
    private readonly JsonSerializerOptions options;

    public WhatsAppEventTypeConverterTests()
    {
        this.options = new JsonSerializerOptions();
        this.options.Converters.Add(new WhatsAppEventTypeConverter());
    }

    [Theory]
    [InlineData("Message", WhatsAppEventType.Message)]
    [InlineData("UndecryptableMessage", WhatsAppEventType.UndecryptableMessage)]
    [InlineData("Receipt", WhatsAppEventType.Receipt)]
    [InlineData("MediaRetry", WhatsAppEventType.MediaRetry)]
    [InlineData("ReadReceipt", WhatsAppEventType.ReadReceipt)]
    [InlineData("GroupInfo", WhatsAppEventType.GroupInfo)]
    [InlineData("JoinedGroup", WhatsAppEventType.JoinedGroup)]
    [InlineData("Picture", WhatsAppEventType.Picture)]
    [InlineData("BlocklistChange", WhatsAppEventType.BlocklistChange)]
    [InlineData("Blocklist", WhatsAppEventType.Blocklist)]
    [InlineData("Connected", WhatsAppEventType.Connected)]
    [InlineData("Disconnected", WhatsAppEventType.Disconnected)]
    [InlineData("ConnectFailure", WhatsAppEventType.ConnectFailure)]
    [InlineData("KeepAliveRestored", WhatsAppEventType.KeepAliveRestored)]
    [InlineData("KeepAliveTimeout", WhatsAppEventType.KeepAliveTimeout)]
    [InlineData("QRTimeout", WhatsAppEventType.QRTimeout)]
    [InlineData("LoggedOut", WhatsAppEventType.LoggedOut)]
    [InlineData("ClientOutdated", WhatsAppEventType.ClientOutdated)]
    [InlineData("TemporaryBan", WhatsAppEventType.TemporaryBan)]
    [InlineData("StreamError", WhatsAppEventType.StreamError)]
    [InlineData("StreamReplaced", WhatsAppEventType.StreamReplaced)]
    [InlineData("PairSuccess", WhatsAppEventType.PairSuccess)]
    [InlineData("PairError", WhatsAppEventType.PairError)]
    [InlineData("QR", WhatsAppEventType.QR)]
    [InlineData("QRScannedWithoutMultidevice", WhatsAppEventType.QRScannedWithoutMultidevice)]
    [InlineData("PrivacySettings", WhatsAppEventType.PrivacySettings)]
    [InlineData("PushNameSetting", WhatsAppEventType.PushNameSetting)]
    [InlineData("UserAbout", WhatsAppEventType.UserAbout)]
    [InlineData("AppState", WhatsAppEventType.AppState)]
    [InlineData("AppStateSyncComplete", WhatsAppEventType.AppStateSyncComplete)]
    [InlineData("HistorySync", WhatsAppEventType.HistorySync)]
    [InlineData("OfflineSyncCompleted", WhatsAppEventType.OfflineSyncCompleted)]
    [InlineData("OfflineSyncPreview", WhatsAppEventType.OfflineSyncPreview)]
    [InlineData("CallOffer", WhatsAppEventType.CallOffer)]
    [InlineData("CallAccept", WhatsAppEventType.CallAccept)]
    [InlineData("CallTerminate", WhatsAppEventType.CallTerminate)]
    [InlineData("CallOfferNotice", WhatsAppEventType.CallOfferNotice)]
    [InlineData("CallRelayLatency", WhatsAppEventType.CallRelayLatency)]
    [InlineData("Presence", WhatsAppEventType.Presence)]
    [InlineData("ChatPresence", WhatsAppEventType.ChatPresence)]
    [InlineData("IdentityChange", WhatsAppEventType.IdentityChange)]
    [InlineData("CATRefreshError", WhatsAppEventType.CATRefreshError)]
    [InlineData("NewsletterJoin", WhatsAppEventType.NewsletterJoin)]
    [InlineData("NewsletterLeave", WhatsAppEventType.NewsletterLeave)]
    [InlineData("NewsletterMuteChange", WhatsAppEventType.NewsletterMuteChange)]
    [InlineData("NewsletterLiveUpdate", WhatsAppEventType.NewsletterLiveUpdate)]
    [InlineData("FBMessage", WhatsAppEventType.FBMessage)]
    public void RoundTrip_AllEventTypes_PreservesValue(string stringValue, WhatsAppEventType enumValue)
    {
        var serialized = JsonSerializer.Serialize(enumValue, this.options);
        var deserialized = JsonSerializer.Deserialize<WhatsAppEventType>(serialized, this.options);

        serialized.Should().Be($"\"{stringValue}\"");
        deserialized.Should().Be(enumValue);
    }
}

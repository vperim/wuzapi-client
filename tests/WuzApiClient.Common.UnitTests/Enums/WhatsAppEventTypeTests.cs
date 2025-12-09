using AwesomeAssertions;
using WuzApiClient.Common.Enums;

namespace WuzApiClient.Common.UnitTests.Enums;

/// <summary>
/// Tests for WhatsAppEventType enum to ensure 1:1 mapping with wuzapi string values.
/// These tests prevent regressions by verifying that enum member names match exactly
/// the string values expected by wuzapi/constants.go.
/// </summary>
[Trait("Category", "Unit")]
public sealed class WhatsAppEventTypeTests
{
    /// <summary>
    /// All event types from wuzapi/constants.go supportedEventTypes array.
    /// Source: https://github.com/asternic/wuzapi/blob/main/constants.go
    /// </summary>
    private static readonly string[] WuzApiEventTypes =
    [
        // Messages and Communication
        "Message",
        "UndecryptableMessage",
        "Receipt",
        "MediaRetry",
        "ReadReceipt",

        // Groups and Contacts
        "GroupInfo",
        "JoinedGroup",
        "Picture",
        "BlocklistChange",
        "Blocklist",

        // Connection and Session
        "Connected",
        "Disconnected",
        "ConnectFailure",
        "KeepAliveRestored",
        "KeepAliveTimeout",
        "QRTimeout",
        "LoggedOut",
        "ClientOutdated",
        "TemporaryBan",
        "StreamError",
        "StreamReplaced",
        "PairSuccess",
        "PairError",
        "QR",
        "QRScannedWithoutMultidevice",

        // Privacy and Settings
        "PrivacySettings",
        "PushNameSetting",
        "UserAbout",

        // Synchronization and State
        "AppState",
        "AppStateSyncComplete",
        "HistorySync",
        "OfflineSyncCompleted",
        "OfflineSyncPreview",

        // Calls
        "CallOffer",
        "CallAccept",
        "CallTerminate",
        "CallOfferNotice",
        "CallRelayLatency",

        // Presence and Activity
        "Presence",
        "ChatPresence",

        // Identity
        "IdentityChange",

        // Errors
        "CATRefreshError",

        // Newsletter (WhatsApp Channels)
        "NewsletterJoin",
        "NewsletterLeave",
        "NewsletterMuteChange",
        "NewsletterLiveUpdate",

        // Facebook/Meta Bridge
        "FBMessage",

        // Special
        "All"
    ];

    [Fact]
    public void AllWuzApiEventTypes_ExistInEnum()
    {
        foreach (var eventType in WuzApiEventTypes)
        {
            var parsed = Enum.TryParse<WhatsAppEventType>(eventType, out var result);

            parsed.Should().BeTrue($"wuzapi event type '{eventType}' should exist in WhatsAppEventType enum");
            result.Should().NotBe(WhatsAppEventType.Unknown, $"'{eventType}' should not map to Unknown");
        }
    }

    [Fact]
    public void AllEnumValues_ExceptUnknown_MatchWuzApiEventTypes()
    {
        var enumValues = Enum.GetValues<WhatsAppEventType>()
            .Where(e => e != WhatsAppEventType.Unknown)
            .ToArray();

        enumValues.Length.Should().Be(WuzApiEventTypes.Length,
            "Enum should have same number of values as wuzapi (excluding Unknown)");

        foreach (var enumValue in enumValues)
        {
            var stringValue = enumValue.ToString();
            WuzApiEventTypes.Should().Contain(stringValue,
                $"Enum value '{stringValue}' should exist in wuzapi constants");
        }
    }

    [Theory]
    [InlineData(WhatsAppEventType.Message, "Message")]
    [InlineData(WhatsAppEventType.QR, "QR")]
    [InlineData(WhatsAppEventType.Receipt, "Receipt")]
    [InlineData(WhatsAppEventType.Connected, "Connected")]
    [InlineData(WhatsAppEventType.All, "All")]
    [InlineData(WhatsAppEventType.Unknown, "Unknown")]
    public void ToString_ReturnsExactEnumName(WhatsAppEventType eventType, string expected)
    {
        eventType.ToString().Should().Be(expected,
            "ToString() should return the exact enum member name");
    }

    [Theory]
    [InlineData("Message", WhatsAppEventType.Message)]
    [InlineData("QR", WhatsAppEventType.QR)]
    [InlineData("Receipt", WhatsAppEventType.Receipt)]
    [InlineData("Connected", WhatsAppEventType.Connected)]
    [InlineData("All", WhatsAppEventType.All)]
    public void EnumParse_ValidString_ReturnsCorrectEnum(string input, WhatsAppEventType expected)
    {
        var parsed = Enum.TryParse<WhatsAppEventType>(input, out var result);

        parsed.Should().BeTrue($"'{input}' should parse successfully");
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("InvalidType")]
    [InlineData("message")] // lowercase - case sensitive
    [InlineData("MESSAGE")] // uppercase - case sensitive
    [InlineData("")]
    public void EnumParse_InvalidString_ReturnsFalse(string input)
    {
        var parsed = Enum.TryParse<WhatsAppEventType>(input, out _);

        parsed.Should().BeFalse($"'{input}' should not parse to a valid enum value");
    }

    [Fact]
    public void EnumParse_IsCaseSensitive()
    {
        // wuzapi uses case-sensitive strings (e.g., "Message", not "message")
        var lowercaseParsed = Enum.TryParse<WhatsAppEventType>("message", out _);
        var correctParsed = Enum.TryParse<WhatsAppEventType>("Message", out var result);

        lowercaseParsed.Should().BeFalse("Lowercase 'message' should not parse (case-sensitive)");
        correctParsed.Should().BeTrue("Correct case 'Message' should parse");
        result.Should().Be(WhatsAppEventType.Message);
    }

    [Fact]
    public void Unknown_IsFirstEnumValue()
    {
        // Unknown should be the first value (0) for default initialization
        ((int)WhatsAppEventType.Unknown).Should().Be(0,
            "Unknown should be the default enum value (0)");
    }
}

using System;
using System.Collections.Generic;

namespace WuzApiClient.RabbitMq;

/// <summary>
/// Constants for all supported event types.
/// </summary>
public static class EventTypes
{
    /// <summary>Incoming WhatsApp message.</summary>
    public const string Message = "Message";

    /// <summary>Message that could not be decrypted.</summary>
    public const string UndecryptableMessage = "UndecryptableMessage";

    /// <summary>
    /// Message delivery receipt.
    /// NOTE: Due to a wuzapi quirk, all receipt events are sent as "ReadReceipt" type.
    /// This constant matches wuzapi/constants.go but subscribing to it has no effect.
    /// Use ReadReceipt instead for filtering, or subscribe to All.
    /// </summary>
    public const string Receipt = "Receipt";

    /// <summary>Message read/delivery/played receipt (actual event type sent by wuzapi).</summary>
    public const string ReadReceipt = "ReadReceipt";

    /// <summary>User presence update (online/offline).</summary>
    public const string Presence = "Presence";

    /// <summary>Chat typing/recording indicator.</summary>
    public const string ChatPresence = "ChatPresence";

    /// <summary>Connection established successfully.</summary>
    public const string Connected = "Connected";

    /// <summary>Connection lost.</summary>
    public const string Disconnected = "Disconnected";

    /// <summary>QR code generated for pairing.</summary>
    public const string Qr = "QR";

    /// <summary>QR code expired without successful scan.</summary>
    public const string QrTimeout = "QRTimeout";

    /// <summary>QR code scanned but multidevice not enabled on phone.</summary>
    public const string QRScannedWithoutMultidevice = "QRScannedWithoutMultidevice";

    /// <summary>Phone pairing completed successfully.</summary>
    public const string PairSuccess = "PairSuccess";

    /// <summary>Phone pairing failed.</summary>
    public const string PairError = "PairError";

    /// <summary>User was logged out.</summary>
    public const string LoggedOut = "LoggedOut";

    /// <summary>Connection attempt failed.</summary>
    public const string ConnectFailure = "ConnectFailure";

    /// <summary>Client version is outdated.</summary>
    public const string ClientOutdated = "ClientOutdated";

    /// <summary>Account is temporarily banned.</summary>
    public const string TemporaryBan = "TemporaryBan";

    /// <summary>Stream error occurred.</summary>
    public const string StreamError = "StreamError";

    /// <summary>Stream was replaced by another connection.</summary>
    public const string StreamReplaced = "StreamReplaced";

    /// <summary>Keep-alive timeout occurred.</summary>
    public const string KeepAliveTimeout = "KeepAliveTimeout";

    /// <summary>Keep-alive connection restored.</summary>
    public const string KeepAliveRestored = "KeepAliveRestored";

    /// <summary>Incoming call offer received.</summary>
    public const string CallOffer = "CallOffer";

    /// <summary>Call was accepted.</summary>
    public const string CallAccept = "CallAccept";

    /// <summary>Call was terminated.</summary>
    public const string CallTerminate = "CallTerminate";

    /// <summary>Call offer notification.</summary>
    public const string CallOfferNotice = "CallOfferNotice";

    /// <summary>Call relay latency update.</summary>
    public const string CallRelayLatency = "CallRelayLatency";

    /// <summary>Group information updated.</summary>
    public const string GroupInfo = "GroupInfo";

    /// <summary>User joined a group.</summary>
    public const string JoinedGroup = "JoinedGroup";

    /// <summary>Profile or group picture updated.</summary>
    public const string Picture = "Picture";

    /// <summary>History synchronization progress.</summary>
    public const string HistorySync = "HistorySync";

    /// <summary>App state update.</summary>
    public const string AppState = "AppState";

    /// <summary>App state synchronization completed.</summary>
    public const string AppStateSyncComplete = "AppStateSyncComplete";

    /// <summary>Offline synchronization completed.</summary>
    public const string OfflineSyncCompleted = "OfflineSyncCompleted";

    /// <summary>Offline synchronization preview available.</summary>
    public const string OfflineSyncPreview = "OfflineSyncPreview";

    /// <summary>Privacy settings updated.</summary>
    public const string PrivacySettings = "PrivacySettings";

    /// <summary>Push name (display name) updated.</summary>
    public const string PushNameSetting = "PushNameSetting";

    /// <summary>Blocklist was changed.</summary>
    public const string BlocklistChange = "BlocklistChange";

    /// <summary>Blocklist retrieved.</summary>
    public const string Blocklist = "Blocklist";

    /// <summary>Contact identity changed.</summary>
    public const string IdentityChange = "IdentityChange";

    /// <summary>User joined a newsletter.</summary>
    public const string NewsletterJoin = "NewsletterJoin";

    /// <summary>User left a newsletter.</summary>
    public const string NewsletterLeave = "NewsletterLeave";

    /// <summary>Newsletter mute state changed.</summary>
    public const string NewsletterMuteChange = "NewsletterMuteChange";

    /// <summary>Newsletter live update received.</summary>
    public const string NewsletterLiveUpdate = "NewsletterLiveUpdate";

    /// <summary>Media download retry requested.</summary>
    public const string MediaRetry = "MediaRetry";

    /// <summary>User about (status message) updated.</summary>
    public const string UserAbout = "UserAbout";

    /// <summary>CAT (Client Access Token) refresh error.</summary>
    public const string CATRefreshError = "CATRefreshError";

    /// <summary>Facebook/Meta integration message.</summary>
    public const string FbMessage = "FBMessage";

    /// <summary>
    /// All supported event types (immutable).
    /// </summary>
    public static readonly IReadOnlyList<string> All = Array.AsReadOnly([
        Message,
        UndecryptableMessage,
        Receipt,
        ReadReceipt,
        Presence,
        ChatPresence,
        Connected,
        Disconnected,
        Qr,
        QrTimeout,
        QRScannedWithoutMultidevice,
        PairSuccess,
        PairError,
        LoggedOut,
        ConnectFailure,
        ClientOutdated,
        TemporaryBan,
        StreamError,
        StreamReplaced,
        KeepAliveTimeout,
        KeepAliveRestored,
        CallOffer,
        CallAccept,
        CallTerminate,
        CallOfferNotice,
        CallRelayLatency,
        GroupInfo,
        JoinedGroup,
        Picture,
        HistorySync,
        AppState,
        AppStateSyncComplete,
        OfflineSyncCompleted,
        OfflineSyncPreview,
        PrivacySettings,
        PushNameSetting,
        BlocklistChange,
        Blocklist,
        IdentityChange,
        NewsletterJoin,
        NewsletterLeave,
        NewsletterMuteChange,
        NewsletterLiveUpdate,
        MediaRetry,
        UserAbout,
        CATRefreshError,
        FbMessage
    ]);
}

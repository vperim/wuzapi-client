namespace WuzApiClient.Common.Enums;

/// <summary>
/// Event types that can be subscribed to when connecting a session.
/// Enum member names match wuzapi's expected string values exactly (case-sensitive).
/// Source: wuzapi/constants.go supportedEventTypes
/// </summary>
public enum WhatsAppEventType
{
    Unknown,

    // ==================== Special ====================

    /// <summary>Subscribe to all events.</summary>
    All,

    // ==================== Messages and Communication ====================

    /// <summary>Incoming WhatsApp message.</summary>
    Message,

    /// <summary>Message that could not be decrypted.</summary>
    UndecryptableMessage,

    /// <summary>
    /// Message delivery receipt.
    /// WARNING: wuzapi has a bug where all receipts are sent as "ReadReceipt" type.
    /// Subscribing to this event alone will not receive any events. Use ReadReceipt instead,
    /// or subscribe to All. Included for API completeness with wuzapi/constants.go.
    /// </summary>
    Receipt,

    /// <summary>Media download retry requested.</summary>
    MediaRetry,

    /// <summary>Message read receipt.</summary>
    ReadReceipt,

    // ==================== Groups and Contacts ====================

    /// <summary>Group information updated.</summary>
    GroupInfo,

    /// <summary>User joined a group.</summary>
    JoinedGroup,

    /// <summary>Profile or group picture updated.</summary>
    Picture,

    /// <summary>Blocklist was changed.</summary>
    BlocklistChange,

    /// <summary>Blocklist retrieved.</summary>
    Blocklist,

    // ==================== Connection and Session ====================

    /// <summary>Connection established successfully.</summary>
    Connected,

    /// <summary>Connection lost.</summary>
    Disconnected,

    /// <summary>Connection attempt failed.</summary>
    ConnectFailure,

    /// <summary>Keep-alive connection restored.</summary>
    KeepAliveRestored,

    /// <summary>Keep-alive timeout occurred.</summary>
    KeepAliveTimeout,

    /// <summary>QR code expired without successful scan.</summary>
    QRTimeout,

    /// <summary>User was logged out.</summary>
    LoggedOut,

    /// <summary>Client version is outdated.</summary>
    ClientOutdated,

    /// <summary>Account is temporarily banned.</summary>
    TemporaryBan,

    /// <summary>Stream error occurred.</summary>
    StreamError,

    /// <summary>Stream was replaced by another connection.</summary>
    StreamReplaced,

    /// <summary>Phone pairing completed successfully.</summary>
    PairSuccess,

    /// <summary>Phone pairing failed.</summary>
    PairError,

    /// <summary>QR code generated for pairing.</summary>
    QR,

    /// <summary>QR code scanned but multidevice not enabled on phone.</summary>
    QRScannedWithoutMultidevice,

    // ==================== Privacy and Settings ====================

    /// <summary>Privacy settings updated.</summary>
    PrivacySettings,

    /// <summary>Push name (display name) updated.</summary>
    PushNameSetting,

    /// <summary>User about (status message) updated.</summary>
    UserAbout,

    // ==================== Synchronization and State ====================

    /// <summary>App state update.</summary>
    AppState,

    /// <summary>App state synchronization completed.</summary>
    AppStateSyncComplete,

    /// <summary>History synchronization progress.</summary>
    HistorySync,

    /// <summary>Offline synchronization completed.</summary>
    OfflineSyncCompleted,

    /// <summary>Offline synchronization preview available.</summary>
    OfflineSyncPreview,

    // ==================== Calls ====================

    /// <summary>Incoming call offer received.</summary>
    CallOffer,

    /// <summary>Call was accepted.</summary>
    CallAccept,

    /// <summary>Call was terminated.</summary>
    CallTerminate,

    /// <summary>Call offer notification.</summary>
    CallOfferNotice,

    /// <summary>Call relay latency update.</summary>
    CallRelayLatency,

    // ==================== Presence and Activity ====================

    /// <summary>User presence update (online/offline).</summary>
    Presence,

    /// <summary>Chat typing/recording indicator.</summary>
    ChatPresence,

    // ==================== Identity ====================

    /// <summary>Contact identity changed.</summary>
    IdentityChange,

    // ==================== Errors ====================

    /// <summary>CAT (Client Access Token) refresh error.</summary>
    CATRefreshError,

    // ==================== Newsletter (WhatsApp Channels) ====================

    /// <summary>User joined a newsletter/channel.</summary>
    NewsletterJoin,

    /// <summary>User left a newsletter/channel.</summary>
    NewsletterLeave,

    /// <summary>Newsletter mute state changed.</summary>
    NewsletterMuteChange,

    /// <summary>Newsletter live update received.</summary>
    NewsletterLiveUpdate,

    // ==================== Facebook/Meta Bridge ====================

    /// <summary>Facebook/Meta integration message.</summary>
    FBMessage
}

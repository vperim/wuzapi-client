using System;
using System.Collections.Generic;
using WuzApiClient.RabbitMq.Models.Events;

namespace WuzApiClient.RabbitMq.Serialization;

/// <summary>
/// Maps wuzapi event type strings to concrete C# event types.
/// </summary>
internal static class EventTypeResolver
{
    /// <summary>
    /// Static dictionary mapping event type strings to CLR types.
    /// Uses ordinal comparison for exact string matching.
    /// </summary>
    private static readonly Dictionary<string, Type> TypeMap = new(StringComparer.Ordinal)
    {
        [EventTypes.Message] = typeof(MessageEvent),
        [EventTypes.UndecryptableMessage] = typeof(UndecryptableMessageEvent),
        [EventTypes.Receipt] = typeof(ReceiptEvent),
        [EventTypes.ReadReceipt] = typeof(ReceiptEvent),
        [EventTypes.Presence] = typeof(PresenceEvent),
        [EventTypes.ChatPresence] = typeof(ChatPresenceEvent),
        [EventTypes.Connected] = typeof(ConnectedEvent),
        [EventTypes.Disconnected] = typeof(DisconnectedEvent),
        [EventTypes.Qr] = typeof(QrCodeEvent),
        [EventTypes.QrTimeout] = typeof(QrTimeoutEvent),
        [EventTypes.QrScannedWithoutMultidevice] = typeof(QrScannedWithoutMultideviceEvent),
        [EventTypes.PairSuccess] = typeof(PairSuccessEvent),
        [EventTypes.PairError] = typeof(PairErrorEvent),
        [EventTypes.LoggedOut] = typeof(LoggedOutEvent),
        [EventTypes.ConnectFailure] = typeof(ConnectFailureEvent),
        [EventTypes.ClientOutdated] = typeof(ClientOutdatedEvent),
        [EventTypes.TemporaryBan] = typeof(TemporaryBanEvent),
        [EventTypes.StreamError] = typeof(StreamErrorEvent),
        [EventTypes.StreamReplaced] = typeof(StreamReplacedEvent),
        [EventTypes.KeepAliveTimeout] = typeof(KeepAliveTimeoutEvent),
        [EventTypes.KeepAliveRestored] = typeof(KeepAliveRestoredEvent),
        [EventTypes.CallOffer] = typeof(CallOfferEvent),
        [EventTypes.CallAccept] = typeof(CallAcceptEvent),
        [EventTypes.CallTerminate] = typeof(CallTerminateEvent),
        [EventTypes.CallOfferNotice] = typeof(CallOfferNoticeEvent),
        [EventTypes.CallRelayLatency] = typeof(CallRelayLatencyEvent),
        [EventTypes.GroupInfo] = typeof(GroupInfoEvent),
        [EventTypes.JoinedGroup] = typeof(JoinedGroupEvent),
        [EventTypes.Picture] = typeof(PictureEvent),
        [EventTypes.HistorySync] = typeof(HistorySyncEvent),
        [EventTypes.AppState] = typeof(AppStateEvent),
        [EventTypes.AppStateSyncComplete] = typeof(AppStateSyncCompleteEvent),
        [EventTypes.OfflineSyncCompleted] = typeof(OfflineSyncCompletedEvent),
        [EventTypes.OfflineSyncPreview] = typeof(OfflineSyncPreviewEvent),
        [EventTypes.PrivacySettings] = typeof(PrivacySettingsEvent),
        [EventTypes.PushNameSetting] = typeof(PushNameSettingEvent),
        [EventTypes.BlocklistChange] = typeof(BlocklistChangeEvent),
        [EventTypes.Blocklist] = typeof(BlocklistEvent),
        [EventTypes.IdentityChange] = typeof(IdentityChangeEvent),
        [EventTypes.NewsletterJoin] = typeof(NewsletterJoinEvent),
        [EventTypes.NewsletterLeave] = typeof(NewsletterLeaveEvent),
        [EventTypes.NewsletterMuteChange] = typeof(NewsletterMuteChangeEvent),
        [EventTypes.NewsletterLiveUpdate] = typeof(NewsletterLiveUpdateEvent),
        [EventTypes.MediaRetry] = typeof(MediaRetryEvent),
        [EventTypes.UserAbout] = typeof(UserAboutEvent),
        [EventTypes.FbMessage] = typeof(FbMessageEvent),
        [EventTypes.CatRefreshError] = typeof(CatRefreshErrorEvent),
    };

    /// <summary>
    /// Resolves an event type string to its corresponding CLR type.
    /// </summary>
    /// <param name="eventType">The wuzapi event type string.</param>
    /// <returns>The CLR type, or null if unknown.</returns>
    public static Type? Resolve(string eventType)
    {
        return TypeMap.TryGetValue(eventType, out var type) ? type : null;
    }

    /// <summary>
    /// Gets all registered event types for diagnostics.
    /// </summary>
    public static IReadOnlyCollection<string> GetRegisteredTypes()
    {
        return TypeMap.Keys;
    }
}

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
        [EventTypes.Message] = typeof(MessageEventEnvelope),
        [EventTypes.UndecryptableMessage] = typeof(UndecryptableMessageEventEnvelope),
        [EventTypes.Receipt] = typeof(ReceiptEventEnvelope),
        [EventTypes.ReadReceipt] = typeof(ReceiptEventEnvelope),
        [EventTypes.Presence] = typeof(PresenceEventEnvelope),
        [EventTypes.ChatPresence] = typeof(ChatPresenceEventEnvelope),
        [EventTypes.Connected] = typeof(ConnectedEventEnvelope),
        [EventTypes.Disconnected] = typeof(DisconnectedEventEnvelope),
        [EventTypes.Qr] = typeof(QrCodeEventEnvelope),
        [EventTypes.QrTimeout] = typeof(QrTimeoutEventEnvelope),
        [EventTypes.QrScannedWithoutMultidevice] = typeof(QrScannedWithoutMultideviceEventEnvelope),
        [EventTypes.PairSuccess] = typeof(PairSuccessEventEnvelope),
        [EventTypes.PairError] = typeof(PairErrorEventEnvelope),
        [EventTypes.LoggedOut] = typeof(LoggedOutEventEnvelope),
        [EventTypes.ConnectFailure] = typeof(ConnectFailureEventEnvelope),
        [EventTypes.ClientOutdated] = typeof(ClientOutdatedEventEnvelope),
        [EventTypes.TemporaryBan] = typeof(TemporaryBanEventEnvelope),
        [EventTypes.StreamError] = typeof(StreamErrorEventEnvelope),
        [EventTypes.StreamReplaced] = typeof(StreamReplacedEventEnvelope),
        [EventTypes.KeepAliveTimeout] = typeof(KeepAliveTimeoutEventEnvelope),
        [EventTypes.KeepAliveRestored] = typeof(KeepAliveRestoredEventEnvelope),
        [EventTypes.CallOffer] = typeof(CallOfferEventEnvelope),
        [EventTypes.CallAccept] = typeof(CallAcceptEventEnvelope),
        [EventTypes.CallTerminate] = typeof(CallTerminateEventEnvelope),
        [EventTypes.CallOfferNotice] = typeof(CallOfferNoticeEventEnvelope),
        [EventTypes.CallRelayLatency] = typeof(CallRelayLatencyEventEnvelope),
        [EventTypes.GroupInfo] = typeof(GroupInfoEventEnvelope),
        [EventTypes.JoinedGroup] = typeof(JoinedGroupEventEnvelope),
        [EventTypes.Picture] = typeof(PictureEventEnvelope),
        [EventTypes.HistorySync] = typeof(HistorySyncEventEnvelope),
        [EventTypes.AppState] = typeof(AppStateEventEnvelope),
        [EventTypes.AppStateSyncComplete] = typeof(AppStateSyncCompleteEventEnvelope),
        [EventTypes.OfflineSyncCompleted] = typeof(OfflineSyncCompletedEventEnvelope),
        [EventTypes.OfflineSyncPreview] = typeof(OfflineSyncPreviewEventEnvelope),
        [EventTypes.PrivacySettings] = typeof(PrivacySettingsEventEnvelope),
        [EventTypes.PushNameSetting] = typeof(PushNameSettingEventEnvelope),
        [EventTypes.BlocklistChange] = typeof(BlocklistChangeEventEnvelope),
        [EventTypes.Blocklist] = typeof(BlocklistEventEnvelope),
        [EventTypes.IdentityChange] = typeof(IdentityChangeEventEnvelope),
        [EventTypes.NewsletterJoin] = typeof(NewsletterJoinEventEnvelope),
        [EventTypes.NewsletterLeave] = typeof(NewsletterLeaveEventEnvelope),
        [EventTypes.NewsletterMuteChange] = typeof(NewsletterMuteChangeEventEnvelope),
        [EventTypes.NewsletterLiveUpdate] = typeof(NewsletterLiveUpdateEventEnvelope),
        [EventTypes.MediaRetry] = typeof(MediaRetryEventEnvelope),
        [EventTypes.UserAbout] = typeof(UserAboutEventEnvelope),
        [EventTypes.FbMessage] = typeof(FbMessageEventEnvelope),
        [EventTypes.CatRefreshError] = typeof(CatRefreshErrorEventEnvelope),
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

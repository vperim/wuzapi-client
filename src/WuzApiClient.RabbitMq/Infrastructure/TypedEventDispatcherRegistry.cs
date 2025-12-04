using System;
using System.Collections.Generic;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models;
using WuzApiClient.RabbitMq.Models.Events;

namespace WuzApiClient.RabbitMq.Infrastructure;

/// <summary>
/// Singleton registry of typed event dispatchers.
/// Registered in DI container at startup.
/// </summary>
public sealed class TypedEventDispatcherRegistry : ITypedEventDispatcherRegistry
{
    private readonly Dictionary<string, ITypedEventDispatcher> dispatchers;
    private readonly ITypedEventDispatcher unknownDispatcher;

    public TypedEventDispatcherRegistry()
    {
        this.unknownDispatcher = new TypedEventDispatcher<UnknownEventData>();

        this.dispatchers = new Dictionary<string, ITypedEventDispatcher>(StringComparer.Ordinal)
        {
            [EventTypes.Message] = new TypedEventDispatcher<MessageEvent>(),
            [EventTypes.UndecryptableMessage] = new TypedEventDispatcher<UndecryptableMessageEvent>(),
            [EventTypes.Receipt] = new TypedEventDispatcher<ReceiptEvent>(),
            [EventTypes.ReadReceipt] = new TypedEventDispatcher<ReceiptEvent>(),
            [EventTypes.Presence] = new TypedEventDispatcher<PresenceEvent>(),
            [EventTypes.ChatPresence] = new TypedEventDispatcher<ChatPresenceEvent>(),
            [EventTypes.Connected] = new TypedEventDispatcher<ConnectedEvent>(),
            [EventTypes.Disconnected] = new TypedEventDispatcher<DisconnectedEvent>(),
            [EventTypes.Qr] = new TypedEventDispatcher<QrCodeEvent>(),
            [EventTypes.QrTimeout] = new TypedEventDispatcher<QrTimeoutEvent>(),
            [EventTypes.QrScannedWithoutMultidevice] = new TypedEventDispatcher<QrScannedWithoutMultideviceEvent>(),
            [EventTypes.PairSuccess] = new TypedEventDispatcher<PairSuccessEvent>(),
            [EventTypes.PairError] = new TypedEventDispatcher<PairErrorEvent>(),
            [EventTypes.LoggedOut] = new TypedEventDispatcher<LoggedOutEvent>(),
            [EventTypes.ConnectFailure] = new TypedEventDispatcher<ConnectFailureEvent>(),
            [EventTypes.ClientOutdated] = new TypedEventDispatcher<ClientOutdatedEvent>(),
            [EventTypes.TemporaryBan] = new TypedEventDispatcher<TemporaryBanEvent>(),
            [EventTypes.StreamError] = new TypedEventDispatcher<StreamErrorEvent>(),
            [EventTypes.StreamReplaced] = new TypedEventDispatcher<StreamReplacedEvent>(),
            [EventTypes.KeepAliveTimeout] = new TypedEventDispatcher<KeepAliveTimeoutEvent>(),
            [EventTypes.KeepAliveRestored] = new TypedEventDispatcher<KeepAliveRestoredEvent>(),
            [EventTypes.CallOffer] = new TypedEventDispatcher<CallOfferEvent>(),
            [EventTypes.CallAccept] = new TypedEventDispatcher<CallAcceptEvent>(),
            [EventTypes.CallTerminate] = new TypedEventDispatcher<CallTerminateEvent>(),
            [EventTypes.CallOfferNotice] = new TypedEventDispatcher<CallOfferNoticeEvent>(),
            [EventTypes.CallRelayLatency] = new TypedEventDispatcher<CallRelayLatencyEvent>(),
            [EventTypes.GroupInfo] = new TypedEventDispatcher<GroupInfoEvent>(),
            [EventTypes.JoinedGroup] = new TypedEventDispatcher<JoinedGroupEvent>(),
            [EventTypes.Picture] = new TypedEventDispatcher<PictureEvent>(),
            [EventTypes.HistorySync] = new TypedEventDispatcher<HistorySyncEvent>(),
            [EventTypes.AppState] = new TypedEventDispatcher<AppStateEvent>(),
            [EventTypes.AppStateSyncComplete] = new TypedEventDispatcher<AppStateSyncCompleteEvent>(),
            [EventTypes.OfflineSyncCompleted] = new TypedEventDispatcher<OfflineSyncCompletedEvent>(),
            [EventTypes.OfflineSyncPreview] = new TypedEventDispatcher<OfflineSyncPreviewEvent>(),
            [EventTypes.PrivacySettings] = new TypedEventDispatcher<PrivacySettingsEvent>(),
            [EventTypes.PushNameSetting] = new TypedEventDispatcher<PushNameSettingEvent>(),
            [EventTypes.BlocklistChange] = new TypedEventDispatcher<BlocklistChangeEvent>(),
            [EventTypes.Blocklist] = new TypedEventDispatcher<BlocklistEvent>(),
            [EventTypes.IdentityChange] = new TypedEventDispatcher<IdentityChangeEvent>(),
            [EventTypes.NewsletterJoin] = new TypedEventDispatcher<NewsletterJoinEvent>(),
            [EventTypes.NewsletterLeave] = new TypedEventDispatcher<NewsletterLeaveEvent>(),
            [EventTypes.NewsletterMuteChange] = new TypedEventDispatcher<NewsletterMuteChangeEvent>(),
            [EventTypes.NewsletterLiveUpdate] = new TypedEventDispatcher<NewsletterLiveUpdateEvent>(),
            [EventTypes.MediaRetry] = new TypedEventDispatcher<MediaRetryEvent>(),
            [EventTypes.UserAbout] = new TypedEventDispatcher<UserAboutEvent>(),
            [EventTypes.FbMessage] = new TypedEventDispatcher<FbMessageEvent>(),
            [EventTypes.CatRefreshError] = new TypedEventDispatcher<CatRefreshErrorEvent>(),
        };
    }

    /// <inheritdoc/>
    public ITypedEventDispatcher GetDispatcher(string eventType)
    {
        return this.dispatchers.TryGetValue(eventType, out var dispatcher)
            ? dispatcher
            : this.unknownDispatcher;
    }
}

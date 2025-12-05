using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedEventDispatcherRegistry"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory for creating typed loggers.</param>
    public TypedEventDispatcherRegistry(ILoggerFactory loggerFactory)
    {
        if (loggerFactory == null)
            throw new ArgumentNullException(nameof(loggerFactory));

        this.unknownDispatcher = CreateDispatcher<UnknownEventEnvelope>(loggerFactory);

        this.dispatchers = new Dictionary<string, ITypedEventDispatcher>(StringComparer.Ordinal)
        {
            [EventTypes.Message] = CreateDispatcher<MessageEventEnvelope>(loggerFactory),
            [EventTypes.UndecryptableMessage] = CreateDispatcher<UndecryptableMessageEventEnvelope>(loggerFactory),
            [EventTypes.Receipt] = CreateDispatcher<ReceiptEventEnvelope>(loggerFactory),
            [EventTypes.ReadReceipt] = CreateDispatcher<ReceiptEventEnvelope>(loggerFactory),
            [EventTypes.Presence] = CreateDispatcher<PresenceEventEnvelope>(loggerFactory),
            [EventTypes.ChatPresence] = CreateDispatcher<ChatPresenceEventEnvelope>(loggerFactory),
            [EventTypes.Connected] = CreateDispatcher<ConnectedEventEnvelope>(loggerFactory),
            [EventTypes.Disconnected] = CreateDispatcher<DisconnectedEventEnvelope>(loggerFactory),
            [EventTypes.Qr] = CreateDispatcher<QrCodeEventEnvelope>(loggerFactory),
            [EventTypes.QrTimeout] = CreateDispatcher<QrTimeoutEventEnvelope>(loggerFactory),
            [EventTypes.QrScannedWithoutMultidevice] = CreateDispatcher<QrScannedWithoutMultideviceEventEnvelope>(loggerFactory),
            [EventTypes.PairSuccess] = CreateDispatcher<PairSuccessEventEnvelope>(loggerFactory),
            [EventTypes.PairError] = CreateDispatcher<PairErrorEventEnvelope>(loggerFactory),
            [EventTypes.LoggedOut] = CreateDispatcher<LoggedOutEventEnvelope>(loggerFactory),
            [EventTypes.ConnectFailure] = CreateDispatcher<ConnectFailureEventEnvelope>(loggerFactory),
            [EventTypes.ClientOutdated] = CreateDispatcher<ClientOutdatedEventEnvelope>(loggerFactory),
            [EventTypes.TemporaryBan] = CreateDispatcher<TemporaryBanEventEnvelope>(loggerFactory),
            [EventTypes.StreamError] = CreateDispatcher<StreamErrorEventEnvelope>(loggerFactory),
            [EventTypes.StreamReplaced] = CreateDispatcher<StreamReplacedEventEnvelope>(loggerFactory),
            [EventTypes.KeepAliveTimeout] = CreateDispatcher<KeepAliveTimeoutEventEnvelope>(loggerFactory),
            [EventTypes.KeepAliveRestored] = CreateDispatcher<KeepAliveRestoredEventEnvelope>(loggerFactory),
            [EventTypes.CallOffer] = CreateDispatcher<CallOfferEventEnvelope>(loggerFactory),
            [EventTypes.CallAccept] = CreateDispatcher<CallAcceptEventEnvelope>(loggerFactory),
            [EventTypes.CallTerminate] = CreateDispatcher<CallTerminateEventEnvelope>(loggerFactory),
            [EventTypes.CallOfferNotice] = CreateDispatcher<CallOfferNoticeEventEnvelope>(loggerFactory),
            [EventTypes.CallRelayLatency] = CreateDispatcher<CallRelayLatencyEventEnvelope>(loggerFactory),
            [EventTypes.GroupInfo] = CreateDispatcher<GroupInfoEventEnvelope>(loggerFactory),
            [EventTypes.JoinedGroup] = CreateDispatcher<JoinedGroupEventEnvelope>(loggerFactory),
            [EventTypes.Picture] = CreateDispatcher<PictureEventEnvelope>(loggerFactory),
            [EventTypes.HistorySync] = CreateDispatcher<HistorySyncEventEnvelope>(loggerFactory),
            [EventTypes.AppState] = CreateDispatcher<AppStateEventEnvelope>(loggerFactory),
            [EventTypes.AppStateSyncComplete] = CreateDispatcher<AppStateSyncCompleteEventEnvelope>(loggerFactory),
            [EventTypes.OfflineSyncCompleted] = CreateDispatcher<OfflineSyncCompletedEventEnvelope>(loggerFactory),
            [EventTypes.OfflineSyncPreview] = CreateDispatcher<OfflineSyncPreviewEventEnvelope>(loggerFactory),
            [EventTypes.PrivacySettings] = CreateDispatcher<PrivacySettingsEventEnvelope>(loggerFactory),
            [EventTypes.PushNameSetting] = CreateDispatcher<PushNameSettingEventEnvelope>(loggerFactory),
            [EventTypes.BlocklistChange] = CreateDispatcher<BlocklistChangeEventEnvelope>(loggerFactory),
            [EventTypes.Blocklist] = CreateDispatcher<BlocklistEventEnvelope>(loggerFactory),
            [EventTypes.IdentityChange] = CreateDispatcher<IdentityChangeEventEnvelope>(loggerFactory),
            [EventTypes.NewsletterJoin] = CreateDispatcher<NewsletterJoinEventEnvelope>(loggerFactory),
            [EventTypes.NewsletterLeave] = CreateDispatcher<NewsletterLeaveEventEnvelope>(loggerFactory),
            [EventTypes.NewsletterMuteChange] = CreateDispatcher<NewsletterMuteChangeEventEnvelope>(loggerFactory),
            [EventTypes.NewsletterLiveUpdate] = CreateDispatcher<NewsletterLiveUpdateEventEnvelope>(loggerFactory),
            [EventTypes.MediaRetry] = CreateDispatcher<MediaRetryEventEnvelope>(loggerFactory),
            [EventTypes.UserAbout] = CreateDispatcher<UserAboutEventEnvelope>(loggerFactory),
            [EventTypes.FbMessage] = CreateDispatcher<FbMessageEventEnvelope>(loggerFactory),
            [EventTypes.CatRefreshError] = CreateDispatcher<CatRefreshErrorEventEnvelope>(loggerFactory),
        };
    }

    private static TypedEventDispatcher<TEvent> CreateDispatcher<TEvent>(ILoggerFactory loggerFactory)
        where TEvent : class, IWhatsAppEnvelope
    {
        return new TypedEventDispatcher<TEvent>(loggerFactory.CreateLogger<TypedEventDispatcher<TEvent>>());
    }

    /// <inheritdoc/>
    public ITypedEventDispatcher GetDispatcher(string eventType)
    {
        return this.dispatchers.TryGetValue(eventType, out var dispatcher)
            ? dispatcher
            : this.unknownDispatcher;
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using WuzApiClient.Common.Enums;
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
    private readonly Dictionary<WhatsAppEventType, ITypedEventDispatcher> dispatchers;
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

        this.dispatchers = new Dictionary<WhatsAppEventType, ITypedEventDispatcher>
        {
            [WhatsAppEventType.Message] = CreateDispatcher<MessageEventEnvelope>(loggerFactory),
            [WhatsAppEventType.UndecryptableMessage] = CreateDispatcher<UndecryptableMessageEventEnvelope>(loggerFactory),
            [WhatsAppEventType.Receipt] = CreateDispatcher<ReceiptEventEnvelope>(loggerFactory),
            [WhatsAppEventType.ReadReceipt] = CreateDispatcher<ReceiptEventEnvelope>(loggerFactory),
            [WhatsAppEventType.Presence] = CreateDispatcher<PresenceEventEnvelope>(loggerFactory),
            [WhatsAppEventType.ChatPresence] = CreateDispatcher<ChatPresenceEventEnvelope>(loggerFactory),
            [WhatsAppEventType.Connected] = CreateDispatcher<ConnectedEventEnvelope>(loggerFactory),
            [WhatsAppEventType.Disconnected] = CreateDispatcher<DisconnectedEventEnvelope>(loggerFactory),
            [WhatsAppEventType.QR] = CreateDispatcher<QrCodeEventEnvelope>(loggerFactory),
            [WhatsAppEventType.QRTimeout] = CreateDispatcher<QrTimeoutEventEnvelope>(loggerFactory),
            [WhatsAppEventType.QRScannedWithoutMultidevice] = CreateDispatcher<QrScannedWithoutMultideviceEventEnvelope>(loggerFactory),
            [WhatsAppEventType.PairSuccess] = CreateDispatcher<PairSuccessEventEnvelope>(loggerFactory),
            [WhatsAppEventType.PairError] = CreateDispatcher<PairErrorEventEnvelope>(loggerFactory),
            [WhatsAppEventType.LoggedOut] = CreateDispatcher<LoggedOutEventEnvelope>(loggerFactory),
            [WhatsAppEventType.ConnectFailure] = CreateDispatcher<ConnectFailureEventEnvelope>(loggerFactory),
            [WhatsAppEventType.ClientOutdated] = CreateDispatcher<ClientOutdatedEventEnvelope>(loggerFactory),
            [WhatsAppEventType.TemporaryBan] = CreateDispatcher<TemporaryBanEventEnvelope>(loggerFactory),
            [WhatsAppEventType.StreamError] = CreateDispatcher<StreamErrorEventEnvelope>(loggerFactory),
            [WhatsAppEventType.StreamReplaced] = CreateDispatcher<StreamReplacedEventEnvelope>(loggerFactory),
            [WhatsAppEventType.KeepAliveTimeout] = CreateDispatcher<KeepAliveTimeoutEventEnvelope>(loggerFactory),
            [WhatsAppEventType.KeepAliveRestored] = CreateDispatcher<KeepAliveRestoredEventEnvelope>(loggerFactory),
            [WhatsAppEventType.CallOffer] = CreateDispatcher<CallOfferEventEnvelope>(loggerFactory),
            [WhatsAppEventType.CallAccept] = CreateDispatcher<CallAcceptEventEnvelope>(loggerFactory),
            [WhatsAppEventType.CallTerminate] = CreateDispatcher<CallTerminateEventEnvelope>(loggerFactory),
            [WhatsAppEventType.CallOfferNotice] = CreateDispatcher<CallOfferNoticeEventEnvelope>(loggerFactory),
            [WhatsAppEventType.CallRelayLatency] = CreateDispatcher<CallRelayLatencyEventEnvelope>(loggerFactory),
            [WhatsAppEventType.GroupInfo] = CreateDispatcher<GroupInfoEventEnvelope>(loggerFactory),
            [WhatsAppEventType.JoinedGroup] = CreateDispatcher<JoinedGroupEventEnvelope>(loggerFactory),
            [WhatsAppEventType.Picture] = CreateDispatcher<PictureEventEnvelope>(loggerFactory),
            [WhatsAppEventType.HistorySync] = CreateDispatcher<HistorySyncEventEnvelope>(loggerFactory),
            [WhatsAppEventType.AppState] = CreateDispatcher<AppStateEventEnvelope>(loggerFactory),
            [WhatsAppEventType.AppStateSyncComplete] = CreateDispatcher<AppStateSyncCompleteEventEnvelope>(loggerFactory),
            [WhatsAppEventType.OfflineSyncCompleted] = CreateDispatcher<OfflineSyncCompletedEventEnvelope>(loggerFactory),
            [WhatsAppEventType.OfflineSyncPreview] = CreateDispatcher<OfflineSyncPreviewEventEnvelope>(loggerFactory),
            [WhatsAppEventType.PrivacySettings] = CreateDispatcher<PrivacySettingsEventEnvelope>(loggerFactory),
            [WhatsAppEventType.PushNameSetting] = CreateDispatcher<PushNameSettingEventEnvelope>(loggerFactory),
            [WhatsAppEventType.BlocklistChange] = CreateDispatcher<BlocklistChangeEventEnvelope>(loggerFactory),
            [WhatsAppEventType.Blocklist] = CreateDispatcher<BlocklistEventEnvelope>(loggerFactory),
            [WhatsAppEventType.IdentityChange] = CreateDispatcher<IdentityChangeEventEnvelope>(loggerFactory),
            [WhatsAppEventType.NewsletterJoin] = CreateDispatcher<NewsletterJoinEventEnvelope>(loggerFactory),
            [WhatsAppEventType.NewsletterLeave] = CreateDispatcher<NewsletterLeaveEventEnvelope>(loggerFactory),
            [WhatsAppEventType.NewsletterMuteChange] = CreateDispatcher<NewsletterMuteChangeEventEnvelope>(loggerFactory),
            [WhatsAppEventType.NewsletterLiveUpdate] = CreateDispatcher<NewsletterLiveUpdateEventEnvelope>(loggerFactory),
            [WhatsAppEventType.MediaRetry] = CreateDispatcher<MediaRetryEventEnvelope>(loggerFactory),
            [WhatsAppEventType.UserAbout] = CreateDispatcher<UserAboutEventEnvelope>(loggerFactory),
            [WhatsAppEventType.FBMessage] = CreateDispatcher<FbMessageEventEnvelope>(loggerFactory),
            [WhatsAppEventType.CATRefreshError] = CreateDispatcher<CatRefreshErrorEventEnvelope>(loggerFactory),
        };
    }

    private static TypedEventDispatcher<TEvent> CreateDispatcher<TEvent>(ILoggerFactory loggerFactory)
        where TEvent : class, IWhatsAppEventEnvelope
    {
        return new TypedEventDispatcher<TEvent>(loggerFactory.CreateLogger<TypedEventDispatcher<TEvent>>());
    }

    /// <inheritdoc/>
    public ITypedEventDispatcher GetDispatcher(WhatsAppEventType eventType)
    {
        return this.dispatchers.TryGetValue(eventType, out var dispatcher)
            ? dispatcher
            : this.unknownDispatcher;
    }
}

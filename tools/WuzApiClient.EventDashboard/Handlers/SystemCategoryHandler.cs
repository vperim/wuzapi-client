using WuzApiClient.EventDashboard.Models;
using WuzApiClient.EventDashboard.Models.Metadata;
using WuzApiClient.EventDashboard.Services;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models;
using WuzApiClient.RabbitMq.Models.Events;

namespace WuzApiClient.EventDashboard.Handlers;

/// <summary>
/// Handles system events (Sync, Newsletter, Privacy, Blocklist, Keep-alive, etc.).
/// </summary>
public sealed class SystemCategoryHandler :
    IEventHandler<HistorySyncEvent>,
    IEventHandler<AppStateEvent>,
    IEventHandler<AppStateSyncCompleteEvent>,
    IEventHandler<OfflineSyncCompletedEvent>,
    IEventHandler<OfflineSyncPreviewEvent>,
    IEventHandler<NewsletterJoinEvent>,
    IEventHandler<NewsletterLeaveEvent>,
    IEventHandler<NewsletterMuteChangeEvent>,
    IEventHandler<NewsletterLiveUpdateEvent>,
    IEventHandler<PrivacySettingsEvent>,
    IEventHandler<PushNameSettingEvent>,
    IEventHandler<UserAboutEvent>,
    IEventHandler<BlocklistChangeEvent>,
    IEventHandler<BlocklistEvent>,
    IEventHandler<IdentityChangeEvent>,
    IEventHandler<MediaRetryEvent>,
    IEventHandler<CatRefreshErrorEvent>,
    IEventHandler<KeepAliveTimeoutEvent>,
    IEventHandler<KeepAliveRestoredEvent>
{
    private readonly IEventStreamService eventStream;

    public SystemCategoryHandler(IEventStreamService eventStream)
    {
        this.eventStream = eventStream;
    }

    // Sync Events
    public Task HandleAsync(WuzEventEnvelope<HistorySyncEvent> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "HistorySync",
            IsSyncComplete = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<AppStateEvent> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "AppState",
            IsSyncComplete = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<AppStateSyncCompleteEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "AppStateSyncComplete",
            Details = evt.Name,
            IsSyncComplete = true
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<OfflineSyncCompletedEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "OfflineSyncCompleted",
            Details = $"{evt.Count} items synced",
            IsSyncComplete = true
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<OfflineSyncPreviewEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "OfflineSyncPreview",
            Details = $"Total: {evt.Total}, Messages: {evt.Messages}, Receipts: {evt.Receipts}",
            IsSyncComplete = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    // Newsletter Events
    public Task HandleAsync(WuzEventEnvelope<NewsletterJoinEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "NewsletterJoin",
            NewsletterName = evt.Id,
            SubscriberCount = evt.ViewerMeta?.Role != null ? 1 : null,
            IsSyncComplete = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<NewsletterLeaveEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "NewsletterLeave",
            NewsletterName = evt.Id,
            Details = evt.Role,
            IsSyncComplete = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<NewsletterMuteChangeEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "NewsletterMuteChange",
            NewsletterName = evt.Id,
            Details = evt.Mute ?? "Unknown",
            IsSyncComplete = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<NewsletterLiveUpdateEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "NewsletterLiveUpdate",
            NewsletterName = evt.Jid,
            Details = $"{evt.Messages?.Length ?? 0} messages",
            IsSyncComplete = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    // Privacy/Settings Events
    public Task HandleAsync(WuzEventEnvelope<PrivacySettingsEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var changedSettings = new List<string>();
        if (evt.GroupAddChanged) changedSettings.Add("GroupAdd");
        if (evt.LastSeenChanged) changedSettings.Add("LastSeen");
        if (evt.StatusChanged) changedSettings.Add("Status");
        if (evt.ProfileChanged) changedSettings.Add("Profile");
        if (evt.ReadReceiptsChanged) changedSettings.Add("ReadReceipts");
        if (evt.OnlineChanged) changedSettings.Add("Online");
        if (evt.CallAddChanged) changedSettings.Add("CallAdd");

        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "PrivacySettings",
            SettingType = "Privacy",
            Details = changedSettings.Count > 0 ? string.Join(", ", changedSettings) : "No changes",
            IsSyncComplete = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<PushNameSettingEvent> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "PushNameSetting",
            SettingType = "PushName",
            IsSyncComplete = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<UserAboutEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "UserAbout",
            Details = $"User: {evt.Jid}, Status: {evt.Status}",
            IsSyncComplete = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    // Blocklist Events
    public Task HandleAsync(WuzEventEnvelope<BlocklistChangeEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "BlocklistChange",
            Details = $"{evt.Action}: {evt.Jid}",
            IsSyncComplete = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<BlocklistEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "Blocklist",
            Details = $"{evt.Action}, {evt.Changes?.Length ?? 0} changes",
            IsSyncComplete = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    // Keep-Alive Events
    public Task HandleAsync(WuzEventEnvelope<KeepAliveTimeoutEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "KeepAliveTimeout",
            Details = $"Errors: {evt.ErrorCount}, Last Success: {evt.LastSuccess}",
            IsSyncComplete = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<KeepAliveRestoredEvent> envelope, CancellationToken cancellationToken = default)
    {
        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "KeepAliveRestored",
            IsSyncComplete = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    // Contact Events
    public Task HandleAsync(WuzEventEnvelope<IdentityChangeEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "IdentityChange",
            Details = $"User: {evt.Jid}, Implicit: {evt.Implicit}",
            IsSyncComplete = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<MediaRetryEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "MediaRetry",
            Details = $"Chat: {evt.ChatId}, Message: {evt.MessageId}, Error: {evt.Error}",
            IsSyncComplete = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }

    // Error Events
    public Task HandleAsync(WuzEventEnvelope<CatRefreshErrorEvent> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Event;
        var metadata = new SystemMetadata
        {
            Category = EventCategory.System,
            SystemEvent = "CATRefreshError",
            Details = evt.Error,
            IsSyncComplete = false
        };

        eventStream.AddEvent(envelope, metadata);
        return Task.CompletedTask;
    }
}

using WuzApiClient.EventDashboard.Models;
using WuzApiClient.EventDashboard.Models.Metadata;
using WuzApiClient.EventDashboard.Services;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models.Events;
using WuzApiClient.RabbitMq.Models.Wuz;

namespace WuzApiClient.EventDashboard.Handlers;

/// <summary>
/// Handles system events (Sync, Newsletter, Privacy, Blocklist, Keep-alive, etc.).
/// </summary>
public sealed class SystemCategoryHandler :
    IEventHandler<HistorySyncEventEnvelope>,
    IEventHandler<AppStateEventEnvelope>,
    IEventHandler<AppStateSyncCompleteEventEnvelope>,
    IEventHandler<OfflineSyncCompletedEventEnvelope>,
    IEventHandler<OfflineSyncPreviewEventEnvelope>,
    IEventHandler<NewsletterJoinEventEnvelope>,
    IEventHandler<NewsletterLeaveEventEnvelope>,
    IEventHandler<NewsletterMuteChangeEventEnvelope>,
    IEventHandler<NewsletterLiveUpdateEventEnvelope>,
    IEventHandler<PrivacySettingsEventEnvelope>,
    IEventHandler<PushNameSettingEventEnvelope>,
    IEventHandler<UserAboutEventEnvelope>,
    IEventHandler<BlocklistChangeEventEnvelope>,
    IEventHandler<BlocklistEventEnvelope>,
    IEventHandler<IdentityChangeEventEnvelope>,
    IEventHandler<MediaRetryEventEnvelope>,
    IEventHandler<CatRefreshErrorEventEnvelope>,
    IEventHandler<KeepAliveTimeoutEventEnvelope>,
    IEventHandler<KeepAliveRestoredEventEnvelope>
{
    private readonly IEventStreamService eventStream;

    public SystemCategoryHandler(IEventStreamService eventStream)
    {
        this.eventStream = eventStream;
    }

    // Sync Events
    public Task HandleAsync(IWuzEventEnvelope<HistorySyncEventEnvelope> envelope, CancellationToken cancellationToken = default)
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

    public Task HandleAsync(IWuzEventEnvelope<AppStateEventEnvelope> envelope, CancellationToken cancellationToken = default)
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

    public Task HandleAsync(IWuzEventEnvelope<AppStateSyncCompleteEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
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

    public Task HandleAsync(IWuzEventEnvelope<OfflineSyncCompletedEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
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

    public Task HandleAsync(IWuzEventEnvelope<OfflineSyncPreviewEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
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
    public Task HandleAsync(IWuzEventEnvelope<NewsletterJoinEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
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

    public Task HandleAsync(IWuzEventEnvelope<NewsletterLeaveEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
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

    public Task HandleAsync(IWuzEventEnvelope<NewsletterMuteChangeEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
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

    public Task HandleAsync(IWuzEventEnvelope<NewsletterLiveUpdateEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
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
    public Task HandleAsync(IWuzEventEnvelope<PrivacySettingsEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
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

    public Task HandleAsync(IWuzEventEnvelope<PushNameSettingEventEnvelope> envelope, CancellationToken cancellationToken = default)
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

    public Task HandleAsync(IWuzEventEnvelope<UserAboutEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
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
    public Task HandleAsync(IWuzEventEnvelope<BlocklistChangeEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
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

    public Task HandleAsync(IWuzEventEnvelope<BlocklistEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
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
    public Task HandleAsync(IWuzEventEnvelope<KeepAliveTimeoutEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
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

    public Task HandleAsync(IWuzEventEnvelope<KeepAliveRestoredEventEnvelope> envelope, CancellationToken cancellationToken = default)
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
    public Task HandleAsync(IWuzEventEnvelope<IdentityChangeEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
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

    public Task HandleAsync(IWuzEventEnvelope<MediaRetryEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
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
    public Task HandleAsync(IWuzEventEnvelope<CatRefreshErrorEventEnvelope> envelope, CancellationToken cancellationToken = default)
    {
        var evt = envelope.Payload.Event;
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

# Event Types Reference

Reference for all 44 WhatsApp event types available in `WuzApiClient.RabbitMq`.

> **Note:** The most commonly used event types (`MessageEvent`, `ReceiptEvent`, `PresenceEvent`, `GroupInfoEvent`) include detailed code examples below. For all other event types, see the [Complete Event Type Catalog](#complete-event-type-catalog).

## Base Event Type

All events inherit from `WuzEvent`:

```csharp
public abstract record WuzEvent
{
    public string Type { get; init; }               // Event type identifier
    public string UserId { get; init; }             // User ID that generated this event
    public string InstanceName { get; init; }       // Instance name
    public JsonElement? RawEvent { get; init; }     // Raw event payload for dynamic access
    public DateTimeOffset ReceivedAt { get; init; } // Timestamp when event was received
}
```

> **JSON Property Naming:** Event properties use camelCase in JSON serialization but PascalCase in C# code. For example, the `GroupJid` property in C# corresponds to `"groupJid"` in JSON. When examining raw JSON payloads, expect camelCase property names. The library handles this conversion automatically during deserialization.

## Commonly Used Events (Detailed Examples)

> **Note:** All examples require the following namespace imports:
> ```csharp
> using WuzApiClient.Events.Models;
> using WuzApiClient.Events.Models.Events;
> using WuzApiClient.Events.Core.Interfaces;
> ```

### MessageEvent

Triggered when a message is received (text, image, document, audio, video, etc.).

```csharp
public sealed record MessageEvent : WuzEvent
{
    public MessageInfo? Info { get; init; }          // Message metadata (ID, timestamp, sender, chat)
    public MessageContent? Message { get; init; }    // Message content (text, media, etc.)
    public string? Base64 { get; init; }             // Base64-encoded media content
    public string? MimeType { get; init; }           // Media MIME type
    public string? FileName { get; init; }           // Media file name
    public S3MediaInfo? S3 { get; init; }            // S3 media information
    public bool IsSticker { get; init; }             // True if sticker message
    public bool StickerAnimated { get; init; }       // True if animated sticker
    public bool IsViewOnce { get; init; }            // True if view-once message
    public bool IsEphemeral { get; init; }           // True if ephemeral message
}
```

**Example Handler:**

```csharp
public class MessageHandler : IEventHandler<MessageEvent>
{
    private readonly ILogger<MessageHandler> _logger;

    public MessageHandler(ILogger<MessageHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(MessageEvent @event, CancellationToken ct)
    {
        if (@event.Info == null || @event.Message == null)
            return;

        _logger.LogInformation(
            "Received message from {From} in chat {Chat}: ID={MessageId}",
            @event.Info.Sender,
            @event.Info.Chat,
            @event.Info.ID
        );

        // Access message content through Message property
        // (specific fields depend on message type)
    }
}
```

### ReceiptEvent

Triggered when message delivery or read receipts are received.

```csharp
public sealed record ReceiptEvent : WuzEvent
{
    public string? Chat { get; init; }                      // Chat JID
    public string? Sender { get; init; }                    // Sender JID
    public bool IsFromMe { get; init; }                     // True if from current user
    public bool IsGroup { get; init; }                      // True if group chat
    public IReadOnlyList<string>? MessageIDs { get; init; } // Message IDs this receipt applies to
    public DateTimeOffset? Timestamp { get; init; }         // Receipt timestamp
    public string? ReceiptType { get; init; }               // Receipt type from whatsmeow
    public string? MessageSender { get; init; }             // Message sender JID (for group receipts)
    public string? State { get; init; }                     // Receipt state: "Read", "ReadSelf", "Delivered"
}
```

**Example Handler:**

```csharp
public class ReceiptHandler : IEventHandler<ReceiptEvent>
{
    private readonly ILogger<ReceiptHandler> _logger;

    public ReceiptHandler(ILogger<ReceiptHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(ReceiptEvent @event, CancellationToken ct)
    {
        if (@event.MessageIDs == null || @event.MessageIDs.Count == 0)
            return;

        switch (@event.State)
        {
            case "Delivered":
                _logger.LogInformation(
                    "Messages delivered: {MessageIds}",
                    string.Join(", ", @event.MessageIDs)
                );
                break;
            case "Read":
                _logger.LogInformation(
                    "Messages read by {Sender}: {MessageIds}",
                    @event.Sender,
                    string.Join(", ", @event.MessageIDs)
                );
                break;
        }
    }
}
```

### PresenceEvent

Triggered when contact's online/offline status changes.

```csharp
public sealed record PresenceEvent : WuzEvent
{
    public string? From { get; init; }           // JID of user whose presence changed
    public bool Unavailable { get; init; }       // True if user is offline
    public DateTimeOffset? LastSeen { get; init; } // Last seen timestamp
    public string? State { get; init; }          // Presence state: "online" or "offline"
}
```

**Example Handler:**

```csharp
public class PresenceHandler : IEventHandler<PresenceEvent>
{
    private readonly ILogger<PresenceHandler> _logger;

    public PresenceHandler(ILogger<PresenceHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(PresenceEvent @event, CancellationToken ct)
    {
        if (@event.State == "online" || !@event.Unavailable)
        {
            _logger.LogInformation("{User} is online", @event.From);
        }
        else if (@event.LastSeen.HasValue)
        {
            _logger.LogInformation(
                "{User} was last seen at {Time}",
                @event.From,
                @event.LastSeen.Value
            );
        }
    }
}
```

### GroupInfoEvent

Triggered when group information (name, topic) is updated.

```csharp
public sealed record GroupInfoEvent : WuzEvent
{
    public string? GroupJid { get; init; }  // Group JID
    public string? Name { get; init; }      // Group name
    public string? Topic { get; init; }     // Group topic/description
}
```

**Example Handler:**

```csharp
public class GroupInfoHandler : IEventHandler<GroupInfoEvent>
{
    private readonly ILogger<GroupInfoHandler> _logger;

    public GroupInfoHandler(ILogger<GroupInfoHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(GroupInfoEvent @event, CancellationToken ct)
    {
        _logger.LogInformation(
            "Group {GroupJid} updated: Name='{Name}', Topic='{Topic}'",
            @event.GroupJid,
            @event.Name,
            @event.Topic
        );
    }
}
```

## Complete Event Type Catalog

The library includes 44 event types total. Below is the complete list organized by category.

### Connection Events

| Event Type | Triggered When |
|------------|----------------|
| `ConnectedEvent` | Client successfully connects to WhatsApp |
| `DisconnectedEvent` | Client disconnects from WhatsApp |
| `ConnectFailureEvent` | Connection attempt fails |
| `LoggedOutEvent` | User is logged out from WhatsApp |
| `QrCodeEvent` | QR code is generated for pairing (contains `QrCodeBase64` property with base64-encoded QR code image) |
| `QrTimeoutEvent` | QR code expires without being scanned |
| `PairSuccessEvent` | Device pairing succeeds |
| `PairErrorEvent` | Device pairing fails |
| `StreamErrorEvent` | WebSocket stream error occurs |
| `StreamReplacedEvent` | WebSocket stream is replaced |
| `KeepAliveTimeoutEvent` | Keep-alive ping timeout |
| `KeepAliveRestoredEvent` | Keep-alive connection restored |
| `ClientOutdatedEvent` | Client version is outdated |
| `TemporaryBanEvent` | Account is temporarily banned |

### Message Events

| Event Type | Triggered When |
|------------|----------------|
| `MessageEvent` | Incoming WhatsApp message (text, media, etc.) |
| `ReceiptEvent` | Message delivery/read receipt received |
| `UndecryptableMessageEvent` | Message cannot be decrypted |
| `MediaRetryEvent` | Media download retry is requested |
| `FBMessageEvent` | Facebook message event |

### Call Events

| Event Type | Triggered When |
|------------|----------------|
| `CallOfferEvent` | Incoming call offer received |
| `CallAcceptEvent` | Call is accepted |
| `CallTerminateEvent` | Call is terminated |
| `CallOfferNoticeEvent` | Call offer notice received |
| `CallRelayLatencyEvent` | Call relay latency update |

### Group Events

| Event Type | Triggered When |
|------------|----------------|
| `GroupInfoEvent` | Group information (name, topic) is updated |
| `JoinedGroupEvent` | Current user joins a group |

### Newsletter Events

| Event Type | Triggered When |
|------------|----------------|
| `NewsletterJoinEvent` | User joins a newsletter |
| `NewsletterLeaveEvent` | User leaves a newsletter |
| `NewsletterLiveUpdateEvent` | Newsletter live update received |
| `NewsletterMuteChangeEvent` | Newsletter mute status changes |

### Presence Events

| Event Type | Triggered When |
|------------|----------------|
| `PresenceEvent` | Contact online/offline status changes |
| `ChatPresenceEvent` | Chat-level presence update (typing, recording) |

### Sync Events

| Event Type | Triggered When |
|------------|----------------|
| `HistorySyncEvent` | Chat history sync occurs |
| `OfflineSyncPreviewEvent` | Offline sync preview is available |
| `OfflineSyncCompletedEvent` | Offline sync completes |
| `AppStateEvent` | App state change occurs |
| `AppStateSyncCompleteEvent` | App state sync completes |

### Privacy & Settings Events

| Event Type | Triggered When |
|------------|----------------|
| `BlocklistEvent` | Blocklist is updated |
| `BlocklistChangeEvent` | Blocklist change occurs |
| `PrivacySettingsEvent` | Privacy settings are changed |
| `PushNameSettingEvent` | Push name (display name) is changed |
| `UserAboutEvent` | User "about" status is updated |
| `PictureEvent` | Profile picture is updated |
| `IdentityChangeEvent` | Contact identity key changes |

## Handling Multiple Event Types

### Separate Handlers

Register individual handlers for each event type:

```csharp
builder.Services.AddScoped<IEventHandler<MessageEvent>, MessageHandler>();
builder.Services.AddScoped<IEventHandler<ReceiptEvent>, ReceiptHandler>();
builder.Services.AddScoped<IEventHandler<GroupInfoEvent>, GroupInfoHandler>();
builder.Services.AddScoped<IEventHandler<PresenceEvent>, PresenceHandler>();
```

### Shared Handler

A single class can handle multiple event types:

```csharp
public class MultiEventHandler :
    IEventHandler<MessageEvent>,
    IEventHandler<ReceiptEvent>,
    IEventHandler<GroupInfoEvent>
{
    private readonly ILogger<MultiEventHandler> _logger;

    public MultiEventHandler(ILogger<MultiEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(MessageEvent @event, CancellationToken ct)
    {
        _logger.LogInformation("Message received from {Sender}", @event.Info?.Sender);
        return Task.CompletedTask;
    }

    public Task HandleAsync(ReceiptEvent @event, CancellationToken ct)
    {
        _logger.LogInformation("Receipt: {State}", @event.State);
        return Task.CompletedTask;
    }

    public Task HandleAsync(GroupInfoEvent @event, CancellationToken ct)
    {
        _logger.LogInformation("Group {Name} updated", @event.Name);
        return Task.CompletedTask;
    }
}

// Register once for all three event types
builder.Services.AddScoped<IEventHandler<MessageEvent>, MultiEventHandler>();
builder.Services.AddScoped<IEventHandler<ReceiptEvent>, MultiEventHandler>();
builder.Services.AddScoped<IEventHandler<GroupInfoEvent>, MultiEventHandler>();
```

## Event Filtering

Filter specific event types using `IEventFilter`:

```csharp
public class MessageOnlyFilter : IEventFilter
{
    public bool ShouldProcess(WuzEvent evt)
    {
        // Only process MessageEvent and ReceiptEvent, ignore everything else
        return evt is MessageEvent or ReceiptEvent;
    }

    public int Order => 0; // Filter priority (lower = earlier execution)
}

// Register the filter
builder.Services.AddSingleton<IEventFilter, MessageOnlyFilter>();
```

## Next Steps

- **Implement Handlers** → [Event Handling Guide](../usage/event-handling.md)
- **HTTP Client Methods** → [HTTP Client Reference](http-client-reference.md)
- **Filter Events** → [Extension Patterns](../usage/extension-patterns.md)


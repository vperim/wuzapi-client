# Event Types Reference

Reference for all 46 WhatsApp event types available in `WuzApiClient.RabbitMq`.

> **Note:** The most commonly used event types (`MessageEvent`, `ReceiptEvent`, `PresenceEvent`, `GroupInfoEvent`) include detailed code examples below. For all other event types, see the [Complete Event Type Catalog](#complete-event-type-catalog).

## Event Envelope Structure

All events are wrapped in `WuzEventEnvelope<TEvent>`:

```csharp
public sealed record WuzEventEnvelope<TEvent> : WuzEventEnvelope
    where TEvent : class
{
    public string EventType { get; init; }          // Event type identifier (e.g., "Message", "Receipt")
    public string UserId { get; init; }             // User ID that generated this event
    public string InstanceName { get; init; }       // Instance name
    public DateTimeOffset ReceivedAt { get; init; } // Timestamp when event was received by client
    public required TEvent Event { get; init; }     // Typed event data (non-nullable)
    public string RawJson { get; init; }            // Raw JSON string of entire event envelope
}
```

**Key Points:**
- Event handlers receive `WuzEventEnvelope<TEvent>`, not raw event objects
- Access the typed event data via the `Event` property
- `RawJson` contains the complete JSON for debugging/logging purposes
- Event classes (like `MessageEvent`, `ReceiptEvent`) are POCOs that don't inherit from a base class

> **JSON Property Naming:** Event properties use camelCase in JSON serialization but PascalCase in C# code. For example, the `GroupJid` property in C# corresponds to `"groupJid"` in JSON. When examining raw JSON payloads, expect camelCase property names. The library handles this conversion automatically during deserialization.

## Commonly Used Events (Detailed Examples)

> **Note:** All examples require the following namespace imports:
> ```csharp
> using WuzApiClient.RabbitMq.Models;
> using WuzApiClient.RabbitMq.Models.Events;
> using WuzApiClient.RabbitMq.Core.Interfaces;
> ```

### MessageEvent

Triggered when a message is received (text, image, document, audio, video, etc.).

```csharp
public sealed record MessageEvent
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
public sealed class MessageHandler : IEventHandler<MessageEvent>
{
    private readonly ILogger<MessageHandler> logger;

    public MessageHandler(ILogger<MessageHandler> logger)
    {
        this.logger = logger;
    }

    public async Task HandleAsync(WuzEventEnvelope<MessageEvent> envelope, CancellationToken ct)
    {
        var @event = envelope.Event; // Extract typed event data

        if (@event.Info == null || @event.Message == null)
            return;

        this.logger.LogInformation(
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
public sealed record ReceiptEvent
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
public sealed class ReceiptHandler : IEventHandler<ReceiptEvent>
{
    private readonly ILogger<ReceiptHandler> logger;

    public ReceiptHandler(ILogger<ReceiptHandler> logger)
    {
        this.logger = logger;
    }

    public async Task HandleAsync(WuzEventEnvelope<ReceiptEvent> envelope, CancellationToken ct)
    {
        var @event = envelope.Event; // Extract typed event data

        if (@event.MessageIDs == null || @event.MessageIDs.Count == 0)
            return;

        switch (@event.State)
        {
            case "Delivered":
                this.logger.LogInformation(
                    "Messages delivered: {MessageIds}",
                    string.Join(", ", @event.MessageIDs)
                );
                break;
            case "Read":
                this.logger.LogInformation(
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
public sealed record PresenceEvent
{
    public string? From { get; init; }           // JID of user whose presence changed
    public bool Unavailable { get; init; }       // True if user is offline
    public DateTimeOffset? LastSeen { get; init; } // Last seen timestamp
    public string? State { get; init; }          // Presence state: "online" or "offline"
}
```

**Example Handler:**

```csharp
public sealed class PresenceHandler : IEventHandler<PresenceEvent>
{
    private readonly ILogger<PresenceHandler> logger;

    public PresenceHandler(ILogger<PresenceHandler> logger)
    {
        this.logger = logger;
    }

    public async Task HandleAsync(WuzEventEnvelope<PresenceEvent> envelope, CancellationToken ct)
    {
        var @event = envelope.Event; // Extract typed event data

        if (@event.State == "online" || !@event.Unavailable)
        {
            this.logger.LogInformation("{User} is online", @event.From);
        }
        else if (@event.LastSeen.HasValue)
        {
            this.logger.LogInformation(
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
public sealed record GroupInfoEvent
{
    public string? GroupJid { get; init; }  // Group JID
    public string? Name { get; init; }      // Group name
    public string? Topic { get; init; }     // Group topic/description
}
```

**Example Handler:**

```csharp
public sealed class GroupInfoHandler : IEventHandler<GroupInfoEvent>
{
    private readonly ILogger<GroupInfoHandler> logger;

    public GroupInfoHandler(ILogger<GroupInfoHandler> logger)
    {
        this.logger = logger;
    }

    public async Task HandleAsync(WuzEventEnvelope<GroupInfoEvent> envelope, CancellationToken ct)
    {
        var @event = envelope.Event; // Extract typed event data

        this.logger.LogInformation(
            "Group {GroupJid} updated: Name='{Name}', Topic='{Topic}'",
            @event.GroupJid,
            @event.Name,
            @event.Topic
        );
    }
}
```

## Complete Event Type Catalog

The library includes 46 event types total. Below is the complete list organized by category.

### Connection Events

| Event Type | Triggered When |
|------------|----------------|
| `ConnectedEvent` | Client successfully connects to WhatsApp |
| `DisconnectedEvent` | Client disconnects from WhatsApp |
| `ConnectFailureEvent` | Connection attempt fails |
| `LoggedOutEvent` | User is logged out from WhatsApp |
| `QrCodeEvent` | QR code is generated for pairing (contains `QrCodeBase64` property with base64-encoded QR code image) |
| `QrTimeoutEvent` | QR code expires without being scanned |
| `QRScannedWithoutMultideviceEvent` | QR code is scanned but multidevice is not enabled on the phone |
| `PairSuccessEvent` | Device pairing succeeds |
| `PairErrorEvent` | Device pairing fails |
| `StreamErrorEvent` | WebSocket stream error occurs |
| `StreamReplacedEvent` | WebSocket stream is replaced |
| `KeepAliveTimeoutEvent` | Keep-alive ping timeout |
| `KeepAliveRestoredEvent` | Keep-alive connection restored |
| `ClientOutdatedEvent` | Client version is outdated |
| `TemporaryBanEvent` | Account is temporarily banned |
| `CATRefreshErrorEvent` | Client Access Token (CAT) refresh operation fails |

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

Register individual handlers for each event type using the fluent builder:

```csharp
// Using assembly scanning (recommended)
builder.Services.AddWuzEvents(builder.Configuration, b => b
    .AddHandlersFromAssembly(ServiceLifetime.Scoped, typeof(Program).Assembly)
);

// Or register explicitly
builder.Services.AddWuzEvents(builder.Configuration, b => b
    .AddHandler<MessageEvent, MessageHandler>(ServiceLifetime.Scoped)
    .AddHandler<ReceiptEvent, ReceiptHandler>(ServiceLifetime.Scoped)
    .AddHandler<GroupInfoEvent, GroupInfoHandler>(ServiceLifetime.Scoped)
    .AddHandler<PresenceEvent, PresenceHandler>(ServiceLifetime.Scoped)
);
```

### Shared Handler

A single class can handle multiple event types:

```csharp
public sealed class MultiEventHandler :
    IEventHandler<MessageEvent>,
    IEventHandler<ReceiptEvent>,
    IEventHandler<GroupInfoEvent>
{
    private readonly ILogger<MultiEventHandler> logger;

    public MultiEventHandler(ILogger<MultiEventHandler> logger)
    {
        this.logger = logger;
    }

    public Task HandleAsync(WuzEventEnvelope<MessageEvent> envelope, CancellationToken ct)
    {
        var @event = envelope.Event;
        this.logger.LogInformation("Message received from {Sender}", @event.Info?.Sender);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<ReceiptEvent> envelope, CancellationToken ct)
    {
        var @event = envelope.Event;
        this.logger.LogInformation("Receipt: {State}", @event.State);
        return Task.CompletedTask;
    }

    public Task HandleAsync(WuzEventEnvelope<GroupInfoEvent> envelope, CancellationToken ct)
    {
        var @event = envelope.Event;
        this.logger.LogInformation("Group {Name} updated", @event.Name);
        return Task.CompletedTask;
    }
}

// Register using fluent builder (assembly scanning will discover all interfaces)
builder.Services.AddWuzEvents(builder.Configuration, b => b
    .AddHandlersFromAssembly(ServiceLifetime.Scoped, typeof(Program).Assembly)
);
```

## Filtering Events in Handlers

You can implement filtering logic directly in your handlers:

```csharp
public sealed class MessageHandler : IEventHandler<MessageEvent>
{
    private readonly ILogger<MessageHandler> logger;

    public MessageHandler(ILogger<MessageHandler> logger)
    {
        this.logger = logger;
    }

    public async Task HandleAsync(WuzEventEnvelope<MessageEvent> envelope, CancellationToken ct)
    {
        var @event = envelope.Event;

        // Filter: Only process messages from specific user
        if (envelope.UserId != "expected-user-id")
            return;

        // Filter: Ignore messages from ourselves
        if (@event.Info?.IsFromMe == true)
            return;

        // Process the message...
        this.logger.LogInformation("Processing message from {Sender}", @event.Info?.Sender);
    }
}
```

## Next Steps

- **Implement Handlers** → [Event Handling Guide](../usage/event-handling.md)
- **HTTP Client Methods** → [HTTP Client Reference](http-client-reference.md)
- **Extension Patterns** → [Extension Patterns Guide](../usage/extension-patterns.md)


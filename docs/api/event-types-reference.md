# Event Types Reference

Reference for all 46 WhatsApp event types available in `WuzApiClient.RabbitMq`.

> **Note:** The most commonly used event types (`MessageEventEnvelope`, `ReceiptEventEnvelope`, `PresenceEventEnvelope`, `GroupInfoEventEnvelope`) include detailed code examples below. For all other event types, see the [Complete Event Type Catalog](#complete-event-type-catalog).

## Event Envelope Structure

All events are wrapped in a two-layer envelope structure:

```csharp
// Outer envelope - generic wrapper
public sealed record WuzEventEnvelope<TPayload> : IWuzEventEnvelope<TPayload>
    where TPayload : class
{
    public required TPayload Payload { get; init; }         // Typed event payload (non-nullable)
    public required WuzEventMetadata Metadata { get; init; } // Metadata (UserId, InstanceName, etc.)
    public DateTimeOffset ReceivedAt { get; init; }         // Timestamp when event was received
}

// Payload structure - event-specific (example: MessageEventEnvelope)
public sealed record MessageEventEnvelope : IWhatsAppEnvelope
{
    public required string Type { get; init; }               // Event type identifier (e.g., "Message")
    public required MessageEventData Event { get; init; }    // Actual event data
    // Additional envelope-level properties (varies by event type)
    public string? Base64 { get; init; }                     // Base64-encoded media (MessageEventEnvelope only)
    public S3MediaInfo? S3 { get; init; }                    // S3 media info (MessageEventEnvelope only)
    public string? State { get; init; }                      // Receipt state (ReceiptEventEnvelope only)
}
```

**Key Points:**
- Event handlers receive `IWuzEventEnvelope<TPayload>` (e.g., `IWuzEventEnvelope<MessageEventEnvelope>`)
- Access the payload via `envelope.Payload`
- Access the actual event data via `envelope.Payload.Event`
- Some payloads have envelope-level properties (e.g., `MessageEventEnvelope` has `Base64`, `S3`, `FileName`)
- Payload types (like `MessageEventEnvelope`) implement `IWhatsAppEnvelope`

## Commonly Used Events (Detailed Examples)

> **Note:** All examples require the following namespace imports:
> ```csharp
> using WuzApiClient.RabbitMq.Models;
> using WuzApiClient.RabbitMq.Models.Events;
> using WuzApiClient.RabbitMq.Core.Interfaces;
> ```

### MessageEventEnvelope

Triggered when a message is received (text, image, document, audio, video, etc.).

```csharp
// Payload structure
public sealed record MessageEventEnvelope : IWhatsAppEnvelope
{
    public required string Type { get; init; }       // "Message"
    public required MessageEventData Event { get; init; }  // Actual event data
    // Envelope-level properties
    public string? Base64 { get; init; }             // Base64-encoded media content
    public string? MimeType { get; init; }           // Media MIME type
    public string? FileName { get; init; }           // Media file name
    public S3MediaInfo? S3 { get; init; }            // S3 media information
}

// Event data
public sealed record MessageEventData
{
    public MessageInfo? Info { get; init; }          // Message metadata (ID, timestamp, sender, chat)
    public MessageContent? Message { get; init; }    // Message content (text, media, etc.)
    public bool IsSticker { get; init; }             // True if sticker message
    public bool StickerAnimated { get; init; }       // True if animated sticker
    public bool IsViewOnce { get; init; }            // True if view-once message
    public bool IsEphemeral { get; init; }           // True if ephemeral message
}
```

**Example Handler:**

```csharp
public sealed class MessageHandler : IEventHandler<MessageEventEnvelope>
{
    private readonly ILogger<MessageHandler> logger;

    public MessageHandler(ILogger<MessageHandler> logger)
    {
        this.logger = logger;
    }

    public async Task HandleAsync(IWuzEventEnvelope<MessageEventEnvelope> envelope, CancellationToken ct)
    {
        var @event = envelope.Payload.Event; // Extract event data
        var messageEnvelope = envelope.Payload; // For envelope-level properties

        if (@event.Info == null || @event.Message == null)
            return;

        this.logger.LogInformation(
            "Received message from {From} in chat {Chat}: ID={MessageId}",
            @event.Info.Sender,
            @event.Info.Chat,
            @event.Info.ID
        );

        // Access envelope-level properties for media messages
        if (messageEnvelope.S3 != null)
        {
            this.logger.LogInformation("Media available at S3: {S3Key}", messageEnvelope.S3.Key);
        }
    }
}
```

### ReceiptEventEnvelope

Triggered when message delivery or read receipts are received.

```csharp
// Payload structure
public sealed record ReceiptEventEnvelope : IWhatsAppEnvelope
{
    public required string Type { get; init; }       // "Receipt"
    public required ReceiptEventData Event { get; init; }  // Actual event data
    public string? State { get; init; }              // Receipt state: "Read", "ReadSelf", "Delivered"
}

// Event data
public sealed record ReceiptEventData
{
    public string? Chat { get; init; }                      // Chat JID
    public string? Sender { get; init; }                    // Sender JID
    public bool IsFromMe { get; init; }                     // True if from current user
    public bool IsGroup { get; init; }                      // True if group chat
    public IReadOnlyList<string>? MessageIDs { get; init; } // Message IDs this receipt applies to
    public DateTimeOffset? Timestamp { get; init; }         // Receipt timestamp
    public string? ReceiptType { get; init; }               // Receipt type from whatsmeow
    public string? MessageSender { get; init; }             // Message sender JID (for group receipts)
}
```

**Example Handler:**

```csharp
public sealed class ReceiptHandler : IEventHandler<ReceiptEventEnvelope>
{
    private readonly ILogger<ReceiptHandler> logger;

    public ReceiptHandler(ILogger<ReceiptHandler> logger)
    {
        this.logger = logger;
    }

    public async Task HandleAsync(IWuzEventEnvelope<ReceiptEventEnvelope> envelope, CancellationToken ct)
    {
        var @event = envelope.Payload.Event; // Extract event data
        var receiptEnvelope = envelope.Payload; // For State property

        if (@event.MessageIDs == null || @event.MessageIDs.Count == 0)
            return;

        switch (receiptEnvelope.State) // State is on envelope, not event data
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

### PresenceEventEnvelope

Triggered when contact's online/offline status changes.

```csharp
// Payload structure
public sealed record PresenceEventEnvelope : IWhatsAppEnvelope
{
    public required string Type { get; init; }       // "Presence"
    public required PresenceEventData Event { get; init; }  // Actual event data
}

// Event data
public sealed record PresenceEventData
{
    public string? From { get; init; }           // JID of user whose presence changed
    public bool Unavailable { get; init; }       // True if user is offline
    public DateTimeOffset? LastSeen { get; init; } // Last seen timestamp
    public string? State { get; init; }          // Presence state: "online" or "offline"
}
```

**Example Handler:**

```csharp
public sealed class PresenceHandler : IEventHandler<PresenceEventEnvelope>
{
    private readonly ILogger<PresenceHandler> logger;

    public PresenceHandler(ILogger<PresenceHandler> logger)
    {
        this.logger = logger;
    }

    public async Task HandleAsync(IWuzEventEnvelope<PresenceEventEnvelope> envelope, CancellationToken ct)
    {
        var @event = envelope.Payload.Event; // Extract event data

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

### GroupInfoEventEnvelope

Triggered when group information (name, topic) is updated.

```csharp
// Payload structure
public sealed record GroupInfoEventEnvelope : IWhatsAppEnvelope
{
    public required string Type { get; init; }       // "GroupInfo"
    public required GroupInfoEventData Event { get; init; }  // Actual event data
}

// Event data
public sealed record GroupInfoEventData
{
    public string? GroupJid { get; init; }  // Group JID
    public string? Name { get; init; }      // Group name
    public string? Topic { get; init; }     // Group topic/description
}
```

**Example Handler:**

```csharp
public sealed class GroupInfoHandler : IEventHandler<GroupInfoEventEnvelope>
{
    private readonly ILogger<GroupInfoHandler> logger;

    public GroupInfoHandler(ILogger<GroupInfoHandler> logger)
    {
        this.logger = logger;
    }

    public async Task HandleAsync(IWuzEventEnvelope<GroupInfoEventEnvelope> envelope, CancellationToken ct)
    {
        var @event = envelope.Payload.Event; // Extract event data

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
| `MessageEventEnvelope` | Incoming WhatsApp message (text, media, etc.) |
| `ReceiptEventEnvelope` | Message delivery/read receipt received |
| `UndecryptableMessageEventEnvelope` | Message cannot be decrypted |
| `MediaRetryEventEnvelope` | Media download retry is requested |
| `FBMessageEventEnvelope` | Facebook message event |

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
    .AddHandler<MessageEventEnvelope, MessageHandler>(ServiceLifetime.Scoped)
    .AddHandler<ReceiptEventEnvelope, ReceiptHandler>(ServiceLifetime.Scoped)
    .AddHandler<GroupInfoEventEnvelope, GroupInfoHandler>(ServiceLifetime.Scoped)
    .AddHandler<PresenceEventEnvelope, PresenceHandler>(ServiceLifetime.Scoped)
);
```

### Shared Handler

A single class can handle multiple event types:

```csharp
public sealed class MultiEventHandler :
    IEventHandler<MessageEventEnvelope>,
    IEventHandler<ReceiptEventEnvelope>,
    IEventHandler<GroupInfoEventEnvelope>
{
    private readonly ILogger<MultiEventHandler> logger;

    public MultiEventHandler(ILogger<MultiEventHandler> logger)
    {
        this.logger = logger;
    }

    public Task HandleAsync(IWuzEventEnvelope<MessageEventEnvelope> envelope, CancellationToken ct)
    {
        var @event = envelope.Payload.Event;
        this.logger.LogInformation("Message received from {Sender}", @event.Info?.Sender);
        return Task.CompletedTask;
    }

    public Task HandleAsync(IWuzEventEnvelope<ReceiptEventEnvelope> envelope, CancellationToken ct)
    {
        var receiptEnvelope = envelope.Payload;
        this.logger.LogInformation("Receipt: {State}", receiptEnvelope.State);
        return Task.CompletedTask;
    }

    public Task HandleAsync(IWuzEventEnvelope<GroupInfoEventEnvelope> envelope, CancellationToken ct)
    {
        var @event = envelope.Payload.Event;
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
public sealed class MessageHandler : IEventHandler<MessageEventEnvelope>
{
    private readonly ILogger<MessageHandler> logger;

    public MessageHandler(ILogger<MessageHandler> logger)
    {
        this.logger = logger;
    }

    public async Task HandleAsync(IWuzEventEnvelope<MessageEventEnvelope> envelope, CancellationToken ct)
    {
        var @event = envelope.Payload.Event;

        // Filter: Only process messages from specific user
        if (envelope.Metadata.UserId != "expected-user-id")
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


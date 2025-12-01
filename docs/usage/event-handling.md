# Event Handling Guide

This guide explains how to process incoming WhatsApp events using WuzAPI Client's RabbitMQ event consumer.

## Overview

WhatsApp events (incoming messages, status updates, group changes, etc.) flow through this pipeline:

```
WhatsApp → asternic/wuzapi → RabbitMQ → EventConsumer → EventFilter → EventDispatcher → IEventHandler<T>
```

The library provides:
- **EventConsumer** – Background service consuming from RabbitMQ
- **EventDispatcher** – Routes events to registered handlers
- **IEventHandler\<TEvent>** – Interface for implementing event handlers
- **IEventFilter** – Interface for filtering events before processing
- **IEventErrorHandler** – Interface for customizing error handling

## Prerequisites

Before handling events:

1. **Running RabbitMQ instance** – See [RabbitMQ documentation](https://www.rabbitmq.com/documentation.html)
2. **asternic/wuzapi configured** – Publishing events to your RabbitMQ queue
3. **WuzApiClient.RabbitMq package** – Installed in your project

## Step 1: Register Event Consumer

In your `Program.cs`:

```csharp
using WuzApiClient.Events.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Register event consumer
builder.Services.AddWuzEvents(options =>
{
    options.ConnectionString = "amqp://guest:guest@localhost:5672/";
    options.QueueName = "wuzapi-events";
    options.MaxConcurrentMessages = 1; // See concurrency warning below
});

var app = builder.Build();
app.Run();
```

For production configuration, see [Configuration Reference](configuration.md).

## Step 2: Implement Event Handlers

Create a class implementing `IEventHandler<TEvent>`:

```csharp
using WuzApiClient.Core.Interfaces;
using WuzApiClient.Events.Core.Interfaces;
using WuzApiClient.Events.Models.Events;
using WuzApiClient.Models.Common;

public sealed class MessageReceivedHandler : IEventHandler<MessageEvent>
{
    private readonly ILogger<MessageReceivedHandler> logger;
    private readonly IWaClient client;

    public MessageReceivedHandler(
        ILogger<MessageReceivedHandler> logger,
        IWaClient client)
    {
        this.logger = logger;
        this.client = client;
    }

    public async Task HandleAsync(
        MessageEvent @event,
        CancellationToken cancellationToken)
    {
        var text = @event.Message?.Conversation ?? @event.Message?.ExtendedTextMessage?.Text;
        var sender = @event.Info?.Sender;

        this.logger.LogInformation(
            "Received message from {From}: {Text}",
            sender,
            text
        );

        // Echo the message back
        if (!string.IsNullOrEmpty(text) && sender != null)
        {
            // Extract phone number from JID (remove @s.whatsapp.net)
            var phoneNumber = sender.Split('@')[0];
            var phone = Phone.Create(phoneNumber);

            var result = await this.client.SendTextMessageAsync(
                phone,
                $"You said: {text}",
                cancellationToken: cancellationToken
            );

            if (result.IsFailure)
            {
                this.logger.LogError("Failed to send reply: {Error}", result.Error.Message);
            }
        }
    }
}
```

## Step 3: Register Event Handlers

Register your handlers in DI:

```csharp
// Register as scoped (new instance per message)
builder.Services.AddScoped<IEventHandler<MessageEvent>, MessageReceivedHandler>();

// You can register multiple handlers for different events
builder.Services.AddScoped<IEventHandler<ReceiptEvent>, MessageStatusHandler>();
builder.Services.AddScoped<IEventHandler<GroupInfoEvent>, GroupUpdateHandler>();
```

**Service Lifetime:** Handlers are resolved from a **scoped DI container** created per message. This ensures proper lifetime management for scoped dependencies.

## Event Types

WuzAPI Client includes 44 strongly-typed event classes. Common events:

| Event Type | Triggered When | Key Properties |
|------------|----------------|----------------|
| `MessageEvent` | Incoming message | `Info.Sender`, `Message.Conversation`, `Info.Type` |
| `ReceiptEvent` | Message status change | `MessageIDs`, `State`, `ReceiptType` |
| `GroupInfoEvent` | Group information update | `GroupJid`, `Name`, `Topic` |
| `PresenceEvent` | Contact online status | `From`, `Unavailable`, `State` |

For complete event reference, see [Event Types Reference](../api/event-types-reference.md).

## Handling Different Event Types

### Text Messages

```csharp
public sealed class MessageReceivedHandler : IEventHandler<MessageEvent>
{
    public async Task HandleAsync(MessageEvent @event, CancellationToken ct)
    {
        if (@event.Info?.Type == "text")
        {
            var text = @event.Message?.Conversation ?? @event.Message?.ExtendedTextMessage?.Text;
            // Process text message...
        }
    }
}
```

### Media Messages

```csharp
public async Task HandleAsync(MessageEvent @event, CancellationToken ct)
{
    switch (@event.Info?.Type)
    {
        case "image":
            var s3Info = @event.S3;
            var caption = @event.Message?.ImageMessage?.Caption;
            // Download and process image from S3...
            break;

        case "document":
            var fileName = @event.FileName;
            var documentS3 = @event.S3;
            // Download and process document from S3...
            break;

        case "audio":
            var audioS3 = @event.S3;
            // Download and process audio from S3...
            break;
    }
}
```

### Message Status Updates

```csharp
public sealed class MessageStatusHandler : IEventHandler<ReceiptEvent>
{
    public async Task HandleAsync(ReceiptEvent @event, CancellationToken ct)
    {
        switch (@event.State)
        {
            case "Delivered":
                // Message delivered to recipient
                break;
            case "Read":
                // Message read by recipient
                break;
            case "ReadSelf":
                // Message read by current user on another device
                break;
        }
    }
}
```

### Group Events

```csharp
public sealed class GroupUpdateHandler : IEventHandler<GroupInfoEvent>
{
    private readonly ILogger<GroupUpdateHandler> logger;

    public GroupUpdateHandler(ILogger<GroupUpdateHandler> logger)
    {
        this.logger = logger;
    }

    public async Task HandleAsync(GroupInfoEvent @event, CancellationToken ct)
    {
        this.logger.LogInformation(
            "Group info updated - JID: {GroupJid}, Name: {Name}, Topic: {Topic}",
            @event.GroupJid,
            @event.Name,
            @event.Topic
        );

        // Handle group information update
        // Note: For participant changes, use JoinedGroup event instead
    }
}
```

## Event Filtering

Filter events before they reach handlers using `IEventFilter`:

```csharp
using WuzApiClient.Events.Core.Interfaces;
using WuzApiClient.Events.Models.Events;

public sealed class IgnoreOwnMessagesFilter : IEventFilter
{
    private readonly string myPhoneNumber;

    public IgnoreOwnMessagesFilter(IConfiguration configuration)
    {
        this.myPhoneNumber = configuration["MyPhoneNumber"];
    }

    public int Order => 0;

    public bool ShouldProcess(WuzEvent @event)
    {
        // Filter out messages from our own number
        if (@event is MessageEvent message)
        {
            return message.Info?.Sender != $"{this.myPhoneNumber}@c.us";
        }

        // Process all other events
        return true;
    }
}
```

Register the filter:

```csharp
builder.Services.AddSingleton<IEventFilter, IgnoreOwnMessagesFilter>();
```

**Multiple Filters:** All registered filters must return `true` for an event to be processed. Filters run sequentially in registration order.

## Error Handling

### Default Behavior

By default, if a handler throws an exception:
1. Exception is logged
2. Message is **acknowledged** (removed from queue)
3. Processing continues with next message

### Custom Error Handling

Implement `IEventErrorHandler` to customize error behavior:

```csharp
using WuzApiClient.Events.Core.Interfaces;
using WuzApiClient.Events.Models.Events;

public sealed class CustomErrorHandler : IEventErrorHandler
{
    private readonly ILogger<CustomErrorHandler> logger;

    public CustomErrorHandler(ILogger<CustomErrorHandler> logger)
    {
        this.logger = logger;
    }

    public Task HandleErrorAsync(
        WuzEvent @event,
        Exception exception,
        CancellationToken cancellationToken)
    {
        this.logger.LogError(exception, "Error processing event {EventType}", @event.GetType().Name);

        // Custom error handling logic
        // Note: Error acknowledgment behavior is controlled by WuzEventOptions.ErrorBehavior
        // This handler is for logging, alerting, or other side effects

        return Task.CompletedTask;
    }
}
```

Register the error handler:

```csharp
builder.Services.AddSingleton<IEventErrorHandler, CustomErrorHandler>();
```

**ErrorBehavior Options (configured in WuzEventOptions):**
- `AcknowledgeOnError` – Acknowledge message even if handler fails (default)
- `RequeueOnError` – Reject and requeue message on handler failure
- `DeadLetterOnError` – Reject without requeue (dead-letter if configured)

### Dead Letter Queue

For failed messages, configure dead-letter exchange at the RabbitMQ queue level (not in WuzEventOptions). When using `DeadLetterOnError`, messages will be rejected and routed to the dead-letter exchange if one is configured for your queue.

See [RabbitMQ Dead Letter Exchanges](https://www.rabbitmq.com/docs/dlx) for details on queue-level configuration.

## Concurrency Configuration

> **Warning:** Setting `MaxConcurrentMessages > 1` enables parallel processing but **breaks message ordering guarantees**. Messages may be processed out of order.

```csharp
builder.Services.AddWuzEvents(options =>
{
    // Process one message at a time (preserves ordering)
    options.MaxConcurrentMessages = 1;

    // Or process multiple messages in parallel (faster, but no ordering)
    // options.MaxConcurrentMessages = 10;
});
```

**When to use parallel processing:**
- Events are independent and order doesn't matter
- High throughput is required
- Processing is I/O bound

**When to use sequential processing:**
- Message order must be preserved
- Events have dependencies on previous events
- Shared state needs synchronization

## Dependency Injection in Handlers

Handlers are resolved from a **scoped DI container** per message:

```csharp
public sealed class MessageReceivedHandler : IEventHandler<MessageEvent>
{
    // All standard DI lifetimes work
    private readonly IWaClient client;              // Scoped
    private readonly ILogger<MessageReceivedHandler> logger; // Singleton
    private readonly MyDbContext dbContext;             // Scoped

    public MessageReceivedHandler(
        IWaClient client,
        ILogger<MessageReceivedHandler> logger,
        MyDbContext dbContext)
    {
        this.client = client;
        this.logger = logger;
        this.dbContext = dbContext;
    }

    public async Task HandleAsync(MessageEvent @event, CancellationToken ct)
    {
        // Use scoped services safely
        var entity = new MyEntity { Message = @event.Body };
        this.dbContext.Add(entity);
        await this.dbContext.SaveChangesAsync(ct);
    }
}
```

## Testing Event Handlers

### Unit Testing

Mock the event and test handler logic:

```csharp
[Fact]
public async Task HandleAsync_LogsMessage()
{
    // Arrange
    var loggerMock = new Mock<ILogger<MessageReceivedHandler>>();
    var clientMock = new Mock<IWaClient>();
    var handler = new MessageReceivedHandler(loggerMock.Object, clientMock.Object);

    var @event = new MessageEvent
    {
        Info = new MessageInfo
        {
            Sender = "5511999999999@c.us",
            Type = "text"
        },
        Message = new MessageContent
        {
            Conversation = "Hello"
        }
    };

    // Act
    await handler.HandleAsync(@event, CancellationToken.None);

    // Assert
    loggerMock.Verify(
        x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Hello")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()),
        Times.Once);
}
```

### Integration Testing

Use RabbitMQ test container:

```csharp
public sealed class EventHandlerIntegrationTests : IClassFixture<RabbitMqFixture>
{
    [Fact]
    public async Task EventConsumer_ProcessesMessage()
    {
        // Setup test host with real RabbitMQ
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddWuzEvents(options =>
                {
                    options.HostName = "localhost";
                    options.QueueName = "test-queue";
                });
                services.AddScoped<IEventHandler<MessageEvent>, TestHandler>();
            })
            .Build();

        await host.StartAsync();

        // Publish test message to RabbitMQ
        // ... assert handler was invoked
    }
}
```

## Performance Considerations

### Message Throughput

Factors affecting throughput:
- `MaxConcurrentMessages` setting
- Handler processing time
- RabbitMQ prefetch count
- Network latency

### Memory Usage

Each concurrent message creates a scoped DI container. Monitor memory with high `MaxConcurrentMessages`.

### Backpressure

If handlers are slow, messages queue in RabbitMQ. Consider:
- Increasing `MaxConcurrentMessages` if CPU allows
- Optimizing handler logic
- Scaling to multiple consumer instances

## Common Patterns

### Command Pattern

Route text messages to commands:

```csharp
public sealed class CommandHandler : IEventHandler<MessageEvent>
{
    public async Task HandleAsync(MessageEvent @event, CancellationToken ct)
    {
        var text = @event.Message?.Conversation ?? @event.Message?.ExtendedTextMessage?.Text;

        if (@event.Info?.Type != "text" || string.IsNullOrEmpty(text))
            return;

        var command = text.Split(' ')[0].ToLower();

        switch (command)
        {
            case "/start":
                await this.HandleStartCommand(@event, ct);
                break;
            case "/help":
                await this.HandleHelpCommand(@event, ct);
                break;
            case "/status":
                await this.HandleStatusCommand(@event, ct);
                break;
        }
    }
}
```

### State Machine

Track conversation state per user:

```csharp
public sealed class ConversationHandler : IEventHandler<MessageEvent>
{
    private readonly IConversationStateRepository stateRepo;

    public ConversationHandler(IConversationStateRepository stateRepo)
    {
        this.stateRepo = stateRepo;
    }

    public async Task HandleAsync(MessageEvent @event, CancellationToken ct)
    {
        var sender = @event.Info?.Sender;
        if (sender == null) return;

        var state = await this.stateRepo.GetStateAsync(sender);

        switch (state)
        {
            case ConversationState.AwaitingName:
                await this.ProcessNameInput(@event, ct);
                await this.stateRepo.SetStateAsync(sender, ConversationState.AwaitingEmail);
                break;

            case ConversationState.AwaitingEmail:
                await this.ProcessEmailInput(@event, ct);
                await this.stateRepo.SetStateAsync(sender, ConversationState.Complete);
                break;
        }
    }
}
```

## Troubleshooting

### Handler Not Invoked

**Symptoms:** Events arrive in RabbitMQ but handler doesn't execute

**Solutions:**
1. Verify handler is registered: `services.AddScoped<IEventHandler<TEvent>, THandler>()`
2. Check event type matches: `IEventHandler<MessageEvent>` only handles that event type
3. Verify event filters aren't blocking: Check `IEventFilter.ShouldProcess` returns true
4. Check logs for exceptions during handler resolution

## Next Steps

- **Filter Events** → [Filtering Guide](filtering.md)
- **Configure Options** → [Configuration Reference](configuration.md)
- **Handle Errors** → [Error Handling Guide](error-handling.md)
- **Event Types** → [Event Types Reference](../api/event-types-reference.md)


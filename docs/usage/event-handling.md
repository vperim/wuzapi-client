# Event Handling Guide

This guide explains how to process incoming WhatsApp events using WuzAPI Client's RabbitMQ event consumer.

## Overview

WhatsApp events (incoming messages, status updates, group changes, etc.) flow through this pipeline:

```
WhatsApp → asternic/wuzapi → RabbitMQ → EventConsumer → EventDispatcher → IEventHandler<T>
```

The library provides:
- **EventConsumer** – Background service consuming from RabbitMQ
- **EventDispatcher** – Routes events to registered handlers
- **IEventHandler\<TEvent>** – Interface for implementing event handlers

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

// Register event consumer with fluent builder
builder.Services.AddWuzEvents(builder.Configuration, b => b
    .AddHandlersFromAssembly(ServiceLifetime.Scoped, typeof(Program).Assembly)
);

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

public sealed class MessageReceivedHandler : IEventHandler<MessageEventEnvelope>
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
        IWuzEventEnvelope<MessageEventEnvelope> envelope,
        CancellationToken cancellationToken)
    {
        var @event = envelope.Payload.Event;
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

Register your handlers using the fluent builder API:

```csharp
// Using assembly scanning (recommended - automatically discovers all handlers)
builder.Services.AddWuzEvents(builder.Configuration, b => b
    .AddHandlersFromAssembly(ServiceLifetime.Scoped, typeof(Program).Assembly)
);

// Or register individual handlers explicitly
builder.Services.AddWuzEvents(builder.Configuration, b => b
    .AddHandler<MessageEventEnvelope, MessageReceivedHandler>(ServiceLifetime.Scoped)
    .AddHandler<ReceiptEventEnvelope, MessageStatusHandler>(ServiceLifetime.Scoped)
    .AddHandler<GroupInfoEventEnvelope, GroupUpdateHandler>(ServiceLifetime.Scoped)
);
```

**Service Lifetime:** Handlers are resolved from a **scoped DI container** created per message. This ensures proper lifetime management for scoped dependencies. The default lifetime is `Scoped` but can be overridden per handler or for all handlers in an assembly.

## Event Types

WuzAPI Client includes multiple strongly-typed event classes.
For complete event reference, see [Event Types Reference](../api/event-types-reference.md).

## Handling Different Event Types

### Text Messages

```csharp
public sealed class MessageReceivedHandler : IEventHandler<MessageEventEnvelope>
{
    public async Task HandleAsync(IWuzEventEnvelope<MessageEventEnvelope> envelope, CancellationToken ct)
    {
        var @event = envelope.Payload.Event;
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
public async Task HandleAsync(IWuzEventEnvelope<MessageEventEnvelope> envelope, CancellationToken ct)
{
    var @event = envelope.Payload.Event;
    var messageEnvelope = envelope.Payload; // Access envelope-level properties

    switch (@event.Info?.Type)
    {
        case "image":
            var s3Info = messageEnvelope.S3;
            var caption = @event.Message?.ImageMessage?.Caption;
            // Download and process image from S3...
            break;

        case "document":
            var fileName = messageEnvelope.FileName;
            var documentS3 = messageEnvelope.S3;
            // Download and process document from S3...
            break;

        case "audio":
            var audioS3 = messageEnvelope.S3;
            // Download and process audio from S3...
            break;
    }
}
```

### Message Status Updates

```csharp
public sealed class MessageStatusHandler : IEventHandler<ReceiptEventEnvelope>
{
    public async Task HandleAsync(IWuzEventEnvelope<ReceiptEventEnvelope> envelope, CancellationToken ct)
    {
        var receiptEnvelope = envelope.Payload; // Envelope has State property
        switch (receiptEnvelope.State)
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
public sealed class GroupUpdateHandler : IEventHandler<GroupInfoEventEnvelope>
{
    private readonly ILogger<GroupUpdateHandler> logger;

    public GroupUpdateHandler(ILogger<GroupUpdateHandler> logger)
    {
        this.logger = logger;
    }

    public async Task HandleAsync(IWuzEventEnvelope<GroupInfoEventEnvelope> envelope, CancellationToken ct)
    {
        var @event = envelope.Payload.Event;
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


## Error Handling

### Default Behavior

By default, if a handler throws an exception:
1. Exception is logged to the configured logger
2. Message is **acknowledged** (removed from queue)
3. Processing continues with next message

**Custom Error Logic:** Implement error handling within your event handlers using try-catch blocks:

```csharp
public sealed class ResilientMessageHandler : IEventHandler<MessageEventEnvelope>
{
    private readonly ILogger<ResilientMessageHandler> logger;

    public ResilientMessageHandler(ILogger<ResilientMessageHandler> logger)
    {
        this.logger = logger;
    }

    public async Task HandleAsync(IWuzEventEnvelope<MessageEventEnvelope> envelope, CancellationToken ct)
    {
        var @event = envelope.Payload.Event;
        try
        {
            // Process message...
            await ProcessMessageAsync(@event, ct);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to process message from {Sender}", @event.Info?.Sender);

            // Custom error handling:
            // - Send alert
            // - Store in error queue
            // - Retry with backoff
            // etc.
        }
    }
}
```

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
public sealed class MessageReceivedHandler : IEventHandler<MessageEventEnvelope>
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

    public async Task HandleAsync(IWuzEventEnvelope<MessageEventEnvelope> envelope, CancellationToken ct)
    {
        var @event = envelope.Payload.Event;
        // Use scoped services safely
        var entity = new MyEntity { Message = @event.Body };
        this.dbContext.Add(entity);
        await this.dbContext.SaveChangesAsync(ct);
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
public sealed class CommandHandler : IEventHandler<MessageEventEnvelope>
{
    public async Task HandleAsync(IWuzEventEnvelope<MessageEventEnvelope> envelope, CancellationToken ct)
    {
        var @event = envelope.Payload.Event;
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
public sealed class ConversationHandler : IEventHandler<MessageEventEnvelope>
{
    private readonly IConversationStateRepository stateRepo;

    public ConversationHandler(IConversationStateRepository stateRepo)
    {
        this.stateRepo = stateRepo;
    }

    public async Task HandleAsync(IWuzEventEnvelope<MessageEventEnvelope> envelope, CancellationToken ct)
    {
        var @event = envelope.Payload.Event;
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
1. Verify handler is registered via fluent builder or is discovered by assembly scanning
2. Check event type matches: `IEventHandler<MessageEvent>` only handles that event type
3. Check logs for exceptions during handler resolution or execution

## Next Steps

- **Configure Options** → [Configuration Reference](configuration.md)
- **Handle Errors** → [Error Handling Guide](error-handling.md)
- **Event Types** → [Event Types Reference](../api/event-types-reference.md)


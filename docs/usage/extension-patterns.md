# Extension Patterns

This guide documents all extension points in WuzAPI Client and how to use them to customize behavior.

## Overview

WuzAPI Client provides clear extension points for customization:

| Extension Point | Purpose | Priority |
|----------------|---------|----------|
| `IEventHandler<TEvent>` | Handle WhatsApp events | High |
| `WuzEventBuilder` | Fluent API for handler registration | High |
| `WuzApiOptions` | Configure HTTP client | High |
| `WuzEventOptions` | Configure RabbitMQ consumer | High |
| Custom JSON Converters | Add serialization for custom types | Low |

## Extension Point 1: IEventHandler\<TEvent>

**Location:** `WuzApiClient.RabbitMq/Core/Interfaces/IEventHandler.cs`

**Purpose:** Process specific WhatsApp event types

**When to Use:**
- Handle incoming messages
- React to message status updates
- Process group membership changes
- Track contact presence updates

### Interface Definition

```csharp
namespace WuzApiClient.Events.Core.Interfaces;

public interface IEventHandler<TEvent> where TEvent : WuzEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}
```

### Implementation Example

```csharp
using WuzApiClient.Core.Interfaces;
using WuzApiClient.Events.Core.Interfaces;
using WuzApiClient.Events.Models.Events;
using WuzApiClient.Models.Common;

public sealed class WelcomeMessageHandler : IEventHandler<MessageEvent>
{
    private readonly IWaClient client;
    private readonly ILogger<WelcomeMessageHandler> logger;

    public WelcomeMessageHandler(
        IWaClient client,
        ILogger<WelcomeMessageHandler> logger)
    {
        this.client = client;
        this.logger = logger;
    }

    public async Task HandleAsync(
        MessageEvent @event,
        CancellationToken cancellationToken)
    {
        // Extract text from message (could be in Conversation or ExtendedTextMessage)
        var text = @event.Message?.Conversation
            ?? @event.Message?.ExtendedTextMessage?.Text;

        if (!string.IsNullOrWhiteSpace(text) &&
            text.ToLower().Contains("hello"))
        {
            var sender = @event.Info?.Sender; // JID like "5511999999999@s.whatsapp.net"
            if (sender == null) return;

            this.logger.LogInformation("Sending welcome message to {Sender}", sender);

            // Extract phone number from JID (remove @s.whatsapp.net)
            var phoneNumber = sender.Split('@')[0];
            var phone = Phone.Create(phoneNumber);

            var result = await this.client.SendTextMessageAsync(
                phone,
                "Welcome! How can I help you today?",
                cancellationToken: cancellationToken
            );

            if (result.IsFailure)
            {
                this.logger.LogError("Failed to send welcome: {Error}", result.Error.Message);
            }
        }
    }
}
```

### Registration

```csharp
// Using assembly scanning (recommended - automatically discovers all handlers)
builder.Services.AddWuzEvents(builder.Configuration, b => b
    .AddHandlersFromAssembly(ServiceLifetime.Scoped, typeof(Program).Assembly)
);

// Or register individual handlers explicitly
builder.Services.AddWuzEvents(builder.Configuration, b => b
    .AddHandler<MessageEvent, WelcomeMessageHandler>(ServiceLifetime.Scoped)
    .AddHandler<MessageEvent, LoggingHandler>(ServiceLifetime.Scoped)
    .AddHandler<MessageEvent, AnalyticsHandler>(ServiceLifetime.Scoped)
);
```

### Constraints

- Handlers **must** complete within RabbitMQ timeout (default: 30 seconds)
- Long-running operations should be queued to background jobs
- Handlers are resolved per message from scoped DI container
- Unhandled exceptions trigger `IEventErrorHandler`

### Anti-Patterns

❌ **DO NOT block thread:**
```csharp
public async Task HandleAsync(MessageEvent @event, CancellationToken ct)
{
    Thread.Sleep(10000); // Blocks thread pool
}
```

✅ **DO use async properly:**
```csharp
public async Task HandleAsync(MessageEvent @event, CancellationToken ct)
{
    await Task.Delay(10000, ct); // Properly async
}
```

## Extension Point 2: WuzEventBuilder

**Location:** `WuzApiClient.RabbitMq/Configuration/WuzEventBuilder.cs`

**Purpose:** Fluent API for registering event handlers

**When to Use:**
- Registering multiple event handlers
- Using assembly scanning to auto-discover handlers
- Controlling handler service lifetimes

### Interface Definition

```csharp
namespace WuzApiClient.RabbitMq.Configuration;

public sealed class WuzEventBuilder
{
    public WuzEventBuilder AddHandler<TEvent, THandler>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TEvent : class
        where THandler : class, IEventHandler<TEvent>;

    public WuzEventBuilder AddHandlersFromAssembly(params Assembly[] assemblies);

    public WuzEventBuilder AddHandlersFromAssembly(ServiceLifetime lifetime, params Assembly[] assemblies);
}
```

### Usage Examples

**Assembly Scanning (Recommended):**

```csharp
builder.Services.AddWuzEvents(builder.Configuration, b => b
    .AddHandlersFromAssembly(ServiceLifetime.Scoped, typeof(Program).Assembly)
);
```

**Individual Handler Registration:**

```csharp
builder.Services.AddWuzEvents(builder.Configuration, b => b
    .AddHandler<MessageEvent, MessageHandler>(ServiceLifetime.Scoped)
    .AddHandler<ReceiptEvent, ReceiptHandler>(ServiceLifetime.Scoped)
    .AddHandler<GroupInfoEvent, GroupHandler>(ServiceLifetime.Singleton)
);
```

**Mixed Approach:**

```csharp
builder.Services.AddWuzEvents(builder.Configuration, b => b
    // Scan for most handlers
    .AddHandlersFromAssembly(ServiceLifetime.Scoped, typeof(Program).Assembly)
    // Override lifetime for specific handler
    .AddHandler<CriticalMessageEvent, CriticalHandler>(ServiceLifetime.Singleton)
);
```

### Benefits

- **Less boilerplate** - No need to register each handler individually
- **Compile-time safety** - Handlers must implement `IEventHandler<T>`
- **Lifetime control** - Specify lifetime per handler or for entire assembly
- **Discoverable** - Assembly scanning finds all handlers automatically

## Extension Point 3: WuzApiOptions

**Location:** `WuzApiClient/Configuration/WuzApiOptions.cs`

**Purpose:** Configure HTTP client behavior

**When to Use:**
- Set gateway URL and authentication
- Configure timeouts
- Customize HTTP client behavior

### Properties

```csharp
public class WuzApiOptions
{
    public string BaseUrl { get; set; }
    public string UserToken { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
}
```

### Configuration Example

```csharp
builder.Services.AddWuzApiClient(options =>
{
    options.BaseUrl = "https://wuzapi-gateway.internal";
    options.UserToken = configuration["WuzApi:UserToken"];
    options.TimeoutSeconds = 60;
});
```

## Extension Point 4: WuzEventOptions

**Location:** `WuzApiClient.RabbitMq/Configuration/WuzEventOptions.cs`

**Purpose:** Configure RabbitMQ consumer behavior

**When to Use:**
- Set RabbitMQ connection details
- Configure concurrency
- Customize reconnection behavior

### Properties

```csharp
namespace WuzApiClient.RabbitMq.Configuration;

public class WuzEventOptions
{
    public const string SectionName = "WuzEvents";

    public string ConnectionString { get; set; } = string.Empty;
    public string QueueName { get; set; } = "whatsapp_events";
    public string ConsumerTagPrefix { get; set; } = "wuzapi-consumer";
    public ushort PrefetchCount { get; set; } = 10;
    public bool AutoAck { get; set; } = false;
    public int MaxReconnectAttempts { get; set; } = 10;
    public TimeSpan ReconnectDelay { get; set; } = TimeSpan.FromSeconds(3);
    public int MaxConcurrentMessages { get; set; } = Environment.ProcessorCount;
}
```

### Configuration Example

```csharp
// Configuration is loaded from appsettings.json "WuzEvents" section
builder.Services.AddWuzEvents(builder.Configuration, b => b
    .AddHandlersFromAssembly(ServiceLifetime.Scoped, typeof(Program).Assembly)
);

// Advanced: Override specific options
builder.Services.Configure<WuzEventOptions>(options =>
{
    options.MaxConcurrentMessages = 10;
    options.PrefetchCount = 20;
    options.MaxReconnectAttempts = 5;
});
```

## Extension Point 5: Custom JSON Converters

**Location:** `WuzApiClient/Json/Converters/`

**Purpose:** Add custom serialization for domain types

**When to Use:**
- Add support for custom value types
- Customize serialization format
- Handle legacy data formats

### Example: Custom Phone Number Converter

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class CustomPhoneConverter : JsonConverter<Phone>
{
    public override Phone Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var value = reader.GetString();

        // Custom parsing logic - Phone.TryCreate normalizes and validates
        if (Phone.TryCreate(value, out var phone))
        {
            return phone;
        }

        throw new JsonException($"Invalid phone number: {value}");
    }

    public override void Write(
        Utf8JsonWriter writer,
        Phone value,
        JsonSerializerOptions options)
    {
        // Custom formatting logic - Phone has a Value property (string)
        writer.WriteStringValue(value.Value);
    }
}
```

### Registration

```csharp
// Register the WuzAPI client
builder.Services.AddWuzApiClient(options =>
{
    options.BaseUrl = Configuration["WuzApi:BaseUrl"];
    options.UserToken = Configuration["WuzApi:UserToken"];
});

// Note: Custom JSON converters should be configured via the internal WuzApiHttpClient implementation
// or by providing a custom implementation. The AddWuzApiClient method returns IServiceCollection,
// not IHttpClientBuilder, so it cannot be chained with ConfigureHttpClient.
```

See [System.Text.Json Custom Converters](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to) for details on creating custom converters.

## Combining Extension Points

Example: Complete event processing pipeline

```csharp
// 1. Register HTTP client factories
builder.Services.AddWuzApi(options =>
{
    options.BaseUrl = configuration["WuzApi:BaseUrl"];
    options.TimeoutSeconds = 30;
});

// 2. Register event consumer with handlers
builder.Services.AddWuzEvents(builder.Configuration, b => b
    .AddHandlersFromAssembly(ServiceLifetime.Scoped, typeof(Program).Assembly)
);
```

Processing flow:

```
RabbitMQ → EventConsumer
              ↓
         Deserialize to WuzEventEnvelope<T>
              ↓
         EventDispatcher
              ↓
         IEventHandler<T>.HandleAsync()
              ↓
         EventConsumer acknowledges message
```

## Testing Extensions

### Unit Test Event Handler

```csharp
[Fact]
public async Task Handler_SendsReply_OnHelloMessage()
{
    // Arrange
    var clientMock = new Mock<IWaClient>();
    clientMock
        .Setup(x => x.SendTextMessageAsync(
            It.IsAny<string>(),
            "Welcome! How can I help you today?",
            default))
        .ReturnsAsync(WuzResult<SendMessageResponse>.Success(new()));

    var handler = new WelcomeMessageHandler(clientMock.Object, Mock.Of<ILogger<WelcomeMessageHandler>>());

    var @event = new MessageEvent
    {
        Type = "text",
        Body = "Hello",
        From = "123@c.us"
    };

    // Act
    await handler.HandleAsync(@event, CancellationToken.None);

    // Assert
    clientMock.Verify(
        x => x.SendTextMessageAsync("123@c.us", It.IsAny<string>(), default),
        Times.Once);
}
```


## Best Practices

1. **Keep handlers focused** – One responsibility per handler
2. **Use DI liberally** – Inject dependencies, don't create them
3. **Make filters fast** – Avoid async I/O
4. **Test extension code** – Unit test handlers and filters
5. **Log important events** – Aid debugging in production
6. **Handle cancellation** – Respect CancellationToken
7. **Use scoped services** – Handlers get scoped container

## Next Steps

- **Handle Events** → [Event Handling Guide](event-handling.md)
- **Configure Options** → [Configuration Reference](configuration.md)
- **Handle Errors** → [Error Handling Guide](error-handling.md)


# Extension Patterns

This guide documents all extension points in WuzAPI Client and how to use them to customize behavior.

## Overview

WuzAPI Client provides clear extension points for customization:

| Extension Point | Purpose | Priority |
|----------------|---------|----------|
| `IEventHandler<TEvent>` | Handle WhatsApp events | High |
| `IEventFilter` | Filter events before processing | High |
| `IEventErrorHandler` | Customize error handling | Medium |
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
    private readonly IWuzApiClient client;
    private readonly ILogger<WelcomeMessageHandler> logger;

    public WelcomeMessageHandler(
        IWuzApiClient client,
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
// Register as scoped (new instance per message)
builder.Services.AddScoped<IEventHandler<MessageEvent>, WelcomeMessageHandler>();

// Multiple handlers for same event type are supported
builder.Services.AddScoped<IEventHandler<MessageEvent>, LoggingHandler>();
builder.Services.AddScoped<IEventHandler<MessageEvent>, AnalyticsHandler>();
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

## Extension Point 2: IEventFilter

**Location:** `WuzApiClient.RabbitMq/Core/Interfaces/IEventFilter.cs`

**Purpose:** Filter events before they reach handlers

**When to Use:**
- Ignore messages from specific senders
- Skip events during maintenance windows
- Filter by event properties
- Implement rate limiting per sender

### Interface Definition

```csharp
namespace WuzApiClient.Events.Core.Interfaces;

public interface IEventFilter
{
    bool ShouldProcess(WuzEvent @event);
    int Order { get; }
}
```

**Return Value:**
- `true` – Process the event (dispatch to handlers)
- `false` – Skip the event (acknowledge without processing)

### Implementation Example

```csharp
using WuzApiClient.Events.Core.Interfaces;
using WuzApiClient.Events.Models.Events;

public sealed class BusinessHoursFilter : IEventFilter
{
    private readonly ILogger<BusinessHoursFilter> logger;
    private readonly TimeProvider timeProvider;

    public BusinessHoursFilter(ILogger<BusinessHoursFilter> logger, TimeProvider timeProvider)
    {
        this.logger = logger;
        this.timeProvider = timeProvider;
    }

    public int Order => 0; // Execute first

    public bool ShouldProcess(WuzEvent @event)
    {
        var now = this.timeProvider.GetLocalNow();
        var isBusinessHours = now.Hour >= 9 && now.Hour < 18
            && now.DayOfWeek != DayOfWeek.Saturday
            && now.DayOfWeek != DayOfWeek.Sunday;

        if (!isBusinessHours)
        {
            this.logger.LogInformation(
                "Skipping event {EventType} outside business hours",
                @event.GetType().Name
            );
        }

        return isBusinessHours;
    }
}
```

### Registration

```csharp
// Filters typically registered as scoped (default)
builder.Services.AddScoped<IEventFilter, BusinessHoursFilter>();

// All registered filters must return true for event to process
```

### Constraints

- Filters run **sequentially** ordered by `Order` property (lower = earlier execution)
- **All filters** must return `true` for event to be processed
- Filters should be **fast** (synchronous, no async I/O)
- Filters are resolved from **scoped** DI container (per-message)

## Extension Point 3: IEventErrorHandler

**Location:** `WuzApiClient.RabbitMq/Core/Interfaces/IEventErrorHandler.cs`

**Purpose:** Customize error handling when handlers fail

**When to Use:**
- Implement retry logic for transient errors
- Route different errors to different behaviors
- Send alerts on specific error types
- Log errors with custom context

### Interface Definition

```csharp
namespace WuzApiClient.Events.Core.Interfaces;

public interface IEventErrorHandler
{
    Task HandleErrorAsync(
        WuzEvent @event,
        Exception exception,
        CancellationToken cancellationToken);
}
```

### Implementation Example

```csharp
using WuzApiClient.Events.Core.Interfaces;
using WuzApiClient.Events.Models.Events;

public sealed class SmartErrorHandler : IEventErrorHandler
{
    private readonly ILogger<SmartErrorHandler> logger;
    private readonly INotificationService notifications;

    public SmartErrorHandler(
        ILogger<SmartErrorHandler> logger,
        INotificationService notifications)
    {
        this.logger = logger;
        this.notifications = notifications;
    }

    public async Task HandleErrorAsync(
        WuzEvent @event,
        Exception exception,
        CancellationToken cancellationToken)
    {
        this.logger.LogError(
            exception,
            "Error processing {EventType} event",
            @event.GetType().Name
        );

        // Critical errors - send alert
        if (exception is InvalidOperationException)
        {
            this.logger.LogCritical("Critical error detected");
            await this.notifications.SendAlertAsync(
                $"Critical error processing WhatsApp event: {exception.Message}",
                cancellationToken
            );
        }
        else if (exception is TimeoutException or HttpRequestException)
        {
            this.logger.LogWarning("Transient error detected");
        }
        else if (exception is JsonException or ArgumentException)
        {
            this.logger.LogWarning("Data error detected");
        }
    }
}
```

### Registration

```csharp
builder.Services.AddSingleton<IEventErrorHandler, SmartErrorHandler>();
```

### Constraints

- Only **one** error handler can be registered
- Error handler is resolved from **singleton** DI container
- Should be **fast** (avoid long-running operations)
- Error handler is for logging/alerting only - it does not control message acknowledgment behavior

## Extension Point 4: WuzApiOptions

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

## Extension Point 5: WuzEventOptions

**Location:** `WuzApiClient.RabbitMq/Configuration/WuzEventOptions.cs`

**Purpose:** Configure RabbitMQ consumer behavior

**When to Use:**
- Set RabbitMQ connection details
- Configure concurrency
- Customize reconnection behavior

### Properties

```csharp
namespace WuzApiClient.Events.Configuration;

public class WuzEventOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string QueueName { get; set; } = "whatsapp_events";
    public string ConsumerTagPrefix { get; set; } = "wuzapi-consumer";
    public ushort PrefetchCount { get; set; } = 10;
    public bool AutoAck { get; set; } = false;
    public int MaxReconnectAttempts { get; set; } = 10;
    public TimeSpan ReconnectDelay { get; set; } = TimeSpan.FromSeconds(3);
    public int MaxConcurrentMessages { get; set; } = Environment.ProcessorCount;
    public HashSet<string> SubscribedEventTypes { get; set; } = [];
    public HashSet<string> FilterUserIds { get; set; } = [];
    public HashSet<string> FilterInstanceNames { get; set; } = [];
}
```

### Configuration Example

```csharp
builder.Services.AddWuzEvents(options =>
{
    options.ConnectionString = "amqp://user:pass@rabbitmq.internal:5672/vhost";
    options.QueueName = "wuzapi-events";
    options.MaxConcurrentMessages = 10;
    options.PrefetchCount = 20;
    options.MaxReconnectAttempts = 5;
    options.SubscribedEventTypes = ["message", "message.ack"];
    options.FilterUserIds = ["user1", "user2"];
});
```

## Extension Point 6: Custom JSON Converters

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
// 1. Register HTTP client
builder.Services.AddWuzApiClient(options =>
{
    options.BaseUrl = configuration["WuzApi:BaseUrl"];
    options.UserToken = configuration["WuzApi:UserToken"];
});

// 2. Register event consumer
builder.Services.AddWuzEvents(options =>
{
    configuration.GetSection("RabbitMq").Bind(options);
});

// 3. Register filters
builder.Services.AddSingleton<IEventFilter, BusinessHoursFilter>();

// 4. Register error handler
builder.Services.AddSingleton<IEventErrorHandler, SmartErrorHandler>();

// 5. Register event handlers
builder.Services.AddScoped<IEventHandler<MessageEvent>, CommandHandler>();
builder.Services.AddScoped<IEventHandler<MessageStatusEvent>, StatusTracker>();
builder.Services.AddScoped<IEventHandler<GroupUpdateEvent>, GroupMonitor>();
```

Processing flow:

```
RabbitMQ → EventConsumer
              ↓
         Deserialize to WuzEvent
              ↓
         BusinessHoursFilter (pass)
              ↓
         EventDispatcher
              ↓
         CommandHandler.HandleAsync() [throws exception]
              ↓
         SmartErrorHandler.HandleErrorAsync()
              ↓
         EventConsumer nacks message
```

## Testing Extensions

### Unit Test Event Handler

```csharp
[Fact]
public async Task Handler_SendsReply_OnHelloMessage()
{
    // Arrange
    var clientMock = new Mock<IWuzApiClient>();
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

### Unit Test Filter

```csharp
[Fact]
public void Filter_ReturnsTrue_DuringBusinessHours()
{
    // Arrange
    var filter = new BusinessHoursFilter(Mock.Of<ILogger<BusinessHoursFilter>>(), TimeProvider.System);
    var @event = new MessageEvent();

    // Act (assuming test runs during business hours)
    var result = filter.ShouldProcess(@event);

    // Assert
    Assert.True(result);
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


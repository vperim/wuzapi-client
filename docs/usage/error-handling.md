# Error Handling Guide

WuzAPI Client uses **railway-oriented programming** with the Result pattern instead of throwing exceptions for business logic errors. This guide explains how to handle errors effectively.

## Result Pattern Overview

All HTTP client methods return `WuzResult<T>` or `WuzResult` (for void operations):

```csharp
public sealed class WuzResult<T>
{
    public bool IsSuccess { get; }    // True if operation succeeded
    public bool IsFailure { get; }    // True if operation failed (opposite of IsSuccess)
    public T Value { get; }           // Result value (only if IsSuccess)
    public WuzApiError Error { get; } // Error object (only if IsFailure)
}
```

## Basic Error Handling

### Check Success Before Accessing Value

```csharp
var result = await this.client.SendTextMessageAsync(chatId, text);

if (result.IsSuccess)
{
    // Safe to access Value
    var messageId = result.Value.MessageId;
    this.logger.LogInformation("Message sent: {MessageId}", messageId);
}
else
{
    // Handle error
    this.logger.LogError("Failed to send message: {Error}", result.Error.Message);
}
```

### Using IsFailure

```csharp
var result = await this.client.GetContactsAsync();

if (result.IsFailure)
{
    // Early return on error
    this.logger.LogError("Failed to get contacts: {Error}", result.Error.Message);
    return;
}

// Success path - use result.Value
var contacts = result.Value.Contacts;
foreach (var contact in contacts)
{
    Console.WriteLine(contact.Value.FullName ?? contact.Value.PushName ?? "Unknown");
}
```

## Why Result Pattern?

### Traditional Approach (Exceptions)

```csharp
// Caller doesn't know this can fail
try
{
    var response = await client.SendMessageAsync(chatId, text);
    // Use response...
}
catch (HttpRequestException ex)
{
    // Handle network error
}
catch (JsonException ex)
{
    // Handle parsing error
}
catch (Exception ex)
{
    // Catch-all
}
```

**Problems:**
- Error handling is not explicit in method signature
- Easy to forget try-catch blocks
- Performance overhead of throwing exceptions
- Control flow via exceptions is an anti-pattern

### WuzAPI Approach (Result Pattern)

```csharp
// Caller knows this returns a result
var result = await this.client.SendMessageAsync(chatId, text);

if (result.IsSuccess)
{
    // Use result.Value
}
else
{
    // Handle result.Error.Message
}
```

**Benefits:**
- Explicit error handling in return type
- Cannot forget to check (compiler enforces)
- No exception performance overhead
- Composable error flows
- Type-safe (cannot access Value on failure)

## Error Categories

WuzAPI Client returns failures for these categories:

| Category | Example Error Code | Example Message |
|----------|-------------------|-----------------|
| **Network Errors** | `NetworkError`, `Timeout` | "Connection failed", "Request timed out" |
| **Authentication Errors** | `Unauthorized` | "Invalid token" |
| **Validation Errors** | `InvalidRequest`, `InvalidPhoneNumber` | "Invalid phone number format: {phone}" |
| **Gateway Errors** | `SessionNotReady`, `AlreadyLoggedIn` | "Session not ready", "Session is already logged in" |
| **Serialization Errors** | `DeserializationError` | "JSON parsing error: {details}" |

## Handling Different Error Types

### Pattern Matching on Error Code

```csharp
var result = await this.client.SendTextMessageAsync(Phone.Parse(chatId), text);

if (result.IsFailure)
{
    if (result.Error.Code == WuzApiErrorCode.Unauthorized)
    {
        this.logger.LogError("Authentication failed. Check UserToken configuration.");
        // Refresh token or notify admin
    }
    else if (result.Error.Code == WuzApiErrorCode.Timeout)
    {
        this.logger.LogWarning("Request timed out. Retrying...");
        // Retry logic
    }
    else
    {
        this.logger.LogError("Error [{Code}]: {Message}", result.Error.Code, result.Error.Message);
    }
}
```

### Logging Errors

```csharp
var result = await this.client.SendTextMessageAsync(Phone.Parse(chatId), text);

if (result.IsFailure)
{
    this.logger.LogError(
        "Failed to send message to {ChatId}: {Error}",
        chatId,
        result.Error.Message
    );

    // Optionally include error code and context
    this.logger.LogError(
        "Send message failed. ChatId: {ChatId}, Text: {Text}, Code: {Code}, Error: {Error}",
        chatId,
        text,
        result.Error.Code,
        result.Error.Message
    );
}
```

### Returning Errors to Callers

```csharp
public async Task<ActionResult> SendMessage(SendMessageRequest request)
{
    var result = await this.client.SendTextMessageAsync(
        Phone.Parse(request.ChatId),
        request.Text
    );

    if (result.IsFailure)
    {
        return BadRequest(new
        {
            error = "Failed to send message",
            code = result.Error.Code.ToString(),
            details = result.Error.Message
        });
    }

    return Ok(new
    {
        messageId = result.Value.MessageId,
        timestamp = result.Value.Timestamp
    });
}
```

## Retry Patterns

> **Important:** WuzAPI Client does NOT include built-in retry logic. Consumers should implement their own retry and resilience patterns using libraries like [Polly](https://www.pollydocs.org/).

## Event Handler Error Handling

Event handlers can log and handle errors via `IEventErrorHandler`:

```csharp
using WuzApiClient.Events.Core.Interfaces;
using WuzApiClient.Events.Models;

public sealed class CustomEventErrorHandler : IEventErrorHandler
{
    private readonly ILogger<CustomEventErrorHandler> logger;

    public CustomEventErrorHandler(ILogger<CustomEventErrorHandler> logger)
    {
        this.logger = logger;
    }

    public Task HandleErrorAsync(
        WuzEvent evt,
        Exception exception,
        CancellationToken cancellationToken)
    {
        this.logger.LogError(
            exception,
            "Error processing {EventType} event",
            evt.GetType().Name
        );

        // Perform any custom error handling logic
        // (e.g., send alerts, record metrics, etc.)

        return Task.CompletedTask;
    }
}
```

Register the error handler:

```csharp
builder.Services.AddSingleton<IEventErrorHandler, CustomEventErrorHandler>();
```

### ErrorBehavior Configuration

Error behavior is configured at the subscription level, not per-error. Configure it in your event subscription options:

| Behavior | Description | Use Case |
|----------|-------------|----------|
| `AcknowledgeOnError` | Remove message from queue even on error | Fire-and-forget, skip failed messages |
| `RequeueOnError` | Return to queue for retry on error | Transient errors, automatic retry |
| `DeadLetterOnError` | Send to dead-letter queue on error | Unrecoverable errors, manual review needed |

## Common Error Scenarios

### Scenario 1: Gateway Not Running

```csharp
var result = await this.client.GetContactsAsync();

if (result.IsFailure)
{
    if (result.Error.Code == WuzApiErrorCode.NetworkError)
    {
        this.logger.LogError("WuzAPI gateway is not reachable. Check BaseAddress configuration.");
        // Notify operations team
        // Return user-friendly error
    }
}
```

### Scenario 2: Invalid Token

```csharp
var result = await this.client.SendTextMessageAsync(Phone.Parse(chatId), text);

if (result.IsFailure && result.Error.Code == WuzApiErrorCode.Unauthorized)
{
    this.logger.LogCritical("Authentication failed. UserToken may be invalid or expired.");
    // Refresh token from secure store
    // Or terminate application if token cannot be refreshed
}
```

### Scenario 3: Rate Limiting

```csharp
var result = await this.client.SendTextMessageAsync(Phone.Parse(chatId), text);

if (result.IsFailure && result.Error.Code == WuzApiErrorCode.RateLimitExceeded)
{
    this.logger.LogWarning("Rate limit exceeded. Waiting before retry...");
    await Task.Delay(TimeSpan.FromMinutes(1));
    // Retry the request
}
```

### Scenario 4: Invalid Phone Number

```csharp
var result = await this.client.SendTextMessageAsync(Phone.Parse(chatId), text);

if (result.IsFailure && result.Error.Code == WuzApiErrorCode.InvalidPhoneNumber)
{
    this.logger.LogWarning("Invalid phone number: {ChatId}", chatId);
    // Validate phone number format
    // Return validation error to user
}
```

## Composing Operations

Chain multiple operations, short-circuiting on first failure:

```csharp
public async Task<WuzResult> ProcessOrderAsync(Order order)
{
    // Step 1: Send confirmation message
    var confirmResult = await this.client.SendTextMessageAsync(
        order.CustomerPhone,
        $"Order #{order.Id} confirmed!"
    );

    if (confirmResult.IsFailure)
    {
        return WuzResult.Failure(
            WuzApiError.InvalidRequest($"Failed to send confirmation: {confirmResult.Error.Message}")
        );
    }

    // Step 2: Send invoice document
    var invoiceResult = await this.client.SendDocumentAsync(
        new SendDocumentRequest
        {
            Phone = order.CustomerPhone,
            Url = order.InvoiceUrl,
            FileName = $"Invoice-{order.Id}.pdf"
        }
    );

    if (invoiceResult.IsFailure)
    {
        return WuzResult.Failure(
            WuzApiError.InvalidRequest($"Failed to send invoice: {invoiceResult.Error.Message}")
        );
    }

    // Both succeeded
    return WuzResult.Success();
}
```

## Fallback Strategies

Provide fallback behavior on error:

```csharp
public async Task<ContactInfo> GetContactInfoAsync(string phoneNumber)
{
    var result = await this.client.GetUserInfoAsync(Phone.Parse(phoneNumber));

    if (result.IsSuccess)
    {
        return new ContactInfo
        {
            FullName = result.Value.PushName ?? result.Value.JID,
            Found = true
        };
    }

    // Fallback to default contact info
    this.logger.LogWarning(
        "Failed to get contact info for {Phone}, using default. Error: {Error}",
        phoneNumber,
        result.Error.Message
    );

    return new ContactInfo
    {
        FullName = "Unknown Contact",
        Found = false
    };
}
```

## Testing Error Handling

### Unit Testing

Mock failure results:

```csharp
[Fact]
public async Task SendMessage_LogsError_WhenSendFails()
{
    // Arrange
    var clientMock = new Mock<IWaClient>();
    clientMock
        .Setup(x => x.SendTextMessageAsync(It.IsAny<Phone>(), It.IsAny<string>(), null, default))
        .ReturnsAsync(WuzResult<SendMessageResponse>.Failure(
            WuzApiError.NetworkError("Network error")
        ));

    var loggerMock = new Mock<ILogger<MyService>>();
    var service = new MyService(clientMock.Object, loggerMock.Object);

    // Act
    await service.SendMessageAsync("123@c.us", "Hello");

    // Assert
    loggerMock.Verify(
        x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Network error")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()),
        Times.Once);
}
```

### Integration Testing

Test against real failures:

```csharp
[Fact]
public async Task SendMessage_ReturnsFailure_WhenGatewayUnreachable()
{
    // Arrange - configure with invalid gateway URL
    var services = new ServiceCollection();
    services.AddWuzApiClient(options =>
    {
        options.BaseAddress = "http://invalid-gateway:9999";
        options.UserToken = "test-token";
    });
    services.AddLogging();

    var provider = services.BuildServiceProvider();
    var client = provider.GetRequiredService<IWaClient>();

    // Act
    var result = await client.SendTextMessageAsync(Phone.Parse("123"), "Hello");

    // Assert
    Assert.True(result.IsFailure);
    Assert.Equal(WuzApiErrorCode.NetworkError, result.Error.Code);
}
```

## Best Practices

1. **Always check IsSuccess/IsFailure** before accessing Value
2. **Log errors with context** (include relevant parameters)
3. **Use Polly for retry logic** (don't reinvent the wheel)
4. **Return meaningful errors** to callers (don't swallow errors)
5. **Test error paths** as thoroughly as success paths
6. **Provide fallbacks** when possible (graceful degradation)
7. **Monitor error rates** (alert on spikes)

## Anti-Patterns to Avoid

❌ **DO NOT access Value without checking:**
```csharp
var result = await this.client.SendMessageAsync(...);
var messageId = result.Value.MessageId; // May throw if IsFailure!
```

❌ **DO NOT throw exceptions on failure:**
```csharp
if (result.IsFailure)
{
    throw new Exception(result.Error.Message); // Defeats the purpose!
}
```

❌ **DO NOT ignore failures:**
```csharp
var result = await this.client.SendMessageAsync(...);
// Continue without checking result - dangerous!
```

✅ **DO check before accessing:**
```csharp
if (result.IsSuccess)
{
    var messageId = result.Value.MessageId; // Safe
}
```

## Next Steps

- **Handle Events** → [Event Handling Guide](event-handling.md)
- **Configure Client** → [Configuration Reference](configuration.md)


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

## Next Steps

- **Handle Events** → [Event Handling Guide](event-handling.md)
- **Configure Client** → [Configuration Reference](configuration.md)


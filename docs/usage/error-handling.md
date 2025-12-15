# Error Handling

WuzAPI Client uses Result pattern instead of exceptions.

## Result Types

```csharp
public readonly struct WuzResult<T>
{
    public bool IsSuccess { get; }     // True if operation succeeded
    public bool IsFailure { get; }     // Opposite of IsSuccess
    public T Value { get; }            // Result value (only if IsSuccess)
    public WuzApiError Error { get; }  // Error details (only if IsFailure)
}
```

## Basic Usage

```csharp
var result = await client.SendTextMessageAsync(phone, text);

if (result.IsSuccess)
{
    var messageId = result.Value.MessageId;
}
else
{
    logger.LogError("Failed: {Error}", result.Error.Message);
}
```

## Error Categories

| Category | Example Codes | Description |
|----------|---------------|-------------|
| Network | `NetworkError`, `Timeout` | Connection failures |
| Auth | `Unauthorized` | Invalid token |
| Validation | `InvalidRequest`, `InvalidPhoneNumber` | Bad input |
| Gateway | `SessionNotReady`, `AlreadyLoggedIn` | WuzAPI state |
| Parsing | `DeserializationError` | JSON errors |

## Error Codes

| Code | Value | Description |
|------|-------|-------------|
| `Unknown` | 0 | Unknown error occurred |
| `NetworkError` | 1 | Network connection error |
| `Timeout` | 2 | Request timed out |
| `DeserializationError` | 3 | Response deserialization failed |
| `BadRequest` | 400 | Bad request - invalid parameters |
| `Unauthorized` | 401 | Unauthorized - invalid token |
| `Forbidden` | 403 | Forbidden - insufficient permissions |
| `NotFound` | 404 | Resource not found |
| `Conflict` | 409 | Conflict - resource already exists |
| `RateLimitExceeded` | 429 | Rate limit exceeded |
| `InternalServerError` | 500 | Internal server error |
| `SessionNotReady` | 1000 | Session not ready (not connected/logged in) |
| `AlreadyLoggedIn` | 1001 | Session already logged in |
| `InvalidPhoneNumber` | 1002 | Invalid phone number format |
| `InvalidFile` | 1003 | Invalid file format or size |
| `InvalidRequest` | 1004 | Invalid request parameters |
| `UnexpectedResponse` | 9999 | Unexpected response from API |

## Pattern Matching

```csharp
if (result.IsFailure)
{
    switch (result.Error.Code)
    {
        case WuzApiErrorCode.Unauthorized:
            logger.LogError("Invalid token");
            break;
        case WuzApiErrorCode.Timeout:
            logger.LogWarning("Request timed out, retrying...");
            break;
        default:
            logger.LogError("[{Code}]: {Message}", result.Error.Code, result.Error.Message);
            break;
    }
}
```

## API Response Example

```csharp
public async Task<ActionResult> SendMessage(SendMessageRequest request)
{
    var result = await client.SendTextMessageAsync(Phone.Create(request.Phone), request.Text);

    if (result.IsFailure)
        return BadRequest(new { error = result.Error.Message, code = result.Error.Code.ToString() });

    return Ok(new { messageId = result.Value.MessageId });
}
```

## Retries

The library does not include built-in retry logic. Use [Polly](https://www.pollydocs.org/) for resilience patterns.

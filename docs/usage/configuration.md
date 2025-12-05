# Configuration Reference

Reference for configuring WuzAPI Client factories and RabbitMQ event consumer.

## WuzApiOptions (HTTP Client Factories)

Options for configuring the HTTP client factories via `AddWuzApi()`.

**Note:** User tokens are not part of configuration. Tokens are passed dynamically when creating clients via `IWaClientFactory.CreateClient(userToken)` and `IWuzApiAdminClientFactory.CreateClient(adminToken)`.

### Properties

| Property | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| `BaseUrl` | `string` | Yes | `http://localhost:8080/` | Base URL of asternic/wuzapi gateway |
| `TimeoutSeconds` | `int` | No | `30` | HTTP request timeout in seconds |

### Basic Configuration

```csharp
using WuzApiClient.Configuration;

// Register factories (server settings only - tokens passed at runtime)
builder.Services.AddWuzApi(options =>
{
    options.BaseUrl = "http://localhost:8080/";
    options.TimeoutSeconds = 30;
});
```

### Configuration from appsettings.json

```json
{
  "WuzApi": {
    "BaseUrl": "http://localhost:8080/",
    "TimeoutSeconds": 45
  }
}
```

```csharp
builder.Services.AddWuzApi(builder.Configuration);
```

### With HttpClient Customization

Customize the underlying HttpClient with Polly resilience, logging, etc.:

```csharp
builder.Services.AddWuzApi(
    options => { options.BaseUrl = "http://localhost:8080/"; },
    httpClientBuilder =>
    {
        httpClientBuilder
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(300)))
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));
    });
```

### Environment-Specific Configuration

Use environment variables for the base URL:

```bash
# .env or system environment
WUZAPI_BASE_URL=http://localhost:8080/
```

```csharp
builder.Services.AddWuzApi(options =>
{
    options.BaseUrl = Environment.GetEnvironmentVariable("WUZAPI_BASE_URL")
        ?? "http://localhost:8080/";
});
```

### Token Management

Tokens are managed by your application, not the library. Common patterns:

```csharp
// From configuration (for single-account scenarios)
public class WhatsAppService
{
    private readonly IWaClientFactory clientFactory;
    private readonly string userToken;

    public WhatsAppService(IWaClientFactory clientFactory, IConfiguration config)
    {
        this.clientFactory = clientFactory;
        this.userToken = config["WuzApi:UserToken"]
            ?? throw new InvalidOperationException("Token not configured");
    }

    public async Task SendAsync(Phone phone, string message)
    {
        var client = this.clientFactory.CreateClient(this.userToken);
        await client.SendTextMessageAsync(phone, message);
    }
}

// From database/repository (for multi-account scenarios)
public class MultiAccountWhatsAppService
{
    private readonly IWaClientFactory clientFactory;
    private readonly IAccountRepository accountRepo;

    public MultiAccountWhatsAppService(IWaClientFactory clientFactory, IAccountRepository accountRepo)
    {
        this.clientFactory = clientFactory;
        this.accountRepo = accountRepo;
    }

    public async Task SendAsync(Guid accountId, Phone phone, string message)
    {
        var account = await this.accountRepo.GetAsync(accountId);
        var client = this.clientFactory.CreateClient(account.UserToken);
        await client.SendTextMessageAsync(phone, message);
    }
}
```


## WuzEventOptions (RabbitMQ Consumer)

Options for configuring the RabbitMQ event consumer via `AddWuzEvents()`.

### Properties

#### ConnectionString
- **Type:** `string`
- **Required:** Yes
- **Description:** RabbitMQ connection string in AMQP format (e.g., `amqp://user:pass@host:port/vhost`)

#### QueueName
- **Type:** `string`
- **Required:** No
- **Default:** `whatsapp_events`
- **Description:** Queue name to consume from

#### ConsumerTagPrefix
- **Type:** `string`
- **Required:** No
- **Default:** `wuzapi-consumer`
- **Description:** Prefix for consumer tags

#### PrefetchCount
- **Type:** `ushort`
- **Required:** No
- **Default:** `10`
- **Description:** RabbitMQ prefetch count

#### AutoAck
- **Type:** `bool`
- **Required:** No
- **Default:** `false`
- **Description:** Automatically acknowledge messages (not recommended for most use cases)

#### MaxReconnectAttempts
- **Type:** `int`
- **Required:** No
- **Default:** `10`
- **Description:** Maximum number of reconnection attempts

#### ReconnectDelay
- **Type:** `TimeSpan`
- **Required:** No
- **Default:** `3s`
- **Description:** Delay between reconnection attempts

#### MaxConcurrentMessages
- **Type:** `int`
- **Required:** No
- **Default:** `Environment.ProcessorCount`
- **Description:** Number of messages to process concurrently (1 = sequential)

### Basic Configuration

```csharp
using WuzApiClient.Events.Configuration;

builder.Services.AddWuzEvents(options =>
{
    options.ConnectionString = "amqp://guest:guest@localhost:5672/";
    options.QueueName = "wuzapi-events";
});
```

### Configuration from appsettings.json

```json
{
  "WuzEvents": {
    "ConnectionString": "amqp://wuzapi-consumer:secure-password@rabbitmq.example.com:5672/wuzapi",
    "QueueName": "wuzapi-events",
    "ConsumerTagPrefix": "wuzapi-consumer",
    "MaxConcurrentMessages": 1,
    "PrefetchCount": 10,
    "AutoAck": false,
    "MaxReconnectAttempts": 10,
    "ReconnectDelay": "00:00:03"
  }
}
```

```csharp
// Using fluent builder with assembly scanning (recommended)
builder.Services.AddWuzEvents(builder.Configuration, b => b
    .AddHandlersFromAssembly(ServiceLifetime.Scoped, typeof(Program).Assembly)
);

// Or bind configuration manually
builder.Services.AddWuzEvents(builder.Configuration);
```

### Environment-Specific Configuration

```bash
# Development
RABBITMQ_CONNECTION_STRING=amqp://guest:guest@localhost:5672/

# Production
RABBITMQ_CONNECTION_STRING=amqp://wuzapi-prod:<secure-password>@rabbitmq-prod.internal:5672/
```

```csharp
builder.Services.AddWuzEvents(options =>
{
    options.ConnectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING")
        ?? "amqp://guest:guest@localhost:5672/";
    options.QueueName = "wuzapi-events";
});
```

## Concurrency Configuration

### Sequential Processing

```csharp
builder.Services.AddWuzEvents(options =>
{
    options.MaxConcurrentMessages = 1; // Process one at a time
});
```

**Use when:**
- Message order must be preserved
- Events have dependencies
- Shared state requires synchronization

### Parallel Processing (Default)

```csharp
builder.Services.AddWuzEvents(options =>
{
    // Default is Environment.ProcessorCount
    options.MaxConcurrentMessages = 10; // Process up to 10 in parallel
    options.PrefetchCount = 20;         // Fetch 20 messages ahead
});
```

**Use when:**
- Events are independent
- High throughput required
- Processing is I/O bound

> **Warning:** `MaxConcurrentMessages > 1` breaks message ordering guarantees. Messages may be processed out of order.

## Connection Resilience

### Reconnection Configuration

```csharp
builder.Services.AddWuzEvents(options =>
{
    options.MaxReconnectAttempts = 10;  // Maximum reconnection attempts
    options.ReconnectDelay = TimeSpan.FromSeconds(3); // Wait between attempts
});
```


## Health Checks

To monitor RabbitMQ connection health, manually register the health check:

```csharp
builder.Services.AddWuzEvents(options => { ... });

// Manually add the WuzEvents health check
builder.Services.AddHealthChecks()
    .AddWuzEventsHealthCheck(); // Default name: "wuzevents-rabbitmq"

// Add health check endpoint
app.MapHealthChecks("/health");
```

Custom health check configuration:

```csharp
builder.Services.AddHealthChecks()
    .AddWuzEventsHealthCheck(
        name: "rabbitmq",
        tags: "messaging", "rabbitmq");
```

See [ASP.NET Core Health Checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks) for details.

## Example Configurations

### Development Environment

```json
{
  "WuzApi": {
    "BaseUrl": "http://localhost:8080/",
    "TimeoutSeconds": 30
  },
  "WuzEvents": {
    "ConnectionString": "amqp://guest:guest@localhost:5672/",
    "QueueName": "wuzapi-events-dev",
    "MaxConcurrentMessages": 1
  }
}
```

### Production Environment

```json
{
  "WuzApi": {
    "BaseUrl": "https://wuzapi-gateway.production.internal/",
    "TimeoutSeconds": 60
  },
  "WuzEvents": {
    "ConnectionString": null,  // From secure configuration provider
    "QueueName": "wuzapi-events",
    "ConsumerTagPrefix": "wuzapi-consumer",
    "MaxConcurrentMessages": 10,
    "PrefetchCount": 20,
    "AutoAck": false,
    "MaxReconnectAttempts": 10,
    "ReconnectDelay": "00:00:03"
  }
}
```

## Troubleshooting

### Configuration Not Loaded

**Problem:** Options have default/null values

**Solutions:**
1. Verify section name matches: `builder.Configuration.GetSection("WuzApi")`
2. Check file is copied to output: Set `appsettings.json` → `Copy if newer`
3. Inspect loaded configuration: `builder.Configuration.GetDebugView()`

### Connection Refused

**Problem:** Cannot connect to RabbitMQ

**Solutions:**
1. Verify `ConnectionString` is in correct AMQP format: `amqp://user:pass@host:port/vhost`
2. Check network connectivity: `telnet <hostname> 5672`
3. Verify credentials in connection string
4. Check firewall rules

### Token Authentication Failed

**Problem:** HTTP 401 Unauthorized from WuzAPI gateway

**Solutions:**
1. Verify token passed to `CreateClient()` matches gateway configuration
2. Check token is not empty or whitespace
3. Verify gateway is running and accessible
4. Ensure you're using the correct factory (`IWaClientFactory` for user operations, `IWuzApiAdminClientFactory` for admin operations)

## Next Steps

- **Handle Events** → [Event Handling Guide](event-handling.md)
- **Error Handling** → [Error Handling Guide](error-handling.md)


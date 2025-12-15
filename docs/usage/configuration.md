# Configuration Reference

## WuzApiOptions (HTTP Client)

```csharp
builder.Services.AddWuzApi(options =>
{
    options.BaseUrl = "http://localhost:8080/";  // Required - gateway URL
    options.TimeoutSeconds = 30;                  // Default: 30
});
```

### From appsettings.json

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

### Token Management

Tokens are passed at runtime, not stored in configuration:

```csharp
// Single-account
var client = this.clientFactory.CreateClient(config["WuzApi:UserToken"]);

// Multi-account
var account = await this.accountRepo.GetAsync(accountId);
var client = this.clientFactory.CreateClient(account.UserToken);
```

## WuzEventOptions (RabbitMQ Consumer)

```csharp
builder.Services.AddWuzEvents(options =>
{
    // Connection
    options.ConnectionString = "amqp://guest:guest@localhost:5672/";  // Required
    options.QueueName = "whatsapp_events";     // Default: "whatsapp_events"
    options.ConsumerTagPrefix = "wuzapi";      // Default: "wuzapi-consumer"

    // Performance
    options.PrefetchCount = 10;                // Default: 10
    options.MaxConcurrentMessages = 1;         // Default: Environment.ProcessorCount
    options.AutoAck = false;                   // Default: false (manual ack)

    // Resilience
    options.MaxReconnectAttempts = 10;         // Default: 10
    options.ReconnectDelay = TimeSpan.FromSeconds(3);  // Default: 3s
});
```

### From appsettings.json

```json
{
  "WuzEvents": {
    "ConnectionString": "amqp://user:pass@rabbitmq:5672/vhost",
    "QueueName": "wuzapi-events",
    "MaxConcurrentMessages": 10,
    "PrefetchCount": 20
  }
}
```

```csharp
builder.Services.AddWuzEvents(builder.Configuration, b => b
    .AddHandlersFromAssembly(ServiceLifetime.Scoped, typeof(Program).Assembly)
);
```

## Concurrency

```csharp
// Sequential (preserves order)
options.MaxConcurrentMessages = 1;

// Parallel (faster, no ordering guarantee)
options.MaxConcurrentMessages = 10;
options.PrefetchCount = 20;
```

**Use sequential when:** message order matters, events have dependencies.
**Use parallel when:** events are independent, high throughput needed.

## Health Checks

```csharp
builder.Services.AddWuzEvents(options => { ... });
builder.Services.AddHealthChecks()
    .AddWuzEventsHealthCheck();  // Default name: "wuzevents-rabbitmq"

app.MapHealthChecks("/health");
```

## Example Configurations

### Development

```json
{
  "WuzApi": { "BaseUrl": "http://localhost:8080/", "TimeoutSeconds": 30 },
  "WuzEvents": {
    "ConnectionString": "amqp://guest:guest@localhost:5672/",
    "QueueName": "wuzapi-events-dev",
    "MaxConcurrentMessages": 1
  }
}
```

### Production

```json
{
  "WuzApi": { "BaseUrl": "https://wuzapi.internal/", "TimeoutSeconds": 60 },
  "WuzEvents": {
    "ConnectionString": null,
    "QueueName": "wuzapi-events",
    "MaxConcurrentMessages": 10,
    "PrefetchCount": 20
  }
}
```

Use secure configuration providers for `ConnectionString` in production.

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Options have null/default values | Verify section name matches (`WuzApi`, `WuzEvents`) |
| Cannot connect to RabbitMQ | Check AMQP format: `amqp://user:pass@host:port/vhost` |
| HTTP 401 Unauthorized | Verify token passed to `CreateClient()` |

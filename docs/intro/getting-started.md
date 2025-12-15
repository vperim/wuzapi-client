# Getting Started

## Prerequisites

1. **.NET SDK** - .NET Core 2.0+ or .NET Framework 4.6.1+
2. **asternic/wuzapi Gateway** - Running instance ([setup guide](https://github.com/asternic/wuzapi))
3. **RabbitMQ** - Only if processing events
4. **User Token** - From your WuzAPI gateway

## Installation

```bash
dotnet add package WuzApiClient           # HTTP client (required)
dotnet add package WuzApiClient.RabbitMq  # Event handling (optional)
```

## Basic Setup

### 1. Register the Client Factory

```csharp
using WuzApiClient.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWuzApi(options =>
{
    options.BaseUrl = "http://localhost:8080/";
    options.TimeoutSeconds = 30;
});

var app = builder.Build();
```

### 2. Use the Factory

```csharp
using WuzApiClient.Core.Interfaces;
using WuzApiClient.Models.Common;

public sealed class WhatsAppService
{
    private readonly IWaClientFactory clientFactory;

    public WhatsAppService(IWaClientFactory clientFactory)
    {
        this.clientFactory = clientFactory;
    }

    public async Task SendWelcomeMessageAsync(string userToken, string phoneNumber)
    {
        var client = this.clientFactory.CreateClient(userToken);
        var phone = Phone.Create(phoneNumber);

        var result = await client.SendTextMessageAsync(phone, "Welcome!");

        if (result.IsSuccess)
            Console.WriteLine($"Sent: {result.Value.MessageId}");
        else
            Console.WriteLine($"Failed: {result.Error.Message}");
    }
}
```

## Sending Messages

```csharp
using WuzApiClient.Models.Requests.Chat;

// Text
var result = await client.SendTextMessageAsync(Phone.Create("5511999999999"), "Hello!");

// Image
var request = new SendImageRequest
{
    Phone = Phone.Create("5511999999999"),
    Image = Convert.ToBase64String(await File.ReadAllBytesAsync("image.jpg")),
    Caption = "Check this out!",
    MimeType = "image/jpeg"
};
await client.SendImageAsync(request);

// Document
var docRequest = new SendDocumentRequest
{
    Phone = Phone.Create("5511999999999"),
    Document = Convert.ToBase64String(await File.ReadAllBytesAsync("doc.pdf")),
    FileName = "invoice.pdf",
    MimeType = "application/pdf"
};
await client.SendDocumentAsync(docRequest);
```

## Event Handling (Optional)

### 1. Register Consumer

```csharp
using WuzApiClient.RabbitMq.Configuration;

builder.Services.AddWuzEvents(builder.Configuration, b => b
    .AddHandlersFromAssembly(ServiceLifetime.Scoped, typeof(Program).Assembly)
);
```

### 2. Create Handler

```csharp
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models.Events;

public sealed class MessageHandler : IEventHandler<MessageEventEnvelope>
{
    public Task HandleAsync(IWuzEventEnvelope<MessageEventEnvelope> envelope, CancellationToken ct)
    {
        var @event = envelope.Payload.Event;
        var text = @event.Message?.Conversation ?? @event.Message?.ExtendedTextMessage?.Text;
        Console.WriteLine($"From {envelope.Payload.Event.Info?.Sender}: {text}");
        return Task.CompletedTask;
    }
}
```

## Troubleshooting

| Problem | Solution |
|---------|----------|
| HTTP 401 Unauthorized | Verify `UserToken` matches gateway config |
| Connection refused | Check gateway is running, URL format includes protocol |
| Message not delivered | Use E.164 format (digits only), verify WhatsApp connection |

# Getting Started with WuzAPI Client

This guide walks you through installing WuzAPI Client and sending your first WhatsApp message.

## Prerequisites

Before starting, ensure you have:

1. **.NET SDK** – .NET Core 2.0+ or .NET Framework 4.6.1+ (for development)
2. **asternic/wuzapi Gateway** – Running instance with valid WhatsApp connection ([setup guide](https://github.com/asternic/wuzapi))
3. **RabbitMQ** – Running instance (only if processing events)
4. **User Token** – Authentication token from your WuzAPI gateway

For asternic/wuzapi setup, see the [official documentation](https://github.com/asternic/wuzapi/blob/main/API.md).

## Installation

Install the NuGet packages:

```bash
# For HTTP client (required)
dotnet add package WuzApiClient

# For event handling (optional)
dotnet add package WuzApiClient.RabbitMq
```

Or add to your `.csproj`:

```xml
<PackageReference Include="WuzApiClient" Version="1.0.0" />
<PackageReference Include="WuzApiClient.RabbitMq" Version="1.0.0" />
```

## Step 1: Register the HTTP Client

In your application's `Program.cs` or `Startup.cs`, register the WuzAPI client:

```csharp
using WuzApiClient.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Register WuzApiClient
builder.Services.AddWuzApiClient(options =>
{
    options.BaseUrl = "http://localhost:8080"; // Your WuzAPI gateway URL
    options.UserToken = "your-user-token";     // Gateway authentication token
});

var app = builder.Build();
```

For production environments, store the token securely using [configuration providers](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/).

## Step 2: Inject and Use the Client

Inject `IWuzApiClient` into your services or controllers:

```csharp
using WuzApiClient.Core.Interfaces;
using WuzApiClient.Models.Common;

public sealed class WhatsAppService
{
    private readonly IWuzApiClient wuzClient;
    private readonly ILogger<WhatsAppService> logger;

    public WhatsAppService(IWuzApiClient wuzClient, ILogger<WhatsAppService> logger)
    {
        this.wuzClient = wuzClient;
        this.logger = logger;
    }

    public async Task SendWelcomeMessageAsync(string phoneNumber)
    {
        // Create Phone object from E.164 format phone number (digits only)
        var phone = Phone.Create(phoneNumber);

        var result = await this.wuzClient.SendTextMessageAsync(
            phone,
            "Welcome to our service!"
        );

        if (result.IsSuccess)
        {
            this.logger.LogInformation(
                "Message sent successfully. MessageId: {MessageId}",
                result.Value.MessageId
            );
        }
        else
        {
            this.logger.LogError(
                "Failed to send message: {Error}",
                result.ErrorMessage
            );
        }
    }
}
```

## Step 3: Handle the Result

WuzAPI Client uses the Result pattern instead of exceptions. Always check `IsSuccess` before accessing `Value`:

```csharp
var phone = Phone.Create("5511999999999");
var result = await wuzClient.SendTextMessageAsync(phone, "Hello, World!");

if (result.IsSuccess)
{
    // Success path
    var messageId = result.Value.MessageId;
    var timestamp = result.Value.Timestamp;
    // Use the response...
}
else
{
    // Error path
    var errorMessage = result.ErrorMessage;
    // Handle the error...
}
```

See [Error Handling Guide](../usage/error-handling.md) for advanced error handling patterns.

## Step 4: Send Different Message Types

Note: These examples require the following using statements:
```csharp
using WuzApiClient.Models.Common;
using WuzApiClient.Models.Requests.Chat;
```

### Text Message

```csharp
var phone = Phone.Create("5511999999999");
var result = await wuzClient.SendTextMessageAsync(
    phone,
    "Hello, World!"
);
```

### Image Message

```csharp
var phone = Phone.Create("5511999999999");
var imageBytes = await File.ReadAllBytesAsync("path/to/image.jpg");
var imageBase64 = Convert.ToBase64String(imageBytes);

var request = new SendImageRequest
{
    Phone = phone,
    Image = imageBase64,
    Caption = "Check out this image!",
    MimeType = "image/jpeg"
};

var result = await wuzClient.SendImageAsync(request);
```

### Document Message

```csharp
var phone = Phone.Create("5511999999999");
var documentBytes = await File.ReadAllBytesAsync("path/to/document.pdf");
var documentBase64 = Convert.ToBase64String(documentBytes);

var request = new SendDocumentRequest
{
    Phone = phone,
    Document = documentBase64,
    FileName = "invoice.pdf",
    MimeType = "application/pdf"
};

var result = await wuzClient.SendDocumentAsync(request);
```

For complete API reference, see [HTTP Client Reference](../api/http-client-reference.md).

## Step 5: Query Contacts and Groups (Optional)

### Get All Contacts

```csharp
var result = await wuzClient.GetContactsAsync();

if (result.IsSuccess)
{
    foreach (var contact in result.Value.Contacts)
    {
        Console.WriteLine($"{contact.Name}: {contact.Id}");
    }
}
```

### Get All Groups

```csharp
var result = await wuzClient.GetGroupsAsync();

if (result.IsSuccess)
{
    foreach (var group in result.Value.Groups)
    {
        Console.WriteLine($"{group.Name}: {group.Id}");
    }
}
```

## Step 6: Process Incoming Events (Optional)

To handle incoming WhatsApp events (messages, status updates, etc.), set up the RabbitMQ event consumer.

### Register Event Consumer

In `Program.cs`:

```csharp
using WuzApiClient.Events.Configuration;

builder.Services.AddWuzEvents(options =>
{
    options.ConnectionString = "amqp://guest:guest@localhost:5672/";
    options.QueueName = "wuzapi-events";
});
```

For RabbitMQ configuration details, see the [RabbitMQ .NET Client Guide](https://www.rabbitmq.com/client-libraries/dotnet-api-guide).

### Create an Event Handler

Implement `IEventHandler<TEvent>` for the events you want to process:

```csharp
using WuzApiClient.Events.Core.Interfaces;
using WuzApiClient.Events.Models.Events;

public sealed class MessageHandler : IEventHandler<MessageEvent>
{
    private readonly ILogger<MessageHandler> logger;

    public MessageHandler(ILogger<MessageHandler> logger)
    {
        this.logger = logger;
    }

    public Task HandleAsync(MessageEvent @event, CancellationToken cancellationToken)
    {
        // Get sender information
        var sender = @event.Info?.Sender ?? "Unknown";

        // Get message text (check both Conversation and ExtendedTextMessage)
        var text = @event.Message?.Conversation
                   ?? @event.Message?.ExtendedTextMessage?.Text
                   ?? "(no text)";

        this.logger.LogInformation(
            "Received message from {Sender}: {Text}",
            sender,
            text
        );

        // Process the message...
        return Task.CompletedTask;
    }
}
```

### Register the Handler

```csharp
builder.Services.AddScoped<IEventHandler<MessageEvent>, MessageHandler>();
```

For event handling documentation, see [Event Handling Guide](../usage/event-handling.md).

## Common Issues

### Authentication Error

**Problem:** HTTP 401 Unauthorized

**Solution:** Verify your `UserToken` matches the token configured in your asternic/wuzapi gateway.

### Connection Refused

**Problem:** Cannot connect to WuzAPI gateway

**Solution:**
- Ensure asternic/wuzapi is running on the configured `BaseUrl`
- Check network connectivity and firewall rules
- Verify the URL format (include protocol: `http://` or `https://`)

### Message Not Delivered

**Problem:** Message sent successfully but not received

**Solution:**
- Verify the phone number is in E.164 format (digits only, e.g., "5511999999999")
- Use the `Phone.Create()` method to validate phone numbers
- Note: The Phone type automatically converts to WhatsApp JID format (`phoneNumber@s.whatsapp.net` for individuals, `groupId@g.us` for groups)
- Check that the WhatsApp account in asternic/wuzapi is connected and active
- Review asternic/wuzapi gateway logs for errors

## Next Steps

- **Handle Events** → [Event Handling Guide](../usage/event-handling.md)
- **Configure Options** → [Configuration Reference](../usage/configuration.md)
- **Full API Reference** → [HTTP Client Reference](../api/http-client-reference.md)


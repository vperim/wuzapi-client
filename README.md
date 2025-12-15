# WuzAPI Client

[![CI & Publish](https://github.com/vperim/wuzapi-client/actions/workflows/ci.yml/badge.svg)](https://github.com/vperim/wuzapi-client/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/WuzApiClient.svg)](https://www.nuget.org/packages/WuzApiClient)
[![License](https://img.shields.io/github/license/vperim/wuzapi-client)](LICENSE)

> A .NET client library for WhatsApp operations via the WuzAPI gateway, featuring strongly-typed event handling.

## 1. What is this?

WuzAPI Client is a .NET Standard 2.0 library that provides a type-safe interface to the [asternic/wuzapi](https://github.com/asternic/wuzapi) WhatsApp gateway. It enables .NET applications to send WhatsApp messages, manage contacts, groups, and process incoming WhatsApp events through RabbitMQ with the Result pattern for error handling.

The library uses `Microsoft.Extensions.*` for dependency injection, configuration, and hosted services.

## 2. Quick Links

- **Getting Started:** [docs/intro/getting-started.md](docs/intro/getting-started.md)
- **Event Handling Guide:** [docs/usage/event-handling.md](docs/usage/event-handling.md)
- **Configuration Reference:** [docs/usage/configuration.md](docs/usage/configuration.md)
- **API Reference:** [docs/api/http-client-reference.md](docs/api/http-client-reference.md)
- **Integration Testing:** [scripts/README.md](scripts/README.md)
- **Event Dashboard:** [tools/WuzApiClient.EventDashboard/README.md](tools/WuzApiClient.EventDashboard/README.md) - Real-time event visualization tool

## 3. Getting Started (Quick Start)

### Prerequisites

- .NET Standard 2.0 compatible runtime (.NET Core 2.0+, .NET Framework 4.6.1+, .NET 5+)
- Running [asternic/wuzapi](https://github.com/asternic/wuzapi) gateway instance
- RabbitMQ instance (for event handling)

### Installation

```bash
dotnet add package WuzApiClient
dotnet add package WuzApiClient.RabbitMq  # For event handling
```

### Basic Usage

```csharp
using WuzApiClient.Core.Interfaces;
using WuzApiClient.Configuration;
using WuzApiClient.Models.Common;

// Register factories in DI container (server settings only)
services.AddWuzApi(options =>
{
    options.BaseUrl = "http://your-wuzapi-gateway:8080/";
});

// Use factories to create clients with dynamic tokens
public sealed class WhatsAppService
{
    private readonly IWaClientFactory clientFactory;

    public WhatsAppService(IWaClientFactory clientFactory)
    {
        this.clientFactory = clientFactory;
    }

    public async Task SendMessageAsync(string userToken)
    {
        // Create client with user-specific token
        var client = this.clientFactory.CreateClient(userToken);

        var result = await client.SendTextMessageAsync(
            Phone.Create("5511999999999"),
            "Hello from WuzAPI!"
        );

        if (result.IsSuccess)
        {
            Console.WriteLine($"Message sent: {result.Value.MessageId}");
        }
        else
        {
            Console.WriteLine($"Error: {result.Error.Message}");
        }
    }
}
```

See [Getting Started](docs/intro/getting-started.md) for a full guide.

## 4. High-Level Architecture

The library consists of two main components:

1. **WuzApiClient** – HTTP client wrapping the asternic/wuzapi REST API for outbound WhatsApp operations (sending messages, managing contacts, etc.)
2. **WuzApiClient.RabbitMq** – Event consumer that processes incoming WhatsApp events from a RabbitMQ queue

For more details, see the [Overview](docs/intro/overview.md).

## 5. Documentation

Full documentation is available in the [docs/](docs/) folder. See [docs_map.md](docs_map.md) for the full index.
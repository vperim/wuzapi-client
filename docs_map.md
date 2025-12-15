# Documentation Map

This file is the index of all key documentation for WuzAPI Client.
It is optimized for both humans and AI assistants.

## 1. Documentation Index

### 1.1 Intro & Getting Started

| Doc | Description | Audience |
|-----|-------------|----------|
| [README.md](README.md) | Project overview, quick start, key features | All |
| [docs/intro/overview.md](docs/intro/overview.md) | What WuzAPI Client is, core components, design philosophy | End-user, Integrator |
| [docs/intro/getting-started.md](docs/intro/getting-started.md) | Installation and first message tutorial | End-user, Integrator |

### 1.2 Usage Guides

| Doc                                                                  | Description                                          | Audience   |
| -------------------------------------------------------------------- | ---------------------------------------------------- | ---------- |
| [docs/usage/event-handling.md](docs/usage/event-handling.md)         | How to process WhatsApp events with IEventHandler<T> | Integrator |
| [docs/usage/configuration.md](docs/usage/configuration.md)           | Reference for WuzApiOptions and WuzEventOptions      | Integrator |
| [docs/usage/error-handling.md](docs/usage/error-handling.md)         | Using the Result pattern for error handling          | Integrator |
| [docs/usage/extension-patterns.md](docs/usage/extension-patterns.md) | Extension points and how to customize behavior       | Integrator |

### 1.3 API Reference

| Doc | Description | Audience |
|-----|-------------|----------|
| [docs/api/http-client-reference.md](docs/api/http-client-reference.md) | IWuzApiClient method reference (all HTTP operations) | Integrator |
| [docs/api/event-types-reference.md](docs/api/event-types-reference.md) | Complete catalog of all 44 event types with examples | Integrator |

### 1.4 Testing & Scripts

| Doc | Description | Audience |
|-----|-------------|----------|
| [scripts/README.md](scripts/README.md) | Integration test scripts (Setup-TestSession, Validate-WuzApiVersion) | Internal, Operator |
| [tools/WuzApiClient.EventDashboard/README.md](tools/WuzApiClient.EventDashboard/README.md) | Blazor Server dashboard for real-time event visualization | Internal |

## 2. External References

### Must-Read External Documentation

- [asternic/wuzapi API Documentation](https://github.com/asternic/wuzapi/blob/main/API.md) – Gateway API reference
- [RabbitMQ .NET Client Guide](https://www.rabbitmq.com/client-libraries/dotnet-api-guide) – AMQP messaging
- [ASP.NET Core Dependency Injection](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection) – DI patterns
- [IOptions Pattern](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options) – Configuration binding
- [Polly Documentation](https://www.pollydocs.org/) – Resilience library

### Recommended Reading

- [HttpClient Factory](https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory) – HTTP client best practices
- [IHostedService](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services) – Background services
- [ASP.NET Core Configuration](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/) – Configuration providers
- [RabbitMQ Dead Letter Exchanges](https://www.rabbitmq.com/docs/dlx) – Error handling

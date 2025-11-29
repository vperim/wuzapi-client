# WuzAPI Event Dashboard

A real-time **Blazor Server** dashboard for visualizing and debugging WhatsApp events consumed from RabbitMQ via the `WuzApiClient.RabbitMq` library.

## Features

- **Real-time Event Visualization** - Events stream live from RabbitMQ with < 500ms latency
- **All 44 Event Types Supported** - Message, Receipt, Presence, Connection, Call, Group, System, and more
- **Connection Status Indicator** - Real-time RabbitMQ and WhatsApp connection state
- **Raw JSON View** - Expandable section per event for debugging event payload structure
- **Category-based Color Coding** - Visual distinction between event types (Messages, Receipts, Calls, etc.)
- **Thread-Safe Event Buffering** - Configurable in-memory buffer with automatic overflow handling
- **Docker Support** - Pre-configured Dockerfile and docker-compose for instant setup

## Prerequisites

- **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Docker Desktop** - For RabbitMQ and wuzapi containers
- **WhatsApp Account** - For QR code authentication

## Quick Start - Guided Setup (Recommended)

The easiest way to get started is using the setup script:

```powershell
cd tools/WuzApiClient.EventDashboard
.\Setup-EventDashboard.ps1
```

This script will:
1. Start all services via docker-compose (RabbitMQ, wuzapi, dashboard)
2. Wait for services to become healthy
3. Create the dashboard user
4. Guide you through WhatsApp QR code authentication
5. Open the dashboard in your browser

**Cleanup:**
```powershell
.\Setup-EventDashboard.ps1 -Cleanup
```

**Options:**
| Parameter | Default | Description |
|-----------|---------|-------------|
| `-SkipBrowserOpen` | `false` | Don't open browser automatically |
| `-Cleanup` | `false` | Stop and remove all containers/volumes |
| `-WuzApiPort` | `8080` | wuzapi port |
| `-DashboardPort` | `5000` | Dashboard port |

## Quick Start - Local Development

### 1. Start RabbitMQ

```powershell
docker run -d `
  --name rabbitmq `
  -p 5672:5672 `
  -p 15672:15672 `
  rabbitmq:4-management
```

RabbitMQ Management UI: http://localhost:15672 (default: guest/guest)

### 2. Start wuzapi

```powershell
docker run -d `
  --name wuzapi `
  -p 8080:8080 `
  -v wuzapi-session:/app/dbdata `
  -e WUZAPI_ADMIN_TOKEN=admin123 `
  -e RABBITMQ_URL=amqp://guest:guest@host.docker.internal:5672/ `
  asternic/wuzapi:latest
```

### 2.1. Authenticate WhatsApp Session

**Initiate connection** (triggers QR code):
```powershell
# PowerShell
$headers = @{
    "Token" = "user123"
    "Content-Type" = "application/json"
}
$body = '{"Subscribe":["Message"],"Immediate":true}'
Invoke-RestMethod -Uri "http://localhost:8080/session/connect" -Method Post -Headers $headers -Body $body
```

**View QR code** in logs:
```powershell
docker logs wuzapi -f
```

**Scan QR code** from logs with your phone (Settings → Linked Devices → Link a Device).

### 3. Run the Dashboard

```powershell
cd tools/WuzApiClient.EventDashboard
dotnet run
```

Open your browser: http://localhost:5000

## Quick Start - Docker Compose

### One-Command Setup

```powershell
cd tools/WuzApiClient.EventDashboard
docker compose up -d

# Wait for services to become healthy (~10-15 seconds)
docker compose ps
```

### Authenticate WhatsApp Session

**IMPORTANT:** wuzapi requires WhatsApp QR code authentication before events will flow.

1. **Initiate Connection** (triggers QR code generation):
   ```powershell
   # PowerShell
   $headers = @{
       "Token" = "user123"
       "Content-Type" = "application/json"
   }
   $body = '{"Subscribe":["Message"],"Immediate":true}'
   Invoke-RestMethod -Uri "http://localhost:8080/session/connect" -Method Post -Headers $headers -Body $body

   # Or using curl (Git Bash/WSL)
   curl -X POST http://localhost:8080/session/connect \
     -H "Token: user123" \
     -H "Content-Type: application/json" \
     -d '{"Subscribe":["Message"],"Immediate":true}'
   ```

2. **View QR Code in Logs**:
   ```powershell
   # Find container name
   docker compose ps

   # View logs (QR code appears as ASCII art)
   docker logs event-dashboard-wuzapi-1 -f
   ```

3. **Scan QR Code**:
   - Open WhatsApp on your phone
   - Go to **Settings → Linked Devices**
   - Tap **Link a Device**
   - Scan the QR code from the logs

4. **Verify Authentication**:
   ```powershell
   Invoke-RestMethod -Uri "http://localhost:8080/session/status" -Headers @{"Token"="user123"}
   ```
   You should see `Connected: True` and `LoggedIn: True`.

5. **Access Dashboard**:
   - Open browser: http://localhost:5000
   - Connection status bar should show "Connected to RabbitMQ"
   - Send a test WhatsApp message to see events streaming!

**Alternative - Use Automated Setup Scripts:**

For a guided setup with automatic health checks and detailed instructions, see:
- [scripts/README.md](../../scripts/README.md) - Comprehensive setup guide
- `scripts/Setup-TestSession.ps1` - PowerShell script for automated container setup

### Cleanup

```powershell
docker compose down -v
```

## Configuration

Environment variables override `appsettings.json` settings:

| Variable | Description | Default |
|----------|-------------|---------|
| `WuzEvents__ConnectionString` | RabbitMQ AMQP connection string | `amqp://guest:guest@localhost:5672/` |
| `WuzEvents__QueueName` | RabbitMQ queue name | `whatsapp_events` |
| `WuzEvents__PrefetchCount` | Number of events to prefetch | `10` |
| `Dashboard__MaxEventBufferSize` | Max recent events to keep in memory | `100` |
| `ASPNETCORE_URLS` | ASP.NET Core listen URLs | `http://+:5000` |

### Example - Set Custom Buffer Size

```powershell
$env:Dashboard__MaxEventBufferSize = "250"
dotnet run
```

## Architecture

```
RabbitMQ → WuzApiClient.RabbitMq → DashboardEventHandler → EventStreamService
                                                                      ↓
                                        Blazor Server Components (SignalR WebSocket)
                                                      ↓
                                              Browser UI (Real-time updates)
```

**Key Components:**

- **EventStreamService** - Singleton service that buffers recent events and broadcasts state changes
- **DashboardEventHandler** - Catch-all event handler that forwards all 44 event types to EventStreamService
- **Blazor Server Components** - Real-time UI using SignalR for event updates
- **Event Categories** - 7 visual categories: Message, Receipt, Presence, Connection, Call, Group, System

**Thread Safety:**

The dashboard uses locking (`lock(gate)`) throughout `EventStreamService` to safely handle concurrent access from RabbitMQ consumer threads and Blazor circuit threads. No special synchronization is needed in Blazor components - state updates are marshaled via `InvokeAsync(StateHasChanged)`.

## Security Warning

**The Event Dashboard is NOT secure for production use.**

This tool is designed for:
- Local development
- Private VPN-restricted environments
- Closed internal networks

**Never expose to the public internet.** It has no authentication, authorization, or encryption beyond RabbitMQ's basic auth.

## Troubleshooting

### RabbitMQ Not Connecting

```
Error: Cannot connect to amqp://...
```

**Solutions:**
1. Verify RabbitMQ container is running:
   ```powershell
   docker ps | grep rabbitmq
   ```

2. Check connection string format (no trailing slash for path):
   ```
   amqp://guest:guest@localhost:5672/
   ```

3. Test connection with RabbitMQ Management UI:
   ```
   http://localhost:15672
   ```

### wuzapi Not Authenticated

Events won't appear until wuzapi has an authenticated WhatsApp session.

**Quick Fix:**

1. Initiate connection to trigger QR code generation:
   ```powershell
   Invoke-RestMethod -Uri "http://localhost:8080/session/connect" `
     -Method Post `
     -Headers @{"Token"="user123"; "Content-Type"="application/json"} `
     -Body '{"Subscribe":["Message"],"Immediate":true}'
   ```

2. View QR code in logs:
   ```powershell
   docker logs wuzapi -f
   ```

3. Scan with your phone (Settings → Linked Devices → Link a Device)

**Alternative - Use Automated Setup:**

For a fully guided setup process, use the setup script:
```powershell
.\Setup-EventDashboard.ps1
```

This script automates container creation, health checks, and provides step-by-step QR code instructions.

### Dashboard Shows "Disconnected"

Check wuzapi logs:
```powershell
docker logs wuzapi
```

Look for:
- "Connected" message (successful pairing)
- "Temporary Ban" (account temporarily locked)
- "Client Outdated" (wuzapi version incompatibility)

### No Events Appearing

1. Ensure RabbitMQ is connected (connection status bar shows "Connected")
2. Send test messages to the authenticated WhatsApp number
3. Check wuzapi logs for message processing errors
4. Verify queue name matches in appsettings.json

## Development

### Project Structure

```
WuzApiClient.EventDashboard/
├── Program.cs                      # Startup and DI configuration
├── appsettings.json               # Configuration
├── Setup-EventDashboard.ps1       # Guided setup script
├── Dockerfile                      # Multi-stage Docker build
├── docker-compose.yml             # Local development orchestration
├── Models/
│   ├── DashboardOptions.cs        # Configuration class
│   ├── EventCategory.cs           # Event category enum
│   ├── EventCategoryMapper.cs     # Category mapping logic
│   └── EventEntry.cs              # Event wrapper with metadata
├── Services/
│   ├── IEventStreamService.cs     # Event stream interface
│   ├── EventStreamService.cs      # Threadsafe event buffering
│   └── DashboardEventHandler.cs   # WuzApiClient.RabbitMq integration
├── Components/
│   ├── App.razor                  # Root component
│   ├── Routes.razor               # Router
│   ├── Layout/
│   │   ├── MainLayout.razor       # Main layout wrapper
│   │   ├── MainLayout.razor.css   # Layout styles
│   │   └── NavMenu.razor          # Navigation menu
│   └── Pages/
│       ├── Home.razor             # Main event feed page
│       ├── Home.razor.css         # Event feed styles
│       └── Error.razor            # Error page
└── wwwroot/
    ├── css/app.css               # Global styles
    └── favicon.ico
```

### Building for Production (Docker)

```powershell
docker build `
  -f tools/WuzApiClient.EventDashboard/Dockerfile `
  -t wuzapi-event-dashboard:latest `
  .
```

The Dockerfile uses a multi-stage build:
1. **Build stage** - Compiles .NET code with SDK
2. **Runtime stage** - Minimal ASP.NET Core image with non-root user

### Running Tests

```powershell
dotnet test
```

## Event Types

The dashboard supports all 44 event types from WhatsApp/wuzapi:

**Messages (3):** Message, UndecryptableMessage, FBMessage

**Receipts (1):** Receipt

**Presence (2):** Presence, ChatPresence

**Connection (11):** Connected, Disconnected, Qr, QrTimeout, PairSuccess, PairError, LoggedOut, ConnectFailure, ClientOutdated, TemporaryBan, StreamError, StreamReplaced, KeepAliveTimeout, KeepAliveRestored

**Calls (5):** CallOffer, CallAccept, CallTerminate, CallOfferNotice, CallRelayLatency

**Groups (2):** GroupInfo, JoinedGroup

**System (17):** Picture, HistorySync, AppState, AppStateSyncComplete, OfflineSyncCompleted, OfflineSyncPreview, PrivacySettings, PushNameSetting, BlocklistChange, Blocklist, IdentityChange, NewsletterJoin, NewsletterLeave, NewsletterMuteChange, NewsletterLiveUpdate, MediaRetry, UserAbout

For detailed information on each event type, see the [Event Dashboard Proposal](../../docs/dev-guide/event-dashboard-proposal.md#8-supported-event-types).

## Performance

- **Event Latency:** < 500ms from RabbitMQ to UI
- **Memory Usage:** Configurable buffer (default 100 events ~5-10 MB)
- **Concurrency:** Thread-safe for unlimited concurrent messages
- **Scalability:** In-memory only, suitable for dev/testing (not production)

## References

- **Event Dashboard Proposal:** [../../docs/dev-guide/event-dashboard-proposal.md](../../docs/dev-guide/event-dashboard-proposal.md)
- **Blazor Documentation:** [ASP.NET Core Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- **WuzApiClient.RabbitMq:** [../../src/WuzApiClient.RabbitMq/README.md](../../src/WuzApiClient.RabbitMq/README.md)
- **Docker:** [Building .NET Docker Images](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/building-net-docker-images)

## License

Same as parent repository.

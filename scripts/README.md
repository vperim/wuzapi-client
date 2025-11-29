# WuzApi Integration Test Scripts

This directory contains PowerShell scripts for setting up and running wuzapi integration tests.

## Overview

The integration test workflow consists of two main steps:

1. **Setup**: Create a Docker container with a persisted WhatsApp session
2. **Validation**: Run tiered integration tests against the container

```
┌─────────────────────────────────────────────────────────────┐
│  Integration Test Workflow                                  │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  Step 1: Setup-TestSession.ps1                              │
│  ┌────────────────────────────────────────┐                 │
│  │ 1. Create Docker volume                │                 │
│  │ 2. Start wuzapi container              │                 │
│  │ 3. Wait for container health           │                 │
│  │ 4. Check session status                │                 │
│  │ 5. Display QR code (if not auth'd)     │                 │
│  │ 6. Wait for WhatsApp authentication    │                 │
│  └────────────────────────────────────────┘                 │
│                      │                                       │
│                      ▼                                       │
│  Step 2: Validate-WuzApiVersion.ps1                         │
│  ┌────────────────────────────────────────┐                 │
│  │ 1. Verify container is healthy         │                 │
│  │ 2. Verify session is authenticated     │                 │
│  │ 3. Build integration tests             │                 │
│  │ 4. Run Tier 0 (ReadOnly) tests         │                 │
│  │ 5. Run Tier 1 (Messaging) tests        │                 │
│  │ 6. Run Tier 2 (StateModifying) tests   │                 │
│  │ 7. Run Tier 3 (Destructive) tests      │                 │
│  │ 8. Generate TRX reports                │                 │
│  └────────────────────────────────────────┘                 │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

## Prerequisites

- **Docker Desktop**: Installed and running
- **.NET SDK**: .NET 9.0 or later
- **PowerShell**: 5.1 (Windows PowerShell) or 7+ (PowerShell Core)
- **Internet Connection**: To pull wuzapi Docker image
- **WhatsApp Account**: For QR code authentication
- **Test Configuration**: Personal phone number for Tier 1 (Messaging) tests

## Quick Start

### Configuration (First Time Only)

Before running tests, configure your personal test data:

```powershell
# Navigate to integration tests directory
cd tests\WuzApiClient.IntegrationTests

# Copy the template to create your local config
copy appsettings.Local.json.template appsettings.Local.json

# Edit appsettings.Local.json and set:
#   - TestPhoneNumber: Your test recipient's phone (international format, e.g., 5511987654321)
```

**Important Notes:**
- `appsettings.Local.json` is gitignored and will NOT be committed
- This prevents accidentally exposing your personal phone number in the repository
- **Group tests**: TestGroupId is no longer required - tests will auto-create a test group using your TestPhoneNumber
- The template file `appsettings.Local.json.template` shows the required format

### First-Time Setup

```powershell
# Navigate to scripts directory
cd scripts

# Run setup (creates container and authenticates WhatsApp session)
.\Setup-TestSession.ps1

# Follow on-screen instructions to scan QR code
```

### Running Tests

```powershell
# Run all tiers (complete validation)
.\Validate-WuzApiVersion.ps1

# Quick validation (Tier 0 only - read-only tests)
.\Validate-WuzApiVersion.ps1 -TiersToRun "0"

# Safe tests only (skip destructive Tier 3)
.\Validate-WuzApiVersion.ps1 -TiersToRun "0,1,2"
```

**Tier 1 Warning:**
When running Tier 1 (Messaging) tests, the script will:
1. Display the configured phone number
2. Warn that real WhatsApp messages will be sent
3. Prompt you to press any key to continue (or Ctrl+C to abort)

## Scripts Reference

### Setup-TestSession.ps1

Creates and configures a wuzapi test environment.

**Purpose:**
- Creates Docker volume for session persistence
- Starts wuzapi container
- Guides through WhatsApp QR code authentication
- Validates session is ready for testing

**Usage:**

```powershell
# Basic usage (uses defaults)
.\Setup-TestSession.ps1

# Custom wuzapi version
.\Setup-TestSession.ps1 -WuzApiVersion "v3.0.0"

# Custom port
.\Setup-TestSession.ps1 -HostPort 9090

# Custom tokens
.\Setup-TestSession.ps1 -AdminToken "my-admin-token" -UserToken "my-user-token"
```

**Parameters:**

| Parameter | Default | Description |
|-----------|---------|-------------|
| `WuzApiVersion` | `latest` | Docker image tag for wuzapi |
| `AdminToken` | `admin123` | Admin API token |
| `UserToken` | `user123` | User API token |
| `VolumeName` | `wuzapi-test-session` | Docker volume name |
| `ContainerName` | `wuzapi-test` | Docker container name |
| `HostPort` | `8080` | Host port to expose wuzapi |

**Exit Codes:**
- `0` - Success (session authenticated)
- `1` - Failure (Docker not running, health check failed, etc.)

**Example Output:**

```
═══════════════════════════════════════════════════════════
  WuzApi Test Session Setup
═══════════════════════════════════════════════════════════

[INFO] Checking if Docker is running...
[SUCCESS] Docker is running
[INFO] Creating Docker volume: wuzapi-test-session
[SUCCESS] Created volume 'wuzapi-test-session'
[INFO] Starting wuzapi container (version: latest)...
[SUCCESS] Container started: abc123def456

[INFO] Waiting for container to become healthy...
[SUCCESS] Container is healthy and responding

[INFO] Checking session authentication status...

═══════════════════════════════════════════════════════════
  WhatsApp QR Code Authentication Required
═══════════════════════════════════════════════════════════

The session is not authenticated. Please scan the QR code:

  1. View the QR code in container logs: docker logs wuzapi-test -f
  2. Open WhatsApp on your phone
  3. Go to Settings > Linked Devices
  4. Tap 'Link a Device'
  5. Scan the QR code from the container logs

═══════════════════════════════════════════════════════════

[INFO] Waiting for WhatsApp authentication...
[SUCCESS] Session authenticated successfully!

═══════════════════════════════════════════════════════════
  Setup Complete!
═══════════════════════════════════════════════════════════
```

### Validate-WuzApiVersion.ps1

Runs integration tests against a wuzapi container.

**Purpose:**
- Verifies container health and session authentication
- Builds integration test project
- Executes tests in tier order
- Generates TRX test result files
- Reports pass/fail status

**Usage:**

```powershell
# Run all tiers
.\Validate-WuzApiVersion.ps1

# Test specific version (for reporting)
.\Validate-WuzApiVersion.ps1 -Version "v3.0.0"

# Quick validation (Tier 0 only)
.\Validate-WuzApiVersion.ps1 -TiersToRun "0"

# Skip Tier 3 (destructive tests)
.\Validate-WuzApiVersion.ps1 -TiersToRun "0,1,2"

# Custom port and skip build
.\Validate-WuzApiVersion.ps1 -HostPort 9090 -SkipBuild
```

**Parameters:**

| Parameter | Default | Description |
|-----------|---------|-------------|
| `Version` | `latest` | wuzapi version (for reporting) |
| `TiersToRun` | `0,1,2,3` | Comma-separated tier numbers |
| `HostPort` | `8080` | wuzapi container port |
| `UserToken` | `user123` | User API token |
| `SkipBuild` | `false` | Skip building tests |
| `TestResultsDir` | `test-results` | Test results directory |

**Exit Codes:**
- `0` - All tests passed
- `1` - Some tests failed or validation error

**Example Output:**

```
═══════════════════════════════════════════════════════════
  WuzApi Integration Test Validation
═══════════════════════════════════════════════════════════

Version: latest
Tiers: 0,1,2,3
Port: 8080

[INFO] Repository root: D:\dev\wuzapi-client
[SUCCESS] Container is running and healthy
[SUCCESS] Session is authenticated and ready
[INFO] Building integration tests...
[SUCCESS] Build completed successfully

═══════════════════════════════════════════════════════════
  Running Tier 0 Tests
═══════════════════════════════════════════════════════════

[INFO] Tier 0 (ReadOnly) - Running tests...

Passed!  - Failed:     0, Passed:    10, Skipped:     0

[SUCCESS] Tier 0 tests PASSED

═══════════════════════════════════════════════════════════
  Running Tier 1 Tests
═══════════════════════════════════════════════════════════

[INFO] Tier 1 (Messaging) - Running tests...

Passed!  - Failed:     0, Passed:     2, Skipped:     0

[SUCCESS] Tier 1 tests PASSED

═══════════════════════════════════════════════════════════
  Validation Summary - wuzapi latest
═══════════════════════════════════════════════════════════

  Tier 0 (ReadOnly): ✓ PASS
  Tier 1 (Messaging): ✓ PASS
  Tier 2 (StateModifying): ✓ PASS
  Tier 3 (Destructive): ✓ PASS

═══════════════════════════════════════════════════════════
  ALL TESTS PASSED ✓
═══════════════════════════════════════════════════════════
```

## Test Tiers

Tests are organized into tiers by impact level:

| Tier | Name | Description | Test Count | Risk Level |
|------|------|-------------|------------|------------|
| **0** | ReadOnly | Safe read-only operations | 10 | None |
| **1** | Messaging | Rate-limited message sending | 3 | Low |
| **2** | StateModifying | Configuration changes, resource creation | 4 | Medium |
| **3** | Destructive | Session disconnect, irreversible operations | 1 | High |

### Tier 0: ReadOnly
- ✅ No state changes
- ✅ Can run anytime
- ✅ Fast execution (~5s)
- Examples: Get session status, list contacts, get groups

### Tier 1: Messaging
- ⚠️ Sends real WhatsApp messages
- ⚠️ Rate-limited (3-5s delays between tests)
- ⚠️ Requires valid recipient phone number
- Examples: Send text message, send image, send document

### Tier 2: StateModifying
- ⚠️ Modifies configuration or creates resources
- ⚠️ May leave artifacts (groups, users)
- Examples: Connect session, create group, create admin user

### Tier 3: Destructive
- 🚨 Invalidates the session
- 🚨 Requires re-authentication after running
- 🚨 Always runs last
- Examples: Disconnect session

## Common Scenarios

### New wuzapi Version Testing

```powershell
# 1. Setup with new version
.\Setup-TestSession.ps1 -WuzApiVersion "v3.1.0"

# 2. Scan QR code when prompted

# 3. Run full validation
.\Validate-WuzApiVersion.ps1 -Version "v3.1.0"
```

### Quick Regression Check

```powershell
# Only run Tier 0 (read-only) for fast validation
.\Validate-WuzApiVersion.ps1 -TiersToRun "0"
```

### Safe Testing (Skip Destructive)

```powershell
# Run Tier 0, 1, 2 (skip Tier 3)
.\Validate-WuzApiVersion.ps1 -TiersToRun "0,1,2"
```

### CI/CD Pipeline

```powershell
# In CI/CD, you would:
# 1. Pre-authenticate session manually (one-time)
# 2. Persist volume across runs
# 3. Run validation script
.\Validate-WuzApiVersion.ps1 -SkipBuild -TiersToRun "0,1,2"

# Check exit code
if ($LASTEXITCODE -ne 0) {
    Write-Error "Integration tests failed"
    exit 1
}
```

## Troubleshooting

### Container Not Starting

```powershell
# Check Docker is running
docker version

# View container logs
docker logs wuzapi-test

# Remove old container and try again
docker stop wuzapi-test
docker rm wuzapi-test
.\Setup-TestSession.ps1
```

### Session Not Authenticating

```powershell
# Manually check session status
$headers = @{ "token" = "user123" }
Invoke-RestMethod -Uri "http://localhost:8080/session/status" -Headers $headers

# Re-open QR code in browser
Start-Process "http://localhost:8080/login"

# Wait and check again after scanning
```

### Tests Failing

```powershell
# View detailed test output
dotnet test --filter "Tier=0" --logger "console;verbosity=detailed"

# Check TRX files for details
Get-ChildItem test-results\*.trx | Sort-Object LastWriteTime -Descending | Select-Object -First 1
```

### Volume Issues

```powershell
# List volumes
docker volume ls

# Inspect volume
docker volume inspect wuzapi-test-session

# Delete and recreate (loses session!)
docker volume rm wuzapi-test-session
.\Setup-TestSession.ps1
```

## Test Results

Test results are saved as TRX files in the `test-results/` directory:

```
test-results/
├── tier0-latest-20250128-143022.trx
├── tier1-latest-20250128-143045.trx
├── tier2-latest-20250128-143130.trx
└── tier3-latest-20250128-143155.trx
```

**Opening TRX Files:**
- Visual Studio: Test Explorer → Open Additional Results → Select TRX file
- VS Code: Install "Test Explorer UI" extension
- Command line: Use `trx2junit` or similar converter

## Configuration

### appsettings.Local.json (Recommended)

The recommended way to configure personal test data is using `appsettings.Local.json`:

1. **Create your local config** (first time only):
   ```powershell
   cd tests\WuzApiClient.IntegrationTests
   copy appsettings.Local.json.template appsettings.Local.json
   ```

2. **Edit `appsettings.Local.json`** with your phone number:
   ```json
   {
     "TestData": {
       "TestPhoneNumber": "5511987654321"
     }
   }
   ```

   **Note**: You typically only need to set your phone number. The base `appsettings.json` already has the correct tokens (`admin123` and `user123`) that match the setup script defaults.

3. **Why use appsettings.Local.json?**
   - It's gitignored - your personal phone number won't be committed
   - It overrides base `appsettings.json` values automatically
   - Follows .NET configuration best practices
   - Group tests will auto-create a test group using your TestPhoneNumber

### Environment Variables (Alternative)

You can also configure test behavior via environment variables:

| Variable | Purpose | Default |
|----------|---------|---------|
| `WuzApi__BaseUrl` | wuzapi endpoint | `http://localhost:8080` |
| `WuzApi__UserToken` | User API token | `user123` |
| `WuzApiAdmin__AdminToken` | Admin API token | `admin123` |
| `TestData__TestPhoneNumber` | Phone for test messages | *(required)* |

**Note**:
- `appsettings.Local.json` has highest priority and overrides environment variables
- Environment variables override `appsettings.json`
- `TestData__TestGroupId` is no longer needed - tests auto-create a group

## Advanced Usage

### Multiple Containers

```powershell
# Run multiple versions in parallel
.\Setup-TestSession.ps1 -ContainerName "wuzapi-v3-0" -HostPort 8080 -WuzApiVersion "v3.0.0"
.\Setup-TestSession.ps1 -ContainerName "wuzapi-v3-1" -HostPort 8081 -WuzApiVersion "v3.1.0"

# Test each
.\Validate-WuzApiVersion.ps1 -HostPort 8080 -Version "v3.0.0"
.\Validate-WuzApiVersion.ps1 -HostPort 8081 -Version "v3.1.0"
```

### Custom Test Filters

```powershell
# Run only specific tests
dotnet test --filter "FullyQualifiedName~MessagingTests"

# Run by category
dotnet test --filter "Category=LiveApi&Tier!=3"
```

## See Also

- [wuzapi GitHub](https://github.com/asternic/wuzapi)

## Support

For issues or questions:
1. Check troubleshooting section above
2. Review container logs: `docker logs wuzapi-test`
3. Check test output in `test-results/` directory
4. Consult integration testing strategy documentation

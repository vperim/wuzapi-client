<#
.SYNOPSIS
    Sets up the WuzAPI Event Dashboard with Docker Compose and guides through WhatsApp authentication.

.DESCRIPTION
    This script automates the Event Dashboard setup process:
    1. Starts all services via docker-compose (RabbitMQ, wuzapi, dashboard)
    2. Waits for services to become healthy
    3. Creates test user if needed
    4. Initiates WhatsApp connection
    5. Guides through QR code authentication
    6. Opens the dashboard in browser

.PARAMETER SkipBrowserOpen
    Skip automatically opening the dashboard in browser after setup

.PARAMETER AdminToken
    The admin token for wuzapi admin API (default: "admin123")

.PARAMETER UserToken
    The user token for wuzapi user API (default: "user123")

.PARAMETER WuzApiPort
    The host port wuzapi is exposed on (default: 8080)

.PARAMETER DashboardPort
    The host port the dashboard is exposed on (default: 5000)

.PARAMETER Cleanup
    Stop and remove all containers and volumes (teardown mode)

.EXAMPLE
    .\Setup-EventDashboard.ps1
    Starts the Event Dashboard with default settings

.EXAMPLE
    .\Setup-EventDashboard.ps1 -Cleanup
    Stops and removes all Event Dashboard containers and volumes

.EXAMPLE
    .\Setup-EventDashboard.ps1 -SkipBrowserOpen
    Starts without opening browser automatically

.NOTES
    Requirements:
    - Docker Desktop installed and running
    - PowerShell 5.1 or PowerShell 7+
    - Internet connection (to pull Docker images)
#>

[CmdletBinding()]
param(
    [Parameter()]
    [switch]$SkipBrowserOpen,

    [Parameter()]
    [switch]$Cleanup,

    [Parameter()]
    [string]$AdminToken = "admin123",

    [Parameter()]
    [string]$UserToken = "user123",

    [Parameter()]
    [int]$WuzApiPort = 8080,

    [Parameter()]
    [int]$DashboardPort = 5000
)

$ErrorActionPreference = "Stop"

# ============================================================================
# Logging Functions (reused from Setup-TestSession.ps1)
# ============================================================================

function Write-Status {
    param([string]$Message)
    Write-Host "[INFO] " -ForegroundColor Cyan -NoNewline
    Write-Host $Message
}

function Write-Success {
    param([string]$Message)
    Write-Host "[SUCCESS] " -ForegroundColor Green -NoNewline
    Write-Host $Message
}

function Write-Warning {
    param([string]$Message)
    Write-Host "[WARNING] " -ForegroundColor Yellow -NoNewline
    Write-Host $Message
}

function Write-ErrorMessage {
    param([string]$Message)
    Write-Host "[ERROR] " -ForegroundColor Red -NoNewline
    Write-Host $Message
}

# ============================================================================
# Docker Functions
# ============================================================================

function Test-DockerRunning {
    Write-Status "Checking if Docker is running..."

    try {
        $null = docker version 2>&1
        if ($LASTEXITCODE -ne 0) {
            throw "Docker command failed"
        }
        Write-Success "Docker is running"
        return $true
    }
    catch {
        Write-ErrorMessage "Docker is not running or not installed"
        Write-Host "Please start Docker Desktop and try again."
        return $false
    }
}

function Start-DockerCompose {
    Write-Status "Starting Event Dashboard services via docker-compose..."
    Write-Status "This may take a few minutes on first run (pulling images)..."

    # Temporarily disable error action preference for docker command
    # Docker compose outputs status to stderr which PowerShell treats as errors
    $previousErrorAction = $ErrorActionPreference
    $ErrorActionPreference = "Continue"

    try {
        # Run docker compose and capture output
        $output = & docker compose up -d 2>&1
        $exitCode = $LASTEXITCODE

        # Show output for visibility
        if ($output) {
            $output | ForEach-Object {
                $line = $_.ToString()
                if ($line -match "Created|Started|Running") {
                    Write-Host "  $line" -ForegroundColor Gray
                }
            }
        }

        if ($exitCode -ne 0) {
            Write-ErrorMessage "docker-compose failed with exit code $exitCode"
            $output | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
            return $false
        }

        Write-Success "Docker Compose started successfully"
        return $true
    }
    finally {
        $ErrorActionPreference = $previousErrorAction
    }
}

function Stop-DockerCompose {
    Write-Status "Stopping Event Dashboard services..."

    # Temporarily disable error action preference for docker command
    $previousErrorAction = $ErrorActionPreference
    $ErrorActionPreference = "Continue"

    try {
        $output = & docker compose down -v 2>&1
        $exitCode = $LASTEXITCODE

        # Show output for visibility
        if ($output) {
            $output | ForEach-Object {
                $line = $_.ToString()
                if ($line -match "Stopped|Removed|Stopping|Removing") {
                    Write-Host "  $line" -ForegroundColor Gray
                }
            }
        }

        if ($exitCode -eq 0) {
            Write-Success "All services stopped and volumes removed"
        }
        else {
            Write-Warning "Some services may not have stopped cleanly"
        }
    }
    finally {
        $ErrorActionPreference = $previousErrorAction
    }
}

function Get-WuzApiContainerName {
    # Get the wuzapi container name from docker-compose
    $containers = docker compose ps --format json 2>&1 | ConvertFrom-Json
    $wuzapiContainer = $containers | Where-Object { $_.Service -eq "wuzapi" }

    if ($wuzapiContainer) {
        return $wuzapiContainer.Name
    }

    # Fallback: try common naming patterns
    $fallbackNames = @(
        "wuzapiclienteventdashboard-wuzapi-1",
        "event-dashboard-wuzapi-1",
        "wuzapi"
    )

    foreach ($name in $fallbackNames) {
        $exists = docker ps --filter "name=$name" --format "{{.Names}}" 2>&1
        if ($exists -eq $name) {
            return $name
        }
    }

    return "wuzapi"
}

function Wait-ServicesHealthy {
    param(
        [int]$TimeoutSeconds = 120
    )

    Write-Status "Waiting for services to become healthy (timeout: ${TimeoutSeconds}s)..."

    $elapsed = 0
    $checkInterval = 5

    while ($elapsed -lt $TimeoutSeconds) {
        Start-Sleep -Seconds $checkInterval
        $elapsed += $checkInterval

        # Check RabbitMQ
        try {
            $rabbitHealth = Invoke-WebRequest -Uri "http://localhost:15672" -TimeoutSec 2 -ErrorAction Stop
            $rabbitReady = $rabbitHealth.StatusCode -eq 200
        }
        catch {
            $rabbitReady = $false
        }

        # Check wuzapi
        try {
            $wuzapiHealth = Invoke-WebRequest -Uri "http://localhost:${WuzApiPort}/health" -TimeoutSec 2 -ErrorAction Stop
            $wuzapiReady = $wuzapiHealth.StatusCode -eq 200
        }
        catch {
            $wuzapiReady = $false
        }

        # Check dashboard
        try {
            $dashboardHealth = Invoke-WebRequest -Uri "http://localhost:${DashboardPort}" -TimeoutSec 2 -ErrorAction Stop
            $dashboardReady = $dashboardHealth.StatusCode -eq 200
        }
        catch {
            $dashboardReady = $false
        }

        $statusLine = "  RabbitMQ: $(if($rabbitReady){'OK'}else{'...'})"
        $statusLine += "  wuzapi: $(if($wuzapiReady){'OK'}else{'...'})"
        $statusLine += "  Dashboard: $(if($dashboardReady){'OK'}else{'...'})"
        Write-Host "`r$statusLine" -NoNewline

        if ($rabbitReady -and $wuzapiReady -and $dashboardReady) {
            Write-Host ""
            Write-Success "All services are healthy"
            return $true
        }
    }

    Write-Host ""
    Write-ErrorMessage "Timeout waiting for services to become healthy"

    if (-not $rabbitReady) { Write-ErrorMessage "  - RabbitMQ not responding" }
    if (-not $wuzapiReady) { Write-ErrorMessage "  - wuzapi not responding" }
    if (-not $dashboardReady) { Write-ErrorMessage "  - Dashboard not responding" }

    return $false
}

# ============================================================================
# wuzapi API Functions (adapted from Setup-TestSession.ps1)
# ============================================================================

function Ensure-UserExists {
    param(
        [int]$Port,
        [string]$AdminToken,
        [string]$UserToken,
        [string]$UserName = "DashboardUser"
    )

    Write-Status "Ensuring dashboard user exists..."

    $baseUrl = "http://localhost:$Port"
    $adminEndpoint = "$baseUrl/admin/users"

    try {
        $headers = @{
            "Authorization" = $AdminToken
        }

        $users = Invoke-RestMethod -Uri $adminEndpoint -Headers $headers -ErrorAction Stop

        $existingUser = $users | Where-Object { $_.token -eq $UserToken }

        if ($existingUser) {
            Write-Success "User '$UserName' already exists"
            return $true
        }

        Write-Status "Creating user '$UserName'..."
        $body = @{
            name = $UserName
            token = $UserToken
        } | ConvertTo-Json

        $headers["Content-Type"] = "application/json"
        $null = Invoke-RestMethod -Uri $adminEndpoint -Method Post -Headers $headers -Body $body -ErrorAction Stop

        Write-Success "Created user '$UserName'"
        return $true
    }
    catch {
        $errorMessage = $_.ToString()

        # 409 Conflict means user already exists - this is actually success for our purposes
        if ($errorMessage -match '"code":409' -or $errorMessage -match 'user with this token already exists') {
            Write-Success "User '$UserName' already exists"
            return $true
        }

        Write-ErrorMessage "Failed to ensure user exists: $_"
        return $false
    }
}

function Get-SessionStatus {
    param(
        [int]$Port,
        [string]$UserToken
    )

    $baseUrl = "http://localhost:$Port"
    $sessionEndpoint = "$baseUrl/session/status"

    try {
        $headers = @{
            "Token" = $UserToken
        }

        $response = Invoke-RestMethod -Uri $sessionEndpoint -Headers $headers -ErrorAction Stop

        return [PSCustomObject]@{
            Connected = $response.data.Connected
            LoggedIn = $response.data.LoggedIn
            Ready = $response.data.Connected -and $response.data.LoggedIn
        }
    }
    catch {
        return [PSCustomObject]@{
            Connected = $false
            LoggedIn = $false
            Ready = $false
        }
    }
}

function Start-SessionConnection {
    param(
        [int]$Port,
        [string]$UserToken
    )

    Write-Status "Initiating WhatsApp connection..."

    $baseUrl = "http://localhost:$Port"
    $connectEndpoint = "$baseUrl/session/connect"

    try {
        $headers = @{
            "Token" = $UserToken
            "Content-Type" = "application/json"
        }

        $body = @{
            Subscribe = @("All")
            Immediate = $true
        } | ConvertTo-Json

        $response = Invoke-RestMethod -Uri $connectEndpoint -Method Post -Headers $headers -Body $body -ErrorAction Stop

        if ($response.success) {
            Write-Success "Connection initiated, QR code should be available"
            return $true
        }
        else {
            Write-Warning "Connection response: $($response.error)"
            return $true
        }
    }
    catch {
        Write-Warning "Could not initiate connection: $_"
        return $true
    }
}

function Show-QrCodeInstructions {
    param(
        [int]$Port,
        [string]$UserToken,
        [string]$ContainerName
    )

    Start-SessionConnection -Port $Port -UserToken $UserToken | Out-Null

    Write-Status "Waiting for QR code generation..."
    Start-Sleep -Seconds 3

    Write-Host ""
    Write-Host "-----------------------------------------------------------" -ForegroundColor Yellow
    Write-Host "  WhatsApp QR Code Authentication Required" -ForegroundColor Yellow
    Write-Host "-----------------------------------------------------------" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To scan the QR code:" -ForegroundColor Cyan
    Write-Host "  1. Open a NEW terminal window" -ForegroundColor White
    Write-Host "  2. Run: docker logs $ContainerName -f" -ForegroundColor White
    Write-Host "  3. The QR code will be displayed in the logs" -ForegroundColor White
    Write-Host ""
    Write-Host "Then on your phone:" -ForegroundColor Cyan
    Write-Host "  1. Open WhatsApp" -ForegroundColor White
    Write-Host "  2. Go to Settings > Linked Devices" -ForegroundColor White
    Write-Host "  3. Tap 'Link a Device'" -ForegroundColor White
    Write-Host "  4. Scan the QR code from the logs" -ForegroundColor White
    Write-Host ""
    Write-Host "-----------------------------------------------------------" -ForegroundColor Yellow
    Write-Host ""
}

function Wait-SessionAuthenticated {
    param(
        [int]$Port,
        [string]$UserToken,
        [int]$TimeoutSeconds = 120
    )

    Write-Status "Waiting for WhatsApp authentication (timeout: ${TimeoutSeconds}s)..."
    Write-Host "Please scan the QR code from the container logs..."

    $elapsed = 0
    $checkInterval = 3

    while ($elapsed -lt $TimeoutSeconds) {
        Start-Sleep -Seconds $checkInterval
        $elapsed += $checkInterval

        $status = Get-SessionStatus -Port $Port -UserToken $UserToken

        if ($status.Ready) {
            Write-Host ""
            Write-Success "Session authenticated successfully!"
            return $true
        }

        Write-Host "." -NoNewline
    }

    Write-Host ""
    Write-Warning "Authentication timeout reached"
    Write-Host "You can re-run this script or manually scan the QR code."
    return $false
}

# ============================================================================
# Main Script
# ============================================================================

try {
    Write-Host ""
    Write-Host "-----------------------------------------------------------" -ForegroundColor Cyan
    Write-Host "  WuzAPI Event Dashboard Setup" -ForegroundColor Cyan
    Write-Host "-----------------------------------------------------------" -ForegroundColor Cyan
    Write-Host ""

    # Handle cleanup mode
    if ($Cleanup) {
        Write-Status "Running in cleanup mode..."
        Stop-DockerCompose
        Write-Host ""
        Write-Success "Cleanup complete!"
        exit 0
    }

    # Step 1: Check Docker
    if (-not (Test-DockerRunning)) {
        exit 1
    }

    # Step 2: Start docker-compose
    if (-not (Start-DockerCompose)) {
        exit 1
    }

    # Step 3: Wait for services to become healthy
    if (-not (Wait-ServicesHealthy)) {
        Write-Host ""
        Write-Host "Troubleshooting tips:" -ForegroundColor Yellow
        Write-Host "  - Check container logs: docker compose logs" -ForegroundColor White
        Write-Host "  - Restart services: docker compose restart" -ForegroundColor White
        Write-Host "  - Clean restart: .\Setup-EventDashboard.ps1 -Cleanup; .\Setup-EventDashboard.ps1" -ForegroundColor White
        exit 1
    }

    # Step 4: Ensure user exists
    if (-not (Ensure-UserExists -Port $WuzApiPort -AdminToken $AdminToken -UserToken $UserToken)) {
        Write-ErrorMessage "Failed to create dashboard user"
        exit 1
    }

    # Step 5: Check session status
    Write-Status "Checking session authentication status..."
    $sessionStatus = Get-SessionStatus -Port $WuzApiPort -UserToken $UserToken

    if ($sessionStatus.Ready) {
        Write-Success "Session is already authenticated (persisted from previous run)"
    }
    else {
        # Step 6: Get container name and show QR code instructions
        $containerName = Get-WuzApiContainerName
        Show-QrCodeInstructions -Port $WuzApiPort -UserToken $UserToken -ContainerName $containerName

        # Step 7: Wait for authentication
        $authenticated = Wait-SessionAuthenticated -Port $WuzApiPort -UserToken $UserToken

        if (-not $authenticated) {
            Write-Host ""
            Write-Host "Authentication was not completed within the timeout period." -ForegroundColor Yellow
            Write-Host "The services are still running. You can:" -ForegroundColor White
            Write-Host "  1. Run: docker logs $containerName -f" -ForegroundColor Cyan
            Write-Host "  2. Scan the QR code when it appears" -ForegroundColor Cyan
            Write-Host "  3. Re-run this script to verify" -ForegroundColor Cyan
        }
    }

    # Step 8: Show success message and open browser
    Write-Host ""
    Write-Host "-----------------------------------------------------------" -ForegroundColor Green
    Write-Host "  Event Dashboard Ready!" -ForegroundColor Green
    Write-Host "-----------------------------------------------------------" -ForegroundColor Green
    Write-Host ""
    Write-Host "Services running:" -ForegroundColor White
    Write-Host "  Dashboard:  http://localhost:$DashboardPort" -ForegroundColor Cyan
    Write-Host "  RabbitMQ:   http://localhost:15672 (guest/guest)" -ForegroundColor Cyan
    Write-Host "  wuzapi:     http://localhost:$WuzApiPort" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Useful commands:" -ForegroundColor Gray
    Write-Host "  View logs:     docker compose logs -f" -ForegroundColor Gray
    Write-Host "  View wuzapi:   docker logs $containerName -f" -ForegroundColor Gray
    Write-Host "  Stop all:      .\Setup-EventDashboard.ps1 -Cleanup" -ForegroundColor Gray
    Write-Host ""

    if (-not $SkipBrowserOpen) {
        Write-Status "Opening dashboard in browser..."
        Start-Process "http://localhost:$DashboardPort"
    }

    exit 0
}
catch {
    Write-Host ""
    Write-ErrorMessage "Setup failed: $_"
    Write-Host $_.ScriptStackTrace
    exit 1
}

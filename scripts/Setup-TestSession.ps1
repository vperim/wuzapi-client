<#
.SYNOPSIS
    Sets up a wuzapi test session with Docker volume and initiates QR code authentication.

.DESCRIPTION
    This script creates the necessary Docker volume for session persistence,
    starts a wuzapi container, and guides the user through WhatsApp QR code
    authentication. Once authenticated, the session is persisted for integration tests.

.PARAMETER WuzApiVersion
    The wuzapi Docker image version to use (default: "latest")

.PARAMETER AdminToken
    The admin token for wuzapi admin API (default: "admin123")

.PARAMETER UserToken
    The user token for wuzapi user API (default: "user123")

.PARAMETER VolumeName
    The Docker volume name for session persistence (default: "wuzapi-test-session")

.PARAMETER ContainerName
    The Docker container name (default: "wuzapi-test")

.PARAMETER HostPort
    The host port to expose wuzapi on (default: 8080)

.EXAMPLE
    .\Setup-TestSession.ps1
    Uses default settings (latest version, port 8080)

.EXAMPLE
    .\Setup-TestSession.ps1 -WuzApiVersion "v3.0.0" -HostPort 9090
    Uses specific version and custom port

.NOTES
    Requirements:
    - Docker Desktop installed and running
    - PowerShell 5.1 or PowerShell 7+
    - Internet connection (to pull Docker image)
#>

[CmdletBinding()]
param(
    [Parameter()]
    [string]$WuzApiVersion = "latest",

    [Parameter()]
    [string]$AdminToken = "admin123",

    [Parameter()]
    [string]$UserToken = "user123",

    [Parameter()]
    [string]$VolumeName = "wuzapi-test-session",

    [Parameter()]
    [string]$ContainerName = "wuzapi-test",

    [Parameter()]
    [int]$HostPort = 8080
)

$ErrorActionPreference = "Stop"

# Use Write-Host -ForegroundColor for PowerShell 5.1 compatibility
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

function Stop-ExistingContainer {
    param([string]$Name)

    Write-Status "Checking for existing container: $Name"

    # Check if container exists (running or stopped)
    $existingContainer = docker ps -a --filter "name=$Name" --format "{{.Names}}" 2>&1 | Where-Object { $_ -eq $Name }

    if ($existingContainer) {
        Write-Warning "Found existing container '$Name', stopping and removing..."

        # Stop container if running (ignore errors if already stopped)
        docker stop $Name 2>&1 | Out-Null

        # Remove container
        docker rm -f $Name 2>&1 | Out-Null

        Write-Success "Removed existing container"
    }
    else {
        Write-Status "No existing container found"
    }
}

function New-DockerVolumeIfNotExists {
    param([string]$Name)

    Write-Status "Checking Docker volume: $Name"

    $existingVolume = docker volume ls --filter "name=^${Name}$" --format "{{.Name}}" 2>&1

    if ($existingVolume -eq $Name) {
        Write-Success "Volume '$Name' already exists, reusing for session persistence"
    }
    else {
        Write-Status "Creating Docker volume: $Name"
        docker volume create $Name | Out-Null

        if ($LASTEXITCODE -eq 0) {
            Write-Success "Created volume '$Name'"
        }
        else {
            throw "Failed to create Docker volume"
        }
    }
}

function Start-WuzApiContainer {
    param(
        [string]$Version,
        [string]$Name,
        [string]$Volume,
        [int]$Port,
        [string]$AdminToken,
        [string]$UserToken
    )

    Write-Status "Starting wuzapi container (version: $Version)..."
    Write-Status "Container name: $Name"
    Write-Status "Port mapping: ${Port}:8080"
    Write-Status "Volume: $Volume"

    $dockerArgs = @(
        "run"
        "-d"
        "--name", $Name
        "-p", "${Port}:8080"
        "-v", "${Volume}:/data"
        "-e", "WUZAPI_ADMIN_TOKEN=$AdminToken"
        "asternic/wuzapi:$Version"
    )

    $containerId = docker @dockerArgs 2>&1

    if ($LASTEXITCODE -ne 0) {
        Write-ErrorMessage "Failed to start container: $containerId"
        throw "Container startup failed"
    }

    Write-Success "Container started: $($containerId.Substring(0, 12))"
    return $containerId
}

function Wait-ContainerHealthy {
    param(
        [string]$Name,
        [int]$Port,
        [int]$TimeoutSeconds = 60
    )

    Write-Status "Waiting for container to become healthy (timeout: ${TimeoutSeconds}s)..."

    $baseUrl = "http://localhost:$Port"
    $healthEndpoint = "$baseUrl/health"
    $elapsed = 0
    $checkInterval = 2

    while ($elapsed -lt $TimeoutSeconds) {
        Start-Sleep -Seconds $checkInterval
        $elapsed += $checkInterval

        try {
            $response = Invoke-WebRequest -Uri $healthEndpoint -TimeoutSec 2 -ErrorAction Stop

            if ($response.StatusCode -eq 200) {
                Write-Success "Container is healthy and responding"
                return $true
            }
        }
        catch {
            Write-Host "." -NoNewline
        }
    }

    Write-Host ""
    Write-ErrorMessage "Container health check timed out after ${TimeoutSeconds}s"
    return $false
}

function Ensure-UserExists {
    param(
        [int]$Port,
        [string]$AdminToken,
        [string]$UserToken,
        [string]$UserName = "TestUser"
    )

    Write-Status "Ensuring test user exists..."

    $baseUrl = "http://localhost:$Port"
    $adminEndpoint = "$baseUrl/admin/users"

    try {
        # Check if user already exists by listing all users
        $headers = @{
            "Authorization" = $AdminToken
        }

        $users = Invoke-RestMethod -Uri $adminEndpoint -Headers $headers -ErrorAction Stop

        $existingUser = $users | Where-Object { $_.token -eq $UserToken }

        if ($existingUser) {
            Write-Success "User '$UserName' already exists"
            return $true
        }

        # Create new user
        Write-Status "Creating user '$UserName' with token..."
        $body = @{
            name = $UserName
            token = $UserToken
        } | ConvertTo-Json

        $headers["Content-Type"] = "application/json"
        $result = Invoke-RestMethod -Uri $adminEndpoint -Method Post -Headers $headers -Body $body -ErrorAction Stop

        Write-Success "Created user '$UserName'"
        return $true
    }
    catch {
        Write-ErrorMessage "Failed to ensure user exists: $_"
        return $false
    }
}

function Get-SessionStatus {
    param(
        [int]$Port,
        [string]$UserToken
    )

    Write-Status "Checking session authentication status..."

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
        Write-ErrorMessage "Failed to get session status: $_"
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
            Subscribe = @("Message")
            Immediate = $true
        } | ConvertTo-Json

        $response = Invoke-RestMethod -Uri $connectEndpoint -Method Post -Headers $headers -Body $body -ErrorAction Stop

        if ($response.success) {
            Write-Success "Connection initiated, QR code should be available"
            return $true
        }
        else {
            Write-Warning "Connection response: $($response.error)"
            return $true  # Continue anyway, QR might still be available
        }
    }
    catch {
        Write-Warning "Could not initiate connection: $_"
        return $true  # Continue anyway, user can click Login with QR in browser
    }
}

function Show-QrCodeInstructions {
    param(
        [int]$Port,
        [string]$UserToken,
        [string]$ContainerName
    )

    # Initiate connection first so QR code is available
    Start-SessionConnection -Port $Port -UserToken $UserToken | Out-Null

    # Wait for QR code to generate
    Write-Status "Waiting for QR code generation..."
    Start-Sleep -Seconds 3

    $loginUrl = "http://localhost:$Port/login?token=$UserToken"

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
    Write-Host "Alternative: Open browser at $loginUrl" -ForegroundColor Gray
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
    Write-Host "You can re-run this script or manually scan the QR code later."
    return $false
}

# ============================================================================
# Main Script
# ============================================================================

try {
    Write-Host ""
    Write-Host "-----------------------------------------------------------" -ForegroundColor Cyan
    Write-Host "  WuzApi Test Session Setup" -ForegroundColor Cyan
    Write-Host "-----------------------------------------------------------" -ForegroundColor Cyan
    Write-Host ""

    # Step 1: Check Docker
    if (-not (Test-DockerRunning)) {
        exit 1
    }

    # Step 2: Stop existing container if present
    Stop-ExistingContainer -Name $ContainerName

    # Step 3: Create or verify volume
    New-DockerVolumeIfNotExists -Name $VolumeName

    # Step 4: Start container
    $containerId = Start-WuzApiContainer `
        -Version $WuzApiVersion `
        -Name $ContainerName `
        -Volume $VolumeName `
        -Port $HostPort `
        -AdminToken $AdminToken `
        -UserToken $UserToken

    # Step 5: Wait for container health
    if (-not (Wait-ContainerHealthy -Name $ContainerName -Port $HostPort)) {
        Write-ErrorMessage "Container failed to become healthy"
        Write-Host "Check container logs: docker logs $ContainerName"
        exit 1
    }

    # Step 5.5: Ensure test user exists
    if (-not (Ensure-UserExists -Port $HostPort -AdminToken $AdminToken -UserToken $UserToken)) {
        Write-ErrorMessage "Failed to create test user"
        exit 1
    }

    # Step 6: Check session status
    $sessionStatus = Get-SessionStatus -Port $HostPort -UserToken $UserToken

    if ($sessionStatus.Ready) {
        Write-Success "Session is already authenticated (persisted from previous run)"
        Write-Host ""
        Write-Host "Session is ready for integration tests!" -ForegroundColor Green
        Write-Host "You can now run: .\Validate-WuzApiVersion.ps1" -ForegroundColor Cyan
    }
    else {
        # Step 7: Show QR code and wait for authentication
        Show-QrCodeInstructions -Port $HostPort -UserToken $UserToken -ContainerName $ContainerName

        $authenticated = Wait-SessionAuthenticated -Port $HostPort -UserToken $UserToken

        if ($authenticated) {
            Write-Host ""
            Write-Host "-----------------------------------------------------------" -ForegroundColor Green
            Write-Host "  Setup Complete!" -ForegroundColor Green
            Write-Host "-----------------------------------------------------------" -ForegroundColor Green
            Write-Host ""
            Write-Host "Your WhatsApp session is now authenticated and persisted." -ForegroundColor White
            Write-Host "You can now run integration tests:" -ForegroundColor White
            Write-Host ""
            Write-Host "  .\scripts\Validate-WuzApiVersion.ps1" -ForegroundColor Cyan
            Write-Host ""
        }
        else {
            Write-Host ""
            Write-Host "Authentication was not completed within the timeout period." -ForegroundColor Yellow
            Write-Host "The container is still running. You can:" -ForegroundColor White
            Write-Host "  1. Manually visit: http://localhost:${HostPort}/login?token=$UserToken" -ForegroundColor Cyan
            Write-Host "  2. Scan the QR code" -ForegroundColor Cyan
            Write-Host "  3. Re-run this script to verify authentication" -ForegroundColor Cyan
        }
    }

    Write-Host ""
    Write-Host "Container info:" -ForegroundColor Gray
    Write-Host "  Name: $ContainerName" -ForegroundColor Gray
    Write-Host "  URL: http://localhost:$HostPort" -ForegroundColor Gray
    Write-Host "  Volume: $VolumeName" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Useful commands:" -ForegroundColor Gray
    Write-Host "  View logs: docker logs $ContainerName" -ForegroundColor Gray
    Write-Host "  Stop: docker stop $ContainerName" -ForegroundColor Gray
    Write-Host "  Remove: docker rm $ContainerName" -ForegroundColor Gray
    Write-Host ""

    exit 0
}
catch {
    Write-Host ""
    Write-ErrorMessage "Setup failed: $_"
    Write-Host $_.ScriptStackTrace
    exit 1
}

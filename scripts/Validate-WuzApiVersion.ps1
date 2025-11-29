<#
.SYNOPSIS
    Validates a wuzapi version by running integration tests against a running container.

.DESCRIPTION
    This script runs the integration test suite against a wuzapi container.
    It checks container health, verifies session authentication, and executes
    tests in tier order. Test results are saved to TRX files for analysis.

.PARAMETER Version
    The wuzapi version being tested (for reporting purposes, default: "latest")

.PARAMETER TiersToRun
    Comma-separated list of tiers to run (e.g., "0,1,2" or "0" for quick validation)
    Default: "0,1,2,3" (all tiers)

.PARAMETER HostPort
    The host port where wuzapi is running (default: 8080)

.PARAMETER UserToken
    The user token for API authentication (default: "user123")

.PARAMETER SkipBuild
    Skip building the test project (use existing build)

.PARAMETER TestResultsDir
    Directory to save test results (default: "test-results")

.EXAMPLE
    .\Validate-WuzApiVersion.ps1
    Runs all tiers against wuzapi:latest on port 8080

.EXAMPLE
    .\Validate-WuzApiVersion.ps1 -Version "v3.0.0" -TiersToRun "0"
    Quick validation of v3.0.0 using only Tier 0 (read-only) tests

.EXAMPLE
    .\Validate-WuzApiVersion.ps1 -TiersToRun "0,1,2"
    Runs Tier 0, 1, and 2, skipping Tier 3 (destructive tests)

.NOTES
    Requirements:
    - .NET SDK installed
    - Docker container running (use Setup-TestSession.ps1 first)
    - Authenticated WhatsApp session
    - PowerShell 5.1 or PowerShell 7+
#>

[CmdletBinding()]
param(
    [Parameter()]
    [string]$Version = "latest",

    [Parameter()]
    [string]$TiersToRun = "0,1,2,3",

    [Parameter()]
    [int]$HostPort = 8080,

    [Parameter()]
    [string]$UserToken = "user123",

    [Parameter()]
    [switch]$SkipBuild,

    [Parameter()]
    [string]$TestResultsDir = "test-results"
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

function Get-RepositoryRoot {
    $scriptPath = $PSScriptRoot

    if ([string]::IsNullOrEmpty($scriptPath)) {
        $scriptPath = Get-Location
    }

    # Navigate up from scripts/ to repository root
    $repoRoot = Split-Path -Parent $scriptPath

    if (-not (Test-Path "$repoRoot\wuzapi-client.sln")) {
        throw "Could not find repository root (expected wuzapi-client.sln)"
    }

    return $repoRoot
}

function Test-ContainerRunning {
    param([int]$Port)

    Write-Status "Checking if wuzapi container is running on port $Port..."

    $baseUrl = "http://localhost:$Port"
    $healthEndpoint = "$baseUrl/health"

    try {
        $response = Invoke-WebRequest -Uri $healthEndpoint -TimeoutSec 5 -ErrorAction Stop

        if ($response.StatusCode -eq 200) {
            Write-Success "Container is running and healthy"
            return $true
        }
    }
    catch {
        Write-ErrorMessage "Container is not responding on port $Port"
        Write-Host "Please run Setup-TestSession.ps1 first to start the container."
        return $false
    }
}

function Test-SessionAuthenticated {
    param(
        [int]$Port,
        [string]$Token
    )

    Write-Status "Checking WhatsApp session authentication..."

    $baseUrl = "http://localhost:$Port"
    $sessionEndpoint = "$baseUrl/session/status"

    try {
        $headers = @{
            "Token" = $Token
        }

        $response = Invoke-RestMethod -Uri $sessionEndpoint -Headers $headers -ErrorAction Stop

        if ($response.data.Connected -and $response.data.LoggedIn) {
            Write-Success "Session is authenticated and ready"
            return $true
        }
        else {
            Write-Warning "Session is not fully authenticated"
            Write-Host "  Connected: $($response.data.Connected)"
            Write-Host "  LoggedIn: $($response.data.LoggedIn)"
            Write-Host ""
            Write-Host "Please run Setup-TestSession.ps1 to authenticate."
            return $false
        }
    }
    catch {
        Write-ErrorMessage "Failed to check session status: $_"
        return $false
    }
}

function Invoke-BuildTests {
    param([string]$RepoRoot)

    if ($SkipBuild) {
        Write-Status "Skipping build (--SkipBuild specified)"
        return $true
    }

    Write-Status "Building integration tests..."

    $testProject = Join-Path $RepoRoot "tests\WuzApiClient.IntegrationTests\WuzApiClient.IntegrationTests.csproj"

    if (-not (Test-Path $testProject)) {
        Write-ErrorMessage "Test project not found: $testProject"
        return $false
    }

    $buildOutput = dotnet build $testProject 2>&1

    if ($LASTEXITCODE -eq 0) {
        Write-Success "Build completed successfully"
        return $true
    }
    else {
        Write-ErrorMessage "Build failed"
        Write-Host $buildOutput
        return $false
    }
}

function New-TestResultsDirectory {
    param([string]$RepoRoot, [string]$DirName)

    $resultsDir = Join-Path $RepoRoot $DirName

    if (-not (Test-Path $resultsDir)) {
        Write-Status "Creating test results directory: $resultsDir"
        New-Item -ItemType Directory -Path $resultsDir -Force | Out-Null
    }

    return $resultsDir
}

function Invoke-TierTests {
    param(
        [string]$RepoRoot,
        [string]$Tier,
        [string]$ResultsDir,
        [string]$Version
    )

    $testProject = Join-Path $RepoRoot "tests\WuzApiClient.IntegrationTests\WuzApiClient.IntegrationTests.csproj"
    $timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
    $trxFile = Join-Path $ResultsDir "tier${Tier}-${Version}-${timestamp}.trx"

    Write-Host ""
    Write-Host "-----------------------------------------------------------" -ForegroundColor Cyan
    Write-Host "  Running Tier $Tier Tests" -ForegroundColor Cyan
    Write-Host "-----------------------------------------------------------" -ForegroundColor Cyan
    Write-Host ""

    $tierName = switch ($Tier) {
        "0" { "ReadOnly" }
        "1" { "Messaging" }
        "2" { "StateModifying" }
        "3" { "Destructive" }
        default { "Unknown" }
    }

    Write-Status "Tier $Tier ($tierName) - Running tests..."

    $testArgs = @(
        "test"
        $testProject
        "--no-build"
        "--filter", "Tier=$Tier"
        "--logger", "trx;LogFileName=$trxFile"
        "--logger", "console;verbosity=normal"
    )

    $testOutput = dotnet @testArgs 2>&1
    $exitCode = $LASTEXITCODE

    Write-Host ""

    if ($exitCode -eq 0) {
        Write-Success "Tier $Tier tests PASSED"
        return [PSCustomObject]@{
            Tier = $Tier
            TierName = $tierName
            Passed = $true
            ExitCode = $exitCode
            TrxFile = $trxFile
        }
    }
    else {
        Write-ErrorMessage "Tier $Tier tests FAILED"
        return [PSCustomObject]@{
            Tier = $Tier
            TierName = $tierName
            Passed = $false
            ExitCode = $exitCode
            TrxFile = $trxFile
        }
    }
}

function Show-TestSummary {
    param(
        [array]$Results,
        [string]$Version,
        [string]$ResultsDir
    )

    Write-Host ""
    Write-Host "-----------------------------------------------------------" -ForegroundColor Cyan
    Write-Host "  Validation Summary - wuzapi $Version" -ForegroundColor Cyan
    Write-Host "-----------------------------------------------------------" -ForegroundColor Cyan
    Write-Host ""

    $allPassed = $true

    foreach ($result in $Results) {
        $status = if ($result.Passed) { "[PASS]" } else { "[FAIL]" }
        $color = if ($result.Passed) { "Green" } else { "Red" }

        Write-Host "  Tier $($result.Tier) ($($result.TierName)): " -NoNewline
        Write-Host $status -ForegroundColor $color

        if (-not $result.Passed) {
            $allPassed = $false
        }
    }

    Write-Host ""

    if ($allPassed) {
        Write-Host "-----------------------------------------------------------" -ForegroundColor Green
        Write-Host "  ALL TESTS PASSED" -ForegroundColor Green
        Write-Host "-----------------------------------------------------------" -ForegroundColor Green
    }
    else {
        Write-Host "-----------------------------------------------------------" -ForegroundColor Red
        Write-Host "  SOME TESTS FAILED" -ForegroundColor Red
        Write-Host "-----------------------------------------------------------" -ForegroundColor Red
    }

    Write-Host ""
    Write-Host "Test results saved to: $ResultsDir" -ForegroundColor Gray
    Write-Host ""

    foreach ($result in $Results) {
        Write-Host "  Tier $($result.Tier): $($result.TrxFile)" -ForegroundColor Gray
    }

    Write-Host ""

    return $allPassed
}

# ============================================================================
# Main Script
# ============================================================================

try {
    Write-Host ""
    Write-Host "-----------------------------------------------------------" -ForegroundColor Cyan
    Write-Host "  WuzApi Integration Test Validation" -ForegroundColor Cyan
    Write-Host "-----------------------------------------------------------" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Version: $Version" -ForegroundColor White
    Write-Host "Tiers: $TiersToRun" -ForegroundColor White
    Write-Host "Port: $HostPort" -ForegroundColor White
    Write-Host ""

    # Step 1: Find repository root
    $repoRoot = Get-RepositoryRoot
    Write-Status "Repository root: $repoRoot"

    # Step 2: Check container health
    if (-not (Test-ContainerRunning -Port $HostPort)) {
        exit 1
    }

    # Step 3: Check session authentication
    if (-not (Test-SessionAuthenticated -Port $HostPort -Token $UserToken)) {
        exit 1
    }

    # Step 3.5: Warn user if Tier 1 (Messaging) tests will run
    $tiers = $TiersToRun -split "," | ForEach-Object { $_.Trim() }
    if ($tiers -contains "1") {
        Write-Host ""
        Write-Host "-----------------------------------------------------------" -ForegroundColor Yellow
        Write-Host "  WARNING: Tier 1 (Messaging) Tests Will Send Real Messages" -ForegroundColor Yellow
        Write-Host "-----------------------------------------------------------" -ForegroundColor Yellow
        Write-Host ""
        Write-Warning "Tier 1 tests will send real WhatsApp messages to the configured phone number."
        Write-Host ""
        Write-Host "  Configured phone number: " -NoNewline -ForegroundColor White

        # Try to read phone number from appsettings
        $appsettingsLocal = Join-Path $repoRoot "tests\WuzApiClient.IntegrationTests\appsettings.Local.json"
        $appsettings = Join-Path $repoRoot "tests\WuzApiClient.IntegrationTests\appsettings.json"

        $phoneNumber = "Not configured"
        if (Test-Path $appsettingsLocal) {
            try {
                $config = Get-Content $appsettingsLocal -Raw | ConvertFrom-Json
                if ($config.TestData.TestPhoneNumber) {
                    $phoneNumber = $config.TestData.TestPhoneNumber
                }
            }
            catch {
                # Ignore JSON parsing errors
            }
        }
        elseif (Test-Path $appsettings) {
            try {
                $config = Get-Content $appsettings -Raw | ConvertFrom-Json
                if ($config.TestData.TestPhoneNumber) {
                    $phoneNumber = $config.TestData.TestPhoneNumber
                }
            }
            catch {
                # Ignore JSON parsing errors
            }
        }

        Write-Host $phoneNumber -ForegroundColor Cyan
        Write-Host ""

        if (-not (Test-Path $appsettingsLocal)) {
            Write-Warning "You haven't created 'appsettings.Local.json' yet."
            Write-Host "  To configure your personal phone number:" -ForegroundColor White
            Write-Host "  1. Copy 'appsettings.Local.json.template' to 'appsettings.Local.json'" -ForegroundColor Gray
            Write-Host "  2. Edit 'appsettings.Local.json' and set your phone number" -ForegroundColor Gray
            Write-Host ""
        }

        Write-Host "Press any key to continue or Ctrl+C to abort..." -ForegroundColor Yellow
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        Write-Host ""
    }

    # Step 4: Build tests
    if (-not (Invoke-BuildTests -RepoRoot $repoRoot)) {
        exit 1
    }

    # Step 5: Create results directory
    $resultsDir = New-TestResultsDirectory -RepoRoot $repoRoot -DirName $TestResultsDir
    Write-Status "Test results will be saved to: $resultsDir"

    # Step 6: Parse tiers to run
    $tiers = $TiersToRun -split "," | ForEach-Object { $_.Trim() }
    Write-Status "Running tiers: $($tiers -join ', ')"

    # Step 7: Run tests for each tier
    $testResults = @()

    foreach ($tier in $tiers) {
        $result = Invoke-TierTests `
            -RepoRoot $repoRoot `
            -Tier $tier `
            -ResultsDir $resultsDir `
            -Version $Version

        $testResults += $result

        # Stop on first failure if Tier 3 (destructive) is included
        if (-not $result.Passed -and $tier -eq "3") {
            Write-Warning "Tier 3 (Destructive) failed, stopping validation"
            break
        }
    }

    # Step 8: Show summary
    $allPassed = Show-TestSummary -Results $testResults -Version $Version -ResultsDir $resultsDir

    # Step 9: Exit with appropriate code
    if ($allPassed) {
        Write-Success "Validation completed successfully"
        exit 0
    }
    else {
        Write-ErrorMessage "Validation failed - some tests did not pass"
        exit 1
    }
}
catch {
    Write-Host ""
    Write-ErrorMessage "Validation failed with exception: $_"
    Write-Host $_.ScriptStackTrace
    exit 1
}

# Tony Bot - Run Development Server
# This script runs the Tony Bot in development mode

param(
    [string]$Environment = "Development",
    [switch]$Watch,
    [switch]$Build
)

Write-Host "Starting Tony Bot in $Environment mode..." -ForegroundColor Green

# Get the project path
$ProjectPath = Join-Path $PSScriptRoot "..\src\Tony\Tony.csproj"

if (-not (Test-Path $ProjectPath)) {
    Write-Error "Project file not found at: $ProjectPath"
    exit 1
}

try {
    # Build if requested
    if ($Build) {
        Write-Host "Building project..." -ForegroundColor Yellow
        dotnet build $ProjectPath --configuration Debug
        if ($LASTEXITCODE -ne 0) {
            throw "Build failed"
        }
    }

    # Set environment
    $env:ASPNETCORE_ENVIRONMENT = $Environment

    Write-Host "Project path: $ProjectPath" -ForegroundColor Gray
    Write-Host "Environment: $Environment" -ForegroundColor Gray
    Write-Host ""
    Write-Host "API Documentation will be available at: https://localhost:5001/scalar/v1" -ForegroundColor Cyan
    Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
    Write-Host ""

    # Run the application
    if ($Watch) {
        Write-Host "Running with file watching enabled..." -ForegroundColor Yellow
        dotnet watch run --project $ProjectPath
    } else {
        dotnet run --project $ProjectPath
    }
}
catch {
    Write-Error "Failed to start application: $($_.Exception.Message)"
    exit 1
}

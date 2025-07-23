# Tony Bot - Development Setup Script
# This script sets up the development environment

param(
    [switch]$Force
)

Write-Host "Setting up Tony Bot development environment..." -ForegroundColor Green

# Get paths
$SolutionPath = Join-Path $PSScriptRoot "..\src\Tony\Tony.sln"
$ProjectPath = Join-Path $PSScriptRoot "..\src\Tony\Tony.csproj"

try {
    # Check if .NET 10.0 SDK is installed
    Write-Host "Checking .NET SDK version..." -ForegroundColor Yellow
    $dotnetVersion = dotnet --version
    if ($dotnetVersion -match "^10\.") {
        Write-Host "✓ .NET 10.0 preview SDK is installed: $dotnetVersion" -ForegroundColor Green
    } else {
        Write-Warning "⚠ .NET 10.0 preview SDK not detected. Current version: $dotnetVersion"
        Write-Host "Please install .NET 10.0 preview SDK from: https://dotnet.microsoft.com/download/dotnet/10.0" -ForegroundColor Yellow
    }

    # Restore packages
    Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
    dotnet restore $SolutionPath
    if ($LASTEXITCODE -ne 0) {
        throw "Package restore failed"
    }

    # Initialize user secrets
    Write-Host "Initializing user secrets..." -ForegroundColor Yellow
    dotnet user-secrets init --project $ProjectPath
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "User secrets initialization failed or already exists"
    }

    # Check if MongoDB is accessible (optional)
    Write-Host "Checking MongoDB connection..." -ForegroundColor Yellow
    try {
        $mongoTest = Test-NetConnection -ComputerName "localhost" -Port 27017 -InformationLevel Quiet
        if ($mongoTest) {
            Write-Host "✓ MongoDB appears to be running on localhost:27017" -ForegroundColor Green
        } else {
            Write-Warning "⚠ MongoDB not detected on localhost:27017"
            Write-Host "Make sure MongoDB is installed and running, or configure a connection string" -ForegroundColor Yellow
        }
    }
    catch {
        Write-Warning "Could not test MongoDB connection"
    }

    # Create sample appsettings if they don't exist
    $appSettingsPath = Join-Path $PSScriptRoot "..\src\Tony\appsettings.Development.json"
    if (-not (Test-Path $appSettingsPath) -or $Force) {
        Write-Host "Creating sample appsettings.Development.json..." -ForegroundColor Yellow
        $sampleSettings = @{
            "Logging" = @{
                "LogLevel" = @{
                    "Default" = "Information"
                    "Microsoft.AspNetCore" = "Warning"
                }
            }
            "MongoDB" = @{
                "ConnectionString" = "mongodb://localhost:27017"
                "DatabaseName" = "TonyBot"
            }
            "Discord" = @{
                "Token" = "YOUR_DISCORD_BOT_TOKEN_HERE"
                "GuildId" = "YOUR_GUILD_ID_HERE"
            }
        }
        $sampleSettings | ConvertTo-Json -Depth 10 | Out-File -FilePath $appSettingsPath -Encoding UTF8
        Write-Host "✓ Sample configuration created at: $appSettingsPath" -ForegroundColor Green
        Write-Host "Please update the Discord token and other settings as needed" -ForegroundColor Yellow
    }

    Write-Host "Development environment setup completed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "1. Configure your Discord bot token in user secrets or appsettings" -ForegroundColor White
    Write-Host "2. Ensure MongoDB is running" -ForegroundColor White
    Write-Host "3. Run the application with: dotnet run --project src/Tony/Tony.csproj" -ForegroundColor White
    Write-Host "4. Access API documentation at: https://localhost:5001/scalar/v1" -ForegroundColor White
}
catch {
    Write-Error "Setup failed: $($_.Exception.Message)"
    exit 1
}

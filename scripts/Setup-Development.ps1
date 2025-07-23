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

    # Configure user secrets with default values if they don't exist
    Write-Host "Configuring user secrets..." -ForegroundColor Yellow
    $secrets = @{
        "Discord:Token" = "YOUR_DISCORD_BOT_TOKEN_HERE"
        "Discord:GuildId" = "YOUR_GUILD_ID_HERE"
        "MongoDB:ConnectionString" = "mongodb://localhost:27017"
        "MongoDB:DatabaseName" = "TonyBot"
    }

    foreach ($secret in $secrets.GetEnumerator()) {
        $existingValue = dotnet user-secrets get $secret.Key --project $ProjectPath 2>$null
        if (-not $existingValue -or $Force) {
            dotnet user-secrets set $secret.Key $secret.Value --project $ProjectPath | Out-Null
            if ($secret.Value -like "*YOUR_*") {
                Write-Host "⚠ Set placeholder for $($secret.Key) - please update with actual value" -ForegroundColor Yellow
            } else {
                Write-Host "✓ Set $($secret.Key)" -ForegroundColor Green
            }
        } else {
            Write-Host "✓ $($secret.Key) already configured" -ForegroundColor Green
        }
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
    Write-Host "1. Update your Discord bot token in user secrets:" -ForegroundColor White
    Write-Host "   dotnet user-secrets set 'Discord:Token' 'your-actual-bot-token'" -ForegroundColor Gray
    Write-Host "2. Update your Discord guild ID in user secrets:" -ForegroundColor White
    Write-Host "   dotnet user-secrets set 'Discord:GuildId' 'your-guild-id'" -ForegroundColor Gray
    Write-Host "3. Ensure MongoDB is running on localhost:27017" -ForegroundColor White
    Write-Host "4. Run the application with: dotnet run --project src/Tony/Tony.csproj" -ForegroundColor White
    Write-Host "5. Access API documentation at: https://localhost:5001/scalar/v1" -ForegroundColor White
    Write-Host "6. Check health status at: https://localhost:5001/health" -ForegroundColor White
    Write-Host ""
    Write-Host "Useful commands:" -ForegroundColor Cyan
    Write-Host "• List user secrets: dotnet user-secrets list --project src/Tony/Tony.csproj" -ForegroundColor Gray
    Write-Host "• Clear user secrets: dotnet user-secrets clear --project src/Tony/Tony.csproj" -ForegroundColor Gray
}
catch {
    Write-Error "Setup failed: $($_.Exception.Message)"
    exit 1
}

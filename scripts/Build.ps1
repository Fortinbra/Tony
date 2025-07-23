# Tony Bot - Build Script
# This script builds the entire solution in Release configuration

param(
    [string]$Configuration = "Release",
    [switch]$Clean,
    [switch]$Restore
)

Write-Host "Starting build process for Tony Bot..." -ForegroundColor Green

# Get the solution path
$SolutionPath = Join-Path $PSScriptRoot "..\Tony.sln"

if (-not (Test-Path $SolutionPath)) {
    Write-Error "Solution file not found at: $SolutionPath"
    exit 1
}

try {
    # Clean if requested
    if ($Clean) {
        Write-Host "Cleaning solution..." -ForegroundColor Yellow
        dotnet clean $SolutionPath --configuration $Configuration
        if ($LASTEXITCODE -ne 0) {
            throw "Clean failed"
        }
    }

    # Restore packages if requested
    if ($Restore) {
        Write-Host "Restoring packages..." -ForegroundColor Yellow
        dotnet restore $SolutionPath
        if ($LASTEXITCODE -ne 0) {
            throw "Package restore failed"
        }
    }

    # Build the solution
    Write-Host "Building solution in $Configuration configuration..." -ForegroundColor Yellow
    Write-Host "Note: All projects treat warnings as errors - build will fail on any warnings" -ForegroundColor Gray
    dotnet build $SolutionPath --configuration $Configuration --no-restore:$(-not $Restore)
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Build completed successfully!" -ForegroundColor Green
    } else {
        throw "Build failed"
    }
}
catch {
    Write-Error "Build process failed: $($_.Exception.Message)"
    exit 1
}

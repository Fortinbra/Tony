# Run-Tests.ps1
# PowerShell script to run all tests with coverage reporting

param(
    [string]$Configuration = "Debug",
    [switch]$Coverage = $false,
    [switch]$Verbose = $false,
    [switch]$Watch = $false,
    [ValidateSet("All", "Unit", "Integration", "E2E", "Performance", "Smoke")]
    [string]$Category = "All"
)

Write-Host "🧪 Tony Bot Test Runner" -ForegroundColor Cyan
Write-Host "======================" -ForegroundColor Cyan

# Check if .NET is installed
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET SDK Version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ .NET SDK not found. Please install .NET 10 or later." -ForegroundColor Red
    exit 1
}

# Display test category info
if ($Category -ne "All") {
    Write-Host "🏷️  Running $Category tests only" -ForegroundColor Magenta
} else {
    Write-Host "🏷️  Running all test categories" -ForegroundColor Magenta
}

# Restore packages
Write-Host "`n📦 Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore Tony.sln
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Package restore failed" -ForegroundColor Red
    exit 1
}

# Build solution
Write-Host "`n🔨 Building solution..." -ForegroundColor Yellow
$buildArgs = @("build", "Tony.sln", "--configuration", $Configuration, "--no-restore")
if ($Verbose) { $buildArgs += "--verbosity", "normal" }

dotnet @buildArgs
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed" -ForegroundColor Red
    exit 1
}

# Prepare test command
$testArgs = @(
    "test", "Tony.sln",
    "--configuration", $Configuration,
    "--no-build",
    "--logger", "console;verbosity=normal"
)

# Select appropriate run settings file based on category
$runSettingsFile = switch ($Category) {
    "Unit" { "tests.unit.runsettings" }
    "Integration" { "tests.integration.runsettings" }
    "E2E" { "tests.e2e.runsettings" }
    default { "tests.runsettings" }
}

if (Test-Path $runSettingsFile) {
    $testArgs += "--settings", $runSettingsFile
} else {
    Write-Host "⚠️  Run settings file '$runSettingsFile' not found, using default settings" -ForegroundColor Yellow
}

# Add filter for specific test categories (when not using dedicated run settings)
if ($Category -ne "All" -and -not (Test-Path $runSettingsFile)) {
    $testArgs += "--filter", "Category=$Category"
}

if ($Verbose) {
    $testArgs += "--verbosity", "normal"
}

if ($Watch) {
    $testArgs += "--watch"
}

if ($Coverage) {
    Write-Host "`n🧪 Running tests with coverage..." -ForegroundColor Yellow
    
    # Ensure results directory exists
    if (!(Test-Path "TestResults")) {
        New-Item -ItemType Directory -Path "TestResults" | Out-Null
    }
    
    $testArgs += @(
        "--results-directory", "TestResults",
        "--collect:XPlat Code Coverage"
    )
    
    # Run tests
    dotnet @testArgs
    $testResult = $LASTEXITCODE
    
    if ($testResult -eq 0) {
        Write-Host "✅ All tests passed!" -ForegroundColor Green
        
        # Generate coverage report if reportgenerator is available
        try {
            $coverageFiles = Get-ChildItem -Path "TestResults" -Filter "coverage.cobertura.xml" -Recurse
            if ($coverageFiles.Count -gt 0) {
                Write-Host "`n📊 Generating coverage report..." -ForegroundColor Yellow
                
                if (!(Test-Path "CodeCoverage")) {
                    New-Item -ItemType Directory -Path "CodeCoverage" | Out-Null
                }
                
                $reportFiles = $coverageFiles | ForEach-Object { $_.FullName }
                dotnet tool run reportgenerator -reports:($reportFiles -join ';') -targetdir:CodeCoverage -reporttypes:"Html;Cobertura;TextSummary"
                
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "✅ Coverage report generated in ./CodeCoverage/" -ForegroundColor Green
                    
                    # Display coverage summary
                    $summaryFile = "CodeCoverage\Summary.txt"
                    if (Test-Path $summaryFile) {
                        Write-Host "`n📈 Coverage Summary:" -ForegroundColor Cyan
                        Get-Content $summaryFile | Where-Object { $_ -match "Line coverage|Branch coverage" } | ForEach-Object {
                            Write-Host "   $_" -ForegroundColor White
                        }
                    }
                    
                    # Open coverage report
                    $htmlReport = "CodeCoverage\index.html"
                    if (Test-Path $htmlReport) {
                        Write-Host "`n🌐 Opening coverage report..." -ForegroundColor Yellow
                        Start-Process $htmlReport
                    }
                } else {
                    Write-Host "⚠️  Coverage report generation failed" -ForegroundColor Yellow
                }
            } else {
                Write-Host "⚠️  No coverage files found" -ForegroundColor Yellow
            }
        } catch {
            Write-Host "⚠️  ReportGenerator not available. Install with: dotnet tool install -g dotnet-reportgenerator-globaltool" -ForegroundColor Yellow
        }
    } else {
        Write-Host "❌ Some tests failed" -ForegroundColor Red
    }
} else {
    Write-Host "`n🧪 Running tests..." -ForegroundColor Yellow
    
    # Run tests without coverage
    dotnet @testArgs
    $testResult = $LASTEXITCODE
    
    if ($testResult -eq 0) {
        Write-Host "✅ All tests passed!" -ForegroundColor Green
    } else {
        Write-Host "❌ Some tests failed" -ForegroundColor Red
    }
}

# Display test project information
Write-Host "`n📋 Test Projects:" -ForegroundColor Cyan
Get-ChildItem -Path "tests" -Filter "*.csproj" -Recurse | ForEach-Object {
    $projectName = $_.Directory.Name
    Write-Host "   • $projectName" -ForegroundColor White
}

Write-Host "`n�️  Test Categories:" -ForegroundColor Cyan
Write-Host "   • Unit - Fast running isolated component tests" -ForegroundColor White
Write-Host "   • Integration - Multi-component interaction tests" -ForegroundColor White  
Write-Host "   • E2E - End-to-end system tests" -ForegroundColor White
Write-Host "   • Performance - Performance measurement tests" -ForegroundColor White
Write-Host "   • Smoke - Basic functionality verification tests" -ForegroundColor White

Write-Host "`n💡 Usage Examples:" -ForegroundColor Cyan
Write-Host "   .\Run-Tests.ps1 -Category Unit" -ForegroundColor White
Write-Host "   .\Run-Tests.ps1 -Category Integration -Coverage" -ForegroundColor White
Write-Host "   .\Run-Tests.ps1 -Category E2E -Verbose" -ForegroundColor White

Write-Host "`n�🏁 Testing completed!" -ForegroundColor Cyan

exit $testResult

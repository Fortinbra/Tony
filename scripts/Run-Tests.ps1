# Run-Tests.ps1
# PowerShell script to run all tests with coverage reporting

param(
    [string]$Configuration = "Debug",
    [switch]$Coverage = $false,
    [switch]$Verbose = $false,
    [switch]$Watch = $false
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
        "--collect:XPlat Code Coverage",
        "--settings", "tests.runsettings"
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

Write-Host "`n🏁 Testing completed!" -ForegroundColor Cyan

exit $testResult

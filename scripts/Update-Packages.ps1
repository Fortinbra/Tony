# Tony Bot - Package Update Script
# This script updates all NuGet packages to their latest versions

param(
    [switch]$Preview,
    [switch]$DryRun
)

Write-Host "Tony Bot Package Update Script" -ForegroundColor Green

# Get all project files
$ProjectFiles = Get-ChildItem -Path (Join-Path $PSScriptRoot "..\src") -Recurse -Filter "*.csproj"

if ($ProjectFiles.Count -eq 0) {
    Write-Error "No project files found"
    exit 1
}

Write-Host "Found $($ProjectFiles.Count) project file(s):" -ForegroundColor Yellow
foreach ($project in $ProjectFiles) {
    Write-Host "  - $($project.FullName)" -ForegroundColor Gray
}
Write-Host ""

try {
    foreach ($project in $ProjectFiles) {
        Write-Host "Processing: $($project.Name)" -ForegroundColor Cyan
        
        if ($DryRun) {
            Write-Host "  [DRY RUN] Would update packages for: $($project.FullName)" -ForegroundColor Yellow
            continue
        }

        # List outdated packages
        Write-Host "  Checking for outdated packages..." -ForegroundColor Gray
        $listCommand = "dotnet list `"$($project.FullName)`" package --outdated"
        if ($Preview) {
            $listCommand += " --include-prerelease"
        }
        
        Invoke-Expression $listCommand

        # Update packages
        Write-Host "  Updating packages..." -ForegroundColor Gray
        $updateCommand = "dotnet add `"$($project.FullName)`" package --no-restore"
        
        # Get package references from project file
        $projectContent = Get-Content $project.FullName
        $packageReferences = $projectContent | Select-String "PackageReference Include" | ForEach-Object {
            if ($_ -match 'Include="([^"]+)"') {
                $matches[1]
            }
        }

        foreach ($package in $packageReferences) {
            Write-Host "    Updating $package..." -ForegroundColor DarkGray
            $cmd = "dotnet add `"$($project.FullName)`" package $package"
            if ($Preview) {
                $cmd += " --prerelease"
            }
            
            try {
                Invoke-Expression $cmd
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "    ✓ $package updated" -ForegroundColor Green
                } else {
                    Write-Warning "    ⚠ Failed to update $package"
                }
            }
            catch {
                Write-Warning "    ⚠ Error updating $package: $($_.Exception.Message)"
            }
        }
        
        Write-Host ""
    }

    if (-not $DryRun) {
        # Restore packages after updates
        Write-Host "Restoring packages..." -ForegroundColor Yellow
        $solutionPath = Join-Path $PSScriptRoot "..\Tony.sln"
        dotnet restore $solutionPath
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Package updates completed successfully!" -ForegroundColor Green
            Write-Host ""
            Write-Host "Next steps:" -ForegroundColor Cyan
            Write-Host "1. Test the application to ensure compatibility" -ForegroundColor White
            Write-Host "2. Update any breaking changes if necessary" -ForegroundColor White
            Write-Host "3. Commit the changes to version control" -ForegroundColor White
        } else {
            Write-Error "Package restore failed after updates"
        }
    } else {
        Write-Host "✓ Dry run completed - no changes made" -ForegroundColor Green
    }
}
catch {
    Write-Error "Package update failed: $($_.Exception.Message)"
    exit 1
}

# PowerShell deployment script for Tony Bot
# Run this on Windows to prepare and deploy to Raspberry Pi

param(
    [string]$PiAddress,
    [string]$PiUser = "pi",
    [switch]$Build,
    [switch]$Deploy,
    [switch]$Help
)

function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Color
}

function Show-Help {
    Write-ColorOutput "Tony Bot Docker Deployment Script" "Cyan"
    Write-ColorOutput "====================================" "Cyan"
    Write-ColorOutput ""
    Write-ColorOutput "Usage:" "Yellow"
    Write-ColorOutput "  .\Deploy-ToPi.ps1 -PiAddress <ip> -PiUser <user> [-Build] [-Deploy]" "White"
    Write-ColorOutput ""
    Write-ColorOutput "Parameters:" "Yellow"
    Write-ColorOutput "  -PiAddress  : IP address of your Raspberry Pi" "White"
    Write-ColorOutput "  -PiUser     : SSH username (default: pi)" "White"
    Write-ColorOutput "  -Build      : Build the Docker image locally first" "White"
    Write-ColorOutput "  -Deploy     : Deploy to Raspberry Pi" "White"
    Write-ColorOutput "  -Help       : Show this help message" "White"
    Write-ColorOutput ""
    Write-ColorOutput "Examples:" "Yellow"
    Write-ColorOutput "  .\Deploy-ToPi.ps1 -PiAddress 192.168.1.100 -Build -Deploy" "White"
    Write-ColorOutput "  .\Deploy-ToPi.ps1 -PiAddress 192.168.1.100 -PiUser tony -Deploy" "White"
}

if ($Help) {
    Show-Help
    exit 0
}

if (-not $PiAddress) {
    Write-ColorOutput "❌ Error: Pi address is required" "Red"
    Show-Help
    exit 1
}

# Check if required tools are installed
$requiredTools = @("docker", "ssh", "scp")
foreach ($tool in $requiredTools) {
    if (-not (Get-Command $tool -ErrorAction SilentlyContinue)) {
        Write-ColorOutput "❌ Error: $tool is not installed or not in PATH" "Red"
        exit 1
    }
}

Write-ColorOutput "🚀 Tony Bot Deployment to Raspberry Pi" "Cyan"
Write-ColorOutput "Target: $PiUser@$PiAddress" "Green"

if ($Build) {
    Write-ColorOutput "🔨 Building Docker image locally..." "Yellow"
    
    # Test build locally (optional, to catch build errors early)
    try {
        docker build -t tony-bot-test .
        Write-ColorOutput "✅ Local build successful" "Green"
        docker rmi tony-bot-test -f
    }
    catch {
        Write-ColorOutput "❌ Local build failed: $_" "Red"
        exit 1
    }
}

if ($Deploy) {
    Write-ColorOutput "📦 Preparing deployment package..." "Yellow"
    
    # Create deployment package
    $tempDir = Join-Path $env:TEMP "tony-bot-deploy"
    if (Test-Path $tempDir) {
        Remove-Item $tempDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $tempDir | Out-Null
    
    # Copy necessary files
    $filesToCopy = @(
        "docker-compose.yml",
        "docker-compose.dev.yml",
        "Dockerfile",
        ".dockerignore",
        ".env.example",
        "src",
        "deployment"
    )
    
    foreach ($file in $filesToCopy) {
        if (Test-Path $file) {
            Write-ColorOutput "  Copying $file..." "Gray"
            if (Test-Path $file -PathType Container) {
                Copy-Item $file -Destination $tempDir -Recurse
            } else {
                Copy-Item $file -Destination $tempDir
            }
        }
    }
    
    Write-ColorOutput "📤 Uploading to Raspberry Pi..." "Yellow"
    
    # Create directory on Pi
    ssh "$PiUser@$PiAddress" "mkdir -p ~/tony-bot"
    
    # Upload files
    scp -r "$tempDir/*" "$PiUser@$PiAddress`:~/tony-bot/"
    
    Write-ColorOutput "🎯 Deploying on Raspberry Pi..." "Yellow"
    
    # Run deployment on Pi
    $deployCommand = @"
cd ~/tony-bot && \
chmod +x deployment/*.sh && \
echo '🔧 Checking for .env file...' && \
if [ ! -f .env ]; then 
    echo '⚠️  Creating .env from example - please configure it!'; 
    cp .env.example .env; 
    echo '📝 Please edit .env file with your Discord token and other settings:'; 
    echo '   nano .env'; 
    echo 'Then run: ./deployment/deploy.sh'; 
else 
    echo '✅ .env file exists, proceeding with deployment...'; 
    ./deployment/deploy.sh; 
fi
"@
    
    ssh "$PiUser@$PiAddress" $deployCommand
    
    # Cleanup
    Remove-Item $tempDir -Recurse -Force
    
    Write-ColorOutput "✅ Deployment completed!" "Green"
    Write-ColorOutput "🌐 Your bot should be available at: http://$PiAddress:8080" "Cyan"
    Write-ColorOutput "📊 Check status with: ssh $PiUser@$PiAddress 'cd ~/tony-bot && docker-compose ps'" "Gray"
}

if (-not $Build -and -not $Deploy) {
    Write-ColorOutput "⚠️  No action specified. Use -Build and/or -Deploy flags." "Yellow"
    Show-Help
}

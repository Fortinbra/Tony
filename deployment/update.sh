#!/bin/bash
# Update script for Tony Bot on Raspberry Pi

set -e

echo "🔄 Updating Tony Bot..."

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

# Pull latest code (if using git)
if [ -d ".git" ]; then
    print_status "Pulling latest code from git..."
    git pull
fi

# Rebuild and restart containers
print_status "Rebuilding and restarting containers..."
docker-compose down
docker-compose up -d --build

# Wait for health check
print_status "Waiting for services to be healthy..."
sleep 30

# Check status
docker-compose ps

print_success "Tony Bot updated successfully!"

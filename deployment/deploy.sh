#!/bin/bash
# Deployment script for Tony Bot on Raspberry Pi
# This script should be run on the Raspberry Pi

set -e

echo "🚀 Starting Tony Bot deployment on Raspberry Pi..."

# Configuration
PROJECT_NAME="tony-bot"
DOCKER_COMPOSE_FILE="docker-compose.yml"
ENV_FILE=".env"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    print_error "Docker is not installed. Please install Docker first."
    echo "Run: curl -fsSL https://get.docker.com -o get-docker.sh && sh get-docker.sh"
    exit 1
fi

# Check if Docker Compose is installed
if ! command -v docker-compose &> /dev/null; then
    print_error "Docker Compose is not installed. Please install Docker Compose first."
    echo "Run: sudo apt-get update && sudo apt-get install docker-compose-plugin"
    exit 1
fi

# Check if .env file exists
if [ ! -f "$ENV_FILE" ]; then
    print_error ".env file not found!"
    print_warning "Please copy .env.example to .env and configure your settings:"
    echo "cp .env.example .env"
    echo "nano .env"
    exit 1
fi

# Validate required environment variables
print_status "Validating environment configuration..."
source $ENV_FILE

if [ -z "$DISCORD_TOKEN" ] || [ "$DISCORD_TOKEN" = "your_discord_bot_token_here" ]; then
    print_error "DISCORD_TOKEN is not configured in .env file"
    exit 1
fi

if [ -z "$DISCORD_GUILD_ID" ] || [ "$DISCORD_GUILD_ID" = "your_discord_guild_id_here" ]; then
    print_error "DISCORD_GUILD_ID is not configured in .env file"
    exit 1
fi

if [ -z "$MONGO_ROOT_PASSWORD" ] || [ "$MONGO_ROOT_PASSWORD" = "your_secure_mongo_password_here" ]; then
    print_error "MONGO_ROOT_PASSWORD is not configured in .env file"
    exit 1
fi

print_success "Environment configuration validated"

# Create necessary directories
print_status "Creating directories..."
mkdir -p logs
mkdir -p mongodb-init

# Stop existing containers
print_status "Stopping existing containers..."
docker-compose -f $DOCKER_COMPOSE_FILE down --remove-orphans

# Pull latest images (if using pre-built images)
print_status "Pulling latest images..."
docker-compose -f $DOCKER_COMPOSE_FILE pull --ignore-buildable

# Build and start containers
print_status "Building and starting Tony Bot..."
docker-compose -f $DOCKER_COMPOSE_FILE up -d --build

# Wait for services to be healthy
print_status "Waiting for services to be healthy..."
sleep 30

# Check health status
print_status "Checking service health..."
if docker-compose -f $DOCKER_COMPOSE_FILE ps | grep -q "Up (healthy)"; then
    print_success "Tony Bot is running and healthy!"
else
    print_warning "Services may still be starting up. Check logs with:"
    echo "docker-compose -f $DOCKER_COMPOSE_FILE logs -f"
fi

# Display service status
print_status "Service status:"
docker-compose -f $DOCKER_COMPOSE_FILE ps

# Display useful commands
print_success "Deployment completed!"
echo ""
echo "Useful commands:"
echo "📊 View logs: docker-compose -f $DOCKER_COMPOSE_FILE logs -f"
echo "🔄 Restart: docker-compose -f $DOCKER_COMPOSE_FILE restart"
echo "⏹️  Stop: docker-compose -f $DOCKER_COMPOSE_FILE down"
echo "📈 Status: docker-compose -f $DOCKER_COMPOSE_FILE ps"
echo "🏥 Health: curl http://localhost:8080/health"
echo ""
echo "🌐 Tony Bot API will be available at: http://$(hostname -I | awk '{print $1}'):8080"
echo "📖 API Documentation: http://$(hostname -I | awk '{print $1}'):8080/scalar/v1"

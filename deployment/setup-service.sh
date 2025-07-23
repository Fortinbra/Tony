#!/bin/bash
# Setup script for Tony Bot systemd service on Raspberry Pi

set -e

echo "🔧 Setting up Tony Bot as a system service..."

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
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

# Check if running as root
if [ "$EUID" -eq 0 ]; then
    print_warning "Please don't run this script as root. Run as your regular user."
    exit 1
fi

# Ensure we're in the correct directory
if [ ! -f "docker-compose.yml" ]; then
    echo "Error: docker-compose.yml not found. Please run this script from the tony-bot directory."
    exit 1
fi

print_status "Setting up systemd service..."

# Update the service file with the correct user and path
SERVICE_FILE="deployment/tony-bot.service"
CURRENT_USER=$(whoami)
CURRENT_DIR=$(pwd)

# Create a temporary service file with correct paths
cat > /tmp/tony-bot.service << EOF
[Unit]
Description=Tony Discord Bot
Documentation=https://github.com/Fortinbra/Tony
After=docker.service
Requires=docker.service

[Service]
Type=oneshot
RemainAfterExit=yes
User=$CURRENT_USER
Group=$CURRENT_USER
WorkingDirectory=$CURRENT_DIR
ExecStart=/usr/bin/docker-compose up -d
ExecStop=/usr/bin/docker-compose down
TimeoutStartSec=0
Environment=PATH=/usr/bin:/bin:/usr/local/bin

[Install]
WantedBy=multi-user.target
EOF

# Install the service
print_status "Installing systemd service..."
sudo cp /tmp/tony-bot.service /etc/systemd/system/
sudo systemctl daemon-reload

# Enable the service
print_status "Enabling Tony Bot service..."
sudo systemctl enable tony-bot.service

print_success "Tony Bot service installed and enabled!"

echo ""
echo "Service management commands:"
echo "  Start:   sudo systemctl start tony-bot"
echo "  Stop:    sudo systemctl stop tony-bot"
echo "  Status:  sudo systemctl status tony-bot"
echo "  Logs:    journalctl -u tony-bot -f"
echo ""
echo "The service will automatically start on boot."

# Ask if user wants to start the service now
read -p "Start the Tony Bot service now? (y/N): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    print_status "Starting Tony Bot service..."
    sudo systemctl start tony-bot
    
    # Check status
    sleep 5
    if systemctl is-active --quiet tony-bot; then
        print_success "Tony Bot service started successfully!"
    else
        print_warning "Service may still be starting. Check status with: sudo systemctl status tony-bot"
    fi
fi

# Cleanup
rm -f /tmp/tony-bot.service

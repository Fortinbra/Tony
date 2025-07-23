# Tony Bot - Docker Deployment Guide for Raspberry Pi

This guide will help you deploy the Tony Bot application as a Docker container on a Raspberry Pi running Raspberry Pi OS (Debian-based).

## Prerequisites

### Hardware Requirements
- Raspberry Pi 4 (recommended) or Raspberry Pi 3B+ with at least 2GB RAM
- MicroSD card (32GB+ recommended)
- Stable internet connection

### Software Requirements
- Raspberry Pi OS (64-bit recommended for better .NET 10 support)
- Docker and Docker Compose
- Git (for cloning/updating the repository)

## Initial Setup on Raspberry Pi

### 1. Update Your Raspberry Pi
```bash
sudo apt update && sudo apt upgrade -y
```

### 2. Install Docker
```bash
# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sh get-docker.sh

# Add your user to the docker group
sudo usermod -aG docker $USER

# Log out and log back in, or run:
newgrp docker
```

### 3. Install Docker Compose
```bash
sudo apt-get update
sudo apt-get install docker-compose-plugin -y
```

### 4. Verify Installation
```bash
docker --version
docker-compose --version
```

## Deployment Steps

### 1. Clone the Repository
```bash
git clone <your-repository-url>
cd Tony
```

### 2. Configure Environment Variables
```bash
# Copy the example environment file
cp .env.example .env

# Edit the environment file with your settings
nano .env
```

### Required Configuration in `.env`:
```bash
# Discord Bot Configuration
DISCORD_TOKEN=your_discord_bot_token_here
DISCORD_GUILD_ID=your_discord_guild_id_here

# MongoDB Configuration
MONGO_ROOT_PASSWORD=your_secure_mongo_password_here
MONGODB_DATABASE_NAME=TonyBot
```

### 3. Deploy the Application
```bash
# Make the deployment script executable
chmod +x deployment/deploy.sh

# Run the deployment
./deployment/deploy.sh
```

## Service Management

### Start Services
```bash
docker-compose up -d
```

### Stop Services
```bash
docker-compose down
```

### View Logs
```bash
# View all logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f tony-bot
docker-compose logs -f mongodb
```

### Check Service Status
```bash
docker-compose ps
```

### Update Application
```bash
# Make the update script executable
chmod +x deployment/update.sh

# Run the update
./deployment/update.sh
```

## Health Monitoring

### Health Check Endpoint
The application provides a health check endpoint at:
```
http://your-pi-ip:8080/health
```

### API Documentation
Access the API documentation at:
```
http://your-pi-ip:8080/scalar/v1
```

## File Structure After Deployment

```
Tony/
├── docker-compose.yml          # Production configuration
├── docker-compose.dev.yml      # Development configuration
├── Dockerfile                  # Multi-stage build for ARM64
├── .env                       # Environment variables (create from .env.example)
├── .env.example               # Template for environment variables
├── .dockerignore              # Files to ignore during Docker build
├── deployment/
│   ├── deploy.sh              # Deployment script
│   └── update.sh              # Update script
├── logs/                      # Application logs (created automatically)
└── src/                       # Source code
```

## Troubleshooting

### Common Issues

1. **Out of Memory**: Ensure your Raspberry Pi has sufficient RAM. Consider adding swap space:
   ```bash
   sudo dphys-swapfile swapoff
   sudo nano /etc/dphys-swapfile  # Increase CONF_SWAPSIZE to 2048
   sudo dphys-swapfile setup
   sudo dphys-swapfile swapon
   ```

2. **Build Timeouts**: The initial build may take a while on Raspberry Pi. Be patient during the first deployment.

3. **Permission Issues**: Ensure your user is in the docker group:
   ```bash
   groups $USER  # Should show 'docker' in the list
   ```

### Viewing Detailed Logs
```bash
# View real-time logs
docker-compose logs -f tony-bot

# View MongoDB logs
docker-compose logs -f mongodb

# View last 50 lines
docker-compose logs --tail=50 tony-bot
```

### Container Management
```bash
# Restart a specific service
docker-compose restart tony-bot

# Rebuild a specific service
docker-compose up -d --build tony-bot

# Access container shell (for debugging)
docker-compose exec tony-bot sh
```

## Performance Optimization for Raspberry Pi

### Memory Management
- The application is configured with minimal memory footprint
- MongoDB is configured with appropriate memory limits for Pi
- Health checks are optimized to not overload the system

### Storage
- Use a high-quality MicroSD card (Class 10 or better)
- Consider using an external SSD for better I/O performance
- Logs are stored in a volume to prevent filling up the container

## Security Considerations

1. **Change Default Passwords**: Always change the default MongoDB password
2. **Firewall**: Configure iptables or ufw to restrict access to necessary ports only
3. **Updates**: Regularly update the Raspberry Pi OS and Docker images
4. **Environment Variables**: Never commit `.env` files to version control

## Backup Strategy

### Database Backup
```bash
# Create a backup of MongoDB data
docker-compose exec mongodb mongodump --db TonyBot --out /data/backup

# Copy backup from container
docker cp tony-mongodb:/data/backup ./mongodb-backup
```

### Full System Backup
```bash
# Stop services
docker-compose down

# Backup entire project directory
tar -czf tony-bot-backup-$(date +%Y%m%d).tar.gz .

# Restart services
docker-compose up -d
```

## Monitoring and Maintenance

### Resource Monitoring
```bash
# View container resource usage
docker stats

# View system resources
htop
```

### Log Rotation
The application logs are automatically rotated. Monitor disk usage:
```bash
df -h
du -sh logs/
```

### Automatic Updates (Optional)
Consider setting up automatic updates using cron:
```bash
# Edit crontab
crontab -e

# Add line for weekly updates (runs every Sunday at 2 AM)
0 2 * * 0 cd /path/to/Tony && ./deployment/update.sh >> ./logs/update.log 2>&1
```

## Development Mode

For development purposes, use the development compose file:
```bash
# Start in development mode
docker-compose -f docker-compose.dev.yml up -d

# Access development API at http://your-pi-ip:5000
```

This deployment setup provides a robust, production-ready environment for running Tony Bot on a Raspberry Pi with proper health monitoring, logging, and easy management capabilities.

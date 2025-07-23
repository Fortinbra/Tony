# Docker Deployment Quick Start

This guide provides a quick setup for deploying Tony Bot to your Raspberry Pi using Docker.

## 🚀 Quick Deployment (from Windows)

### Prerequisites
- Docker Desktop installed on Windows
- SSH access to your Raspberry Pi
- PowerShell 5.1 or later

### Steps

1. **Configure your bot settings**:
   ```powershell
   # Copy the environment template
   Copy-Item .env.example .env
   
   # Edit with your settings (Discord token, etc.)
   notepad .env
   ```

2. **Deploy to your Raspberry Pi**:
   ```powershell
   .\deployment\Deploy-ToPi.ps1 -PiAddress 192.168.1.100 -Build -Deploy
   ```

   Replace `192.168.1.100` with your Raspberry Pi's IP address.

## 🔧 Manual Deployment (on Raspberry Pi)

If you prefer to deploy directly on the Pi:

1. **Copy files to your Pi**:
   ```bash
   scp -r . pi@192.168.1.100:~/tony-bot/
   ```

2. **SSH to your Pi and deploy**:
   ```bash
   ssh pi@192.168.1.100
   cd ~/tony-bot
   chmod +x deployment/*.sh
   ./deployment/deploy.sh
   ```

## 📝 Configuration

The `.env` file contains all necessary configuration:

```bash
# Required: Discord Bot Settings
DISCORD_TOKEN=your_discord_bot_token_here
DISCORD_GUILD_ID=your_discord_guild_id_here

# Required: Database Security
MONGO_ROOT_PASSWORD=your_secure_password_here

# Optional: Database Settings
MONGODB_DATABASE_NAME=TonyBot
MONGODB_CONNECTION_STRING=mongodb://admin:your_secure_password_here@mongodb:27017/TonyBot?authSource=admin
```

## 🏥 Health Checks

After deployment, verify everything is working:

```bash
# Check if containers are running
docker-compose ps

# Test the health endpoint
curl http://localhost:8080/health

# View logs
docker-compose logs -f tony-bot
```

## 🔄 Updates

To update your deployment:

```bash
# On Raspberry Pi
cd ~/tony-bot
./deployment/update.sh
```

Or from Windows:
```powershell
.\deployment\Deploy-ToPi.ps1 -PiAddress 192.168.1.100 -Deploy
```

## 🛠️ Troubleshooting

**Container won't start?**
- Check logs: `docker-compose logs tony-bot`
- Verify .env configuration
- Ensure sufficient memory (2GB+ recommended)

**Can't connect to Discord?**
- Verify DISCORD_TOKEN is correct
- Check network connectivity
- Review Discord service logs

**Database connection issues?**
- Verify MongoDB container is running: `docker-compose ps`
- Check MongoDB logs: `docker-compose logs mongodb`
- Ensure MONGO_ROOT_PASSWORD is set

## 📚 Complete Documentation

For detailed setup instructions, troubleshooting, and advanced configuration, see [deployment/README.md](deployment/README.md).

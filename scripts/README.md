# Tony Bot - PowerShell Scripts

This directory contains PowerShell scripts for managing the Tony Bot development workflow on Windows.

## Prerequisites

- Windows 10/11 with PowerShell 5.1+ or PowerShell 7+
- .NET 10.0 preview SDK
- MongoDB (local or remote)
- Discord application with bot token

## Available Scripts

### `Setup-Development.ps1`
Sets up the development environment for the first time.

```powershell
.\Setup-Development.ps1
```

**Parameters:**
- `-Force`: Overwrites existing configuration files

**What it does:**
- Checks for .NET 10.0 preview SDK
- Restores NuGet packages
- Initializes user secrets
- Tests MongoDB connection
- Creates sample configuration files

### `Build.ps1`
Builds the entire solution.

```powershell
.\Build.ps1 -Configuration Release -Clean -Restore
```

**Parameters:**
- `-Configuration`: Build configuration (Debug/Release, default: Release)
- `-Clean`: Clean the solution before building
- `-Restore`: Restore packages before building

### `Run-Development.ps1`
Runs the application in development mode.

```powershell
.\Run-Development.ps1 -Watch -Build
```

**Parameters:**
- `-Environment`: Set the environment (default: Development)
- `-Watch`: Enable file watching for automatic restarts
- `-Build`: Build the project before running

**URLs when running:**
- API Documentation: `https://localhost:5001/scalar/v1`
- Application: `https://localhost:5001`

### `Update-Packages.ps1`
Updates all NuGet packages to their latest versions.

```powershell
.\Update-Packages.ps1 -Preview -DryRun
```

**Parameters:**
- `-Preview`: Include prerelease packages
- `-DryRun`: Show what would be updated without making changes

## Quick Start

1. **First-time setup:**
   ```powershell
   .\Setup-Development.ps1
   ```

2. **Configure your bot:**
   - Set Discord bot token in user secrets or appsettings.Development.json
   - Configure MongoDB connection string

3. **Run the application:**
   ```powershell
   .\Run-Development.ps1 -Watch
   ```

4. **Access the API documentation:**
   Open your browser to `https://localhost:5001/scalar/v1`

## Configuration

### User Secrets
Store sensitive configuration in user secrets:

```powershell
dotnet user-secrets set "Discord:Token" "your-bot-token-here" --project ..\src\Tony\Tony.csproj
dotnet user-secrets set "MongoDB:ConnectionString" "mongodb://localhost:27017" --project ..\src\Tony\Tony.csproj
```

### Environment Variables
You can also use environment variables:

```powershell
$env:Discord__Token = "your-bot-token-here"
$env:MongoDB__ConnectionString = "mongodb://localhost:27017"
```

## Troubleshooting

### Common Issues

**MongoDB Connection Failed:**
- Ensure MongoDB is running: `mongod --version`
- Check connection string in configuration
- Verify firewall settings

**Discord Bot Token Issues:**
- Verify token is correct in user secrets or configuration
- Ensure bot has necessary permissions in Discord server
- Check if bot is added to the intended Discord server

**Build Failures:**
- Run `.\Update-Packages.ps1` to ensure latest package versions
- Clear NuGet cache: `dotnet nuget locals all --clear`
- Delete `bin` and `obj` folders and rebuild

**Port Already in Use:**
- Change the port in `launchSettings.json`
- Kill existing processes using the port

### Getting Help

1. Check the logs in the console output
2. Review the Copilot instructions in `.github/copilot-instructions.md`
3. Ensure all prerequisites are installed
4. Verify configuration settings

## Development Workflow

1. **Daily development:**
   ```powershell
   .\Run-Development.ps1 -Watch
   ```

2. **Before committing:**
   ```powershell
   .\Build.ps1 -Configuration Release -Clean -Restore
   ```

3. **Update dependencies monthly:**
   ```powershell
   .\Update-Packages.ps1
   ```

4. **Clean rebuild when needed:**
   ```powershell
   .\Build.ps1 -Clean -Restore
   ```

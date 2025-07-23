# Tony Bot

A Discord bot built with .NET 8.0, ASP.NET Core Web API, and MongoDB. Tony Bot provides Discord server management features and exposes a REST API for external integrations.

## Features

- **Discord Bot Integration**: Built with Discord.Net for robust Discord interactions
- **REST API**: ASP.NET Core Web API with modern documentation via Scalar
- **MongoDB Database**: Persistent data storage with MongoDB
- **Clean Architecture**: Separated layers for maintainability and testability
- **Slash Commands**: Modern Discord interaction patterns
- **GitHub Integration**: Models and webhooks for GitHub events

## Technology Stack

- **.NET 8.0**: Latest Long Term Support version
- **ASP.NET Core**: Web API framework
- **Discord.Net 3.16+**: Discord bot library
- **MongoDB**: NoSQL database with MongoDB.Driver
- **Scalar**: Modern API documentation (replacing Swagger UI)

## Project Structure

```
src/
├── Tony/                    # Main web application
├── Abstractions/           # Interfaces and contracts
├── Models/                 # Data models and DTOs
├── Repositories/          # Data access layer
└── Services/              # Business logic layer
```

## Quick Start

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB](https://www.mongodb.com/try/download/community) (local or cloud)
- Discord Application with Bot Token
- Windows 10/11 with PowerShell

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/Fortinbra/Tony.git
   cd Tony
   ```

2. **Run the setup script**
   ```powershell
   .\scripts\Setup-Development.ps1
   ```

3. **Configure the bot**
   ```powershell
   # Set Discord bot token in user secrets
   dotnet user-secrets set "Discord:Token" "your-bot-token-here" --project src/Tony/Tony.csproj
   
   # Configure MongoDB connection (if not using localhost)
   dotnet user-secrets set "MongoDB:ConnectionString" "your-mongodb-connection-string" --project src/Tony/Tony.csproj
   ```

4. **Run the application**
   ```powershell
   .\scripts\Run-Development.ps1 -Watch
   ```

5. **Access the API documentation**
   Open your browser to: `https://localhost:5001/scalar/v1`

## Development

### Available Scripts

All development scripts are PowerShell-based for Windows compatibility:

- `.\scripts\Setup-Development.ps1` - Initial environment setup
- `.\scripts\Build.ps1` - Build the solution
- `.\scripts\Run-Development.ps1` - Run in development mode
- `.\scripts\Update-Packages.ps1` - Update NuGet packages

See `scripts/README.md` for detailed usage instructions.

### Architecture

The project follows clean architecture principles:

- **Tony**: Main application with controllers and startup configuration
- **Abstractions**: Interfaces for repositories and services
- **Models**: Data models, DTOs, and entities
- **Repositories**: Data access implementations (MongoDB)
- **Services**: Business logic and Discord bot functionality

### Adding New Features

#### Discord Commands
1. Create command class in `src/Services/Discord/SlashCommands/`
2. Implement the command interface
3. Register in Discord extensions

#### API Endpoints
1. Add controller in `src/Tony/Controllers/`
2. Create service interface in `src/Abstractions/Services/`
3. Implement service in `src/Services/API/`
4. Register in service extensions

## Configuration

### User Secrets (Development)
```powershell
dotnet user-secrets set "Discord:Token" "your-bot-token"
dotnet user-secrets set "Discord:GuildId" "your-guild-id"
dotnet user-secrets set "MongoDB:ConnectionString" "mongodb://localhost:27017"
dotnet user-secrets set "MongoDB:DatabaseName" "TonyBot"
```

### Environment Variables (Production)
```bash
Discord__Token=your-bot-token
Discord__GuildId=your-guild-id
MongoDB__ConnectionString=mongodb://localhost:27017
MongoDB__DatabaseName=TonyBot
```

### Configuration Files
- `appsettings.json`: Base configuration
- `appsettings.Development.json`: Development overrides
- `appsettings.Production.json`: Production settings

## API Documentation

The API uses Scalar for modern, interactive documentation. When running in development mode, access the documentation at:

`https://localhost:5001/scalar/v1`

## Database

The application uses MongoDB for data persistence. The repository pattern abstracts database operations, making it easy to test and maintain.

### Collections
- **Users**: Discord user information and preferences
- **Logs**: Application and audit logs
- **GitHub**: GitHub webhook and event data

## Discord Bot Features

Current Discord functionality includes:
- Slash command framework
- User management
- Integration with GitHub webhooks
- Extensible command system

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes following the existing patterns
4. Ensure all tests pass
5. Submit a pull request

### Development Guidelines

- Follow the existing clean architecture patterns
- Use async/await for all I/O operations
- Implement proper error handling
- Add appropriate logging
- Update documentation as needed

See `.github/copilot-instructions.md` for detailed development guidelines.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues and questions:
1. Check the troubleshooting section in `scripts/README.md`
2. Review the Copilot instructions in `.github/copilot-instructions.md`
3. Create an issue on GitHub

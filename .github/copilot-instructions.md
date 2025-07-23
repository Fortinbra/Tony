# Tony Bot - Copilot Instructions

## Project Overview
Tony is a .NET 8.0 web application with Discord bot functionality, built using ASP.NET Core Web API architecture with MongoDB as the database. The project follows a clean architecture pattern with separate layers for abstractions, models, repositories, and services.

## Technology Stack & Requirements

### Core Technologies
- **.NET SDK**: Use .NET 10.0 (latest preview version)
- **Discord.Net**: Use latest stable version (currently 3.x series)
- **MongoDB**: MongoDB.Driver and MongoDB.Bson packages
- **API Documentation**: Use Scalar instead of Swagger/SwaggerUI for modern API documentation
- **Development Platform**: Windows with PowerShell scripts

### Key Dependencies
- ASP.NET Core Web API
- Discord.Net for Discord bot functionality
- MongoDB.Driver for database operations
- Microsoft.AspNetCore.OpenApi for API documentation

## Project Structure

```
src/
├── Tony/                    # Main web application project
│   ├── Controllers/         # API controllers
│   ├── ServiceExtensions/   # Dependency injection extensions
│   └── Program.cs          # Application entry point
├── Abstractions/           # Interfaces and abstractions
│   ├── Repositories/       # Repository interfaces
│   └── Services/          # Service interfaces
├── Models/                 # Data models and DTOs
│   ├── GitHub/            # GitHub-related models
│   ├── Logging/           # Logging models
│   └── Users/             # User models
├── Repositories/          # Data access layer
│   └── Mongo/             # MongoDB implementations
└── Services/              # Business logic layer
    ├── API/               # API-related services
    └── Discord/           # Discord bot services
        └── SlashCommands/ # Discord slash command implementations
```

## Development Guidelines

### Code Standards
- Use **nullable reference types** where appropriate
- Follow **clean architecture** principles
- Implement **dependency injection** patterns
- Use **async/await** for all I/O operations
- Follow C# naming conventions and best practices
- **All projects treat warnings as errors** - ensure code is warning-free

### When Adding New Features

#### Discord Bot Features
- Add new slash commands in `src/Services/Discord/SlashCommands/`
- Implement command interfaces in `src/Abstractions/Services/`
- Register services in `src/Tony/ServiceExtensions/DiscordExtensions.cs`
- Use Discord.Net's interaction framework for command handling

#### API Endpoints
- Add controllers in `src/Tony/Controllers/`
- Implement service interfaces in `src/Abstractions/Services/`
- Create service implementations in `src/Services/API/`
- Register services in `src/Tony/ServiceExtensions/ServicesExtensions.cs`

#### Data Models
- Add models in appropriate `src/Models/` subdirectories
- Implement MongoDB attributes for database mapping
- Create repository interfaces in `src/Abstractions/Repositories/`
- Implement repositories in `src/Repositories/Mongo/`

### Package Management
- Always use the **latest stable versions** of packages
- For Discord.Net, prefer the latest 3.x version unless breaking changes require migration
- Update MongoDB drivers regularly for security and performance improvements
- Use package references, not PackagesReference Include

### Configuration Management
- Store sensitive data in user secrets for development
- Use environment variables for production configuration
- Configure MongoDB connection strings through IConfiguration
- Set up Discord bot tokens securely

### PowerShell Scripts
All development scripts should be written in PowerShell for Windows compatibility:

#### Build Script Example
```powershell
# Build the entire solution
dotnet build src/Tony/Tony.sln --configuration Release

# Run tests if available
dotnet test src/Tony/Tony.sln --configuration Release

# Note: All projects are configured with TreatWarningsAsErrors=true
# Build will fail if any warnings are present
```

#### Development Setup Script
```powershell
# Restore packages
dotnet restore src/Tony/Tony.sln

# Set up user secrets
dotnet user-secrets init --project src/Tony/Tony.csproj
```

### API Documentation with Scalar
- Use Scalar for modern API documentation (no Swagger/SwaggerUI)
- Configure Scalar in Program.cs using built-in .NET 10 OpenAPI
- Use `AddOpenApi()` and `MapOpenApi()` for .NET 10 preview
- Example configuration:
```csharp
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // Modern API documentation
}
```

### MongoDB Best Practices
- Use repository pattern for data access
- Implement proper connection string management
- Use MongoDB.Bson attributes for model mapping
- Implement proper error handling for database operations
- Use async operations for all database calls

### Testing Guidelines
- Create unit tests for services and repositories
- Use integration tests for API endpoints
- Mock external dependencies (Discord API, MongoDB)
- Test Discord command functionality in isolation

### Deployment Considerations
- Containerize the application using Docker
- Use environment-specific configuration files
- Implement health checks for MongoDB and Discord connections
- Set up proper logging and monitoring

## Common Patterns

### Service Registration
```csharp
// In ServiceExtensions
public static IServiceCollection AddServices(this IServiceCollection services)
{
    services.AddScoped<IUserService, UserService>();
    return services;
}
```

### Repository Pattern
```csharp
// Repository interface
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByDiscordIdAsync(ulong discordId);
}

// MongoDB implementation
public class UserRepository : Repository<User>, IUserRepository
{
    public async Task<User?> GetByDiscordIdAsync(ulong discordId)
    {
        return await Collection.Find(u => u.DiscordId == discordId).FirstOrDefaultAsync();
    }
}
```

### Discord Command Pattern
```csharp
[SlashCommand("command-name", "Description of the command")]
public async Task HandleCommandAsync([Summary("param", "Parameter description")] string parameter)
{
    // Command implementation
    await RespondAsync("Response message");
}
```

## Version Control
- Use conventional commit messages
- Create feature branches for new functionality
- Use pull requests for code review
- Tag releases with semantic versioning

## Environment Setup
Ensure development environment has:
- .NET 10.0 preview SDK installed
- MongoDB running locally or connection to remote instance
- Discord application set up with bot token
- PowerShell 7+ for script execution
- Visual Studio or VS Code with C# extensions

When implementing new features, always consider the clean architecture principles, ensure proper dependency injection, and maintain consistency with the existing codebase patterns.

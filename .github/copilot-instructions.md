# Tony Bot - Copilot Instructions

## Project Overview
Tony is a .NET 8.0 web application with Discord bot functionality, built using ASP.NET Core Web API architecture with MongoDB as the database. The project follows a clean architecture pattern with separate layers for abstractions, models, repositories, and services.

**Development Philosophy**: The project strictly follows Test-Driven Development (TDD) principles - all features must be developed using the Red-Green-Refactor cycle, where tests are written first, followed by minimal implementation, then refactoring for quality.

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
- Follow **SOLID principles** (Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion)
- Implement **dependency injection** patterns
- Use **async/await** for all I/O operations
- Follow C# naming conventions and best practices
- **All projects treat warnings as errors** - ensure code is warning-free
- **Follow Test-Driven Development (TDD)** - write tests first, then implement functionality

### Test-Driven Development (TDD) Guidelines

#### TDD Workflow - Red, Green, Refactor
Follow the classic TDD cycle for all new features and bug fixes:

1. **Red**: Write a failing test that describes the desired behavior
2. **Green**: Write the minimal code necessary to make the test pass
3. **Refactor**: Improve the code while keeping tests green

#### TDD Implementation Steps

**1. Red Phase - Write Failing Tests**
```csharp
// Example: Write the test first
[Test]
public async Task GetUserAsync_WithValidId_ReturnsUser()
{
    // Arrange
    var userId = Guid.NewGuid();
    var expectedUser = new User { Id = userId, Name = "Test User" };
    _mockRepository.Setup(r => r.GetAsync(userId)).ReturnsAsync(expectedUser);

    // Act
    var result = await _userService.GetUserAsync(userId);

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Name, Is.EqualTo("Test User"));
}
```

**2. Green Phase - Minimal Implementation**
```csharp
// Write just enough code to make the test pass
public async Task<User?> GetUserAsync(Guid id)
{
    return await _repository.GetAsync(id);
}
```

**3. Refactor Phase - Improve Code Quality**
```csharp
// Add error handling, logging, validation while keeping tests green
public async Task<User?> GetUserAsync(Guid id)
{
    if (id == Guid.Empty)
        throw new ArgumentException("User ID cannot be empty", nameof(id));

    _logger.LogInformation("Retrieving user with ID: {UserId}", id);
    
    var user = await _repository.GetAsync(id);
    
    if (user == null)
        _logger.LogWarning("User not found with ID: {UserId}", id);
        
    return user;
}
```

#### TDD Best Practices

**Test Organization:**
- **Arrange**: Set up test data and mocks
- **Act**: Execute the method under test
- **Assert**: Verify the expected outcome
- Use descriptive test names: `MethodName_Scenario_ExpectedResult`

**Test Coverage Requirements:**
- **Unit Tests**: Test all public methods in services, repositories, and utilities
- **Integration Tests**: Test API endpoints end-to-end
- **Edge Cases**: Test null inputs, empty collections, boundary conditions
- **Error Scenarios**: Test exception handling and error responses

**Test Structure Example:**
```csharp
public class UserServiceTests
{
    private Mock<IUserRepository> _mockRepository;
    private Mock<ILogger<UserService>> _mockLogger;
    private UserService _userService;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _userService = new UserService(_mockRepository.Object, _mockLogger.Object);
    }

    [Test]
    public async Task GetUserAsync_WithValidId_ReturnsUser()
    {
        // Test implementation
    }

    [Test]
    public async Task GetUserAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Test implementation
    }

    [Test]
    public async Task GetUserAsync_UserNotFound_ReturnsNull()
    {
        // Test implementation
    }
}
```

#### TDD for Different Layers

**Controllers (API Endpoints):**
```csharp
[Test]
public async Task GetUser_WithValidId_ReturnsOkResult()
{
    // Arrange
    var userId = Guid.NewGuid();
    var user = new User { Id = userId };
    _mockUserService.Setup(s => s.GetUserAsync(userId)).ReturnsAsync(user);

    // Act
    var result = await _controller.GetUser(userId);

    // Assert
    Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
}
```

**Services (Business Logic):**
```csharp
[Test]
public async Task CreateUserAsync_WithValidUser_SavesAndReturnsUser()
{
    // Arrange
    var newUser = new User { Name = "New User" };
    _mockRepository.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(newUser);

    // Act
    var result = await _userService.CreateUserAsync(newUser);

    // Assert
    _mockRepository.Verify(r => r.CreateAsync(newUser), Times.Once);
    Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
}
```

**Repositories (Data Access):**
```csharp
[Test]
public async Task GetAsync_WithValidId_ReturnsUserFromDatabase()
{
    // Arrange
    var userId = Guid.NewGuid();
    // Use TestContainers or in-memory database for integration tests

    // Act
    var result = await _repository.GetAsync(userId);

    // Assert
    Assert.That(result, Is.Not.Null);
}
```

**Discord Commands:**
```csharp
[Test]
public async Task PingCommand_WhenCalled_RespondsWith Pong()
{
    // Arrange
    var mockContext = new Mock<ISlashCommandContext>();

    // Act
    await _pingCommand.HandleAsync(mockContext.Object);

    // Assert
    mockContext.Verify(c => c.RespondAsync("Pong!", null, false, false, null, null, null, null, null), Times.Once);
}
```

#### TDD Anti-Patterns to Avoid

**Don't:**
- Write tests after implementation (that's not TDD)
- Test implementation details instead of behavior
- Write overly complex tests that are hard to understand
- Skip the refactor phase
- Write tests that depend on external services without mocking

**Do:**
- Write simple, focused tests
- Test public interfaces, not private methods
- Use meaningful assertions
- Keep tests independent and isolated
- Mock external dependencies

#### TDD Workflow Integration

**Before Starting Any Feature:**
1. Write failing test(s) for the desired functionality
2. Run tests to confirm they fail (Red)
3. Write minimal implementation to pass tests (Green)
4. Refactor code while keeping tests green
5. Commit changes with meaningful messages

**PowerShell TDD Development Script:**
```powershell
# TDD development cycle script
function Invoke-TDDCycle {
    param(
        [string]$TestProject,
        [string]$TestFilter = ""
    )
    
    Write-Host "Running TDD Cycle..." -ForegroundColor Yellow
    
    # Run specific tests
    if ($TestFilter) {
        dotnet test $TestProject --filter $TestFilter --verbosity normal
    } else {
        dotnet test $TestProject --verbosity normal
    }
    
    # Build solution to check for compilation errors
    dotnet build Tony.sln --verbosity quiet
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Tests passing - Ready for refactor phase" -ForegroundColor Green
    } else {
        Write-Host "❌ Tests failing - Continue with implementation" -ForegroundColor Red
    }
}
```

### SOLID Principles Implementation

#### Single Responsibility Principle (SRP)
- Each class should have only one reason to change
- Services should handle only one specific domain concern
- Controllers should only handle HTTP requests and responses
- Repositories should only handle data access for one entity type

#### Open/Closed Principle (OCP)
- Classes should be open for extension but closed for modification
- Use interfaces and abstract classes for extensibility
- Implement new features through inheritance or composition, not modification
- Use strategy pattern for varying behaviors

#### Liskov Substitution Principle (LSP)
- Derived classes must be substitutable for their base classes
- Implementations should honor the contracts defined by interfaces
- Avoid strengthening preconditions or weakening postconditions in derived classes

#### Interface Segregation Principle (ISP)
- Create specific, focused interfaces rather than large, general ones
- Clients should not depend on interfaces they don't use
- Split large interfaces into smaller, role-specific interfaces
- Example: `IUserRepository` vs `IUserNotificationRepository`

#### Dependency Inversion Principle (DIP)
- Depend on abstractions, not concretions
- High-level modules should not depend on low-level modules
- Use dependency injection to invert control
- Register dependencies in `ServiceExtensions` classes

**Example of SOLID implementation:**
```csharp
// Good: Follows SRP and DIP
public interface IUserService
{
    Task<User?> GetUserAsync(Guid id);
    Task<User> CreateUserAsync(User user);
}

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository repository, ILogger<UserService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<User?> GetUserAsync(Guid id)
    {
        _logger.LogInformation("Retrieving user with ID: {UserId}", id);
        return await _repository.GetAsync(id);
    }
}
```

#### SOLID Anti-Patterns to Avoid

**Violating SRP:**
```csharp
// Bad: Class doing too many things
public class UserService
{
    public User GetUser(Guid id) { /* data access */ }
    public void SendEmail(string email) { /* email logic */ }
    public void LogActivity(string message) { /* logging logic */ }
}

// Good: Separate responsibilities
public class UserService { /* only user business logic */ }
public class EmailService { /* only email logic */ }
public class Logger { /* only logging logic */ }
```

**Violating DIP:**
```csharp
// Bad: Depending on concrete implementation
public class UserService
{
    private readonly MongoUserRepository _repository; // Concrete dependency
}

// Good: Depending on abstraction
public class UserService
{
    private readonly IUserRepository _repository; // Abstract dependency
}
```

#### Code Review Checklist for SOLID Principles

When reviewing code or implementing new features, ask:

1. **SRP**: Does this class have only one reason to change? If it handles multiple concerns, split it.
2. **OCP**: Can I add new functionality without modifying existing code? Use interfaces and inheritance.
3. **LSP**: Can I substitute derived classes without breaking functionality? Ensure contracts are honored.
4. **ISP**: Does this interface force clients to depend on methods they don't use? Split large interfaces.
5. **DIP**: Does this class depend on concrete implementations? Inject abstractions instead.

**Red Flags:**
- Classes with multiple responsibilities
- Direct instantiation of dependencies (`new SomeService()`)
- Large interfaces with unrelated methods
- Tight coupling between layers
- Violation of abstraction boundaries

### Current SOLID Implementation in Tony Bot

The project already demonstrates good SOLID principles:

**SRP Examples:**
- `DiscordClientManager` handles only Discord client lifecycle management
- `HealthController` handles only health check HTTP requests/responses
- `Repository<T>` handles only data access operations

**DIP Examples:**
- Controllers depend on health check service interfaces, not concrete implementations
- Services depend on `IRepository<T>` interface, not MongoDB specifics
- All dependencies are injected through constructors

**ISP Examples:**
- `IUserService` is focused on user operations only
- `IRepository<T>` provides general data access without entity-specific methods

**To maintain SOLID principles when extending:**
- Create specific service interfaces for new features
- Use dependency injection for all external dependencies
- Keep controllers thin - delegate to services
- Separate concerns across different classes and layers

#### SOLID and TDD Integration

**TDD supports SOLID principles naturally:**
- **SRP**: TDD encourages small, focused methods that are easy to test
- **OCP**: Test interfaces first, then implement extensions
- **LSP**: Tests verify that implementations honor interface contracts
- **ISP**: Mocking reveals when interfaces are too large or unfocused
- **DIP**: TDD requires dependency injection for effective mocking

**TDD Workflow for SOLID Design:**

1. **Define Interface (ISP/DIP)**: Start with a focused interface
```csharp
[Test]
public async Task CreateUserAsync_WithValidUser_ReturnsCreatedUser()
{
    // This test drives interface design
    var mockRepository = new Mock<IUserRepository>(); // Depends on abstraction
    var userService = new UserService(mockRepository.Object);
    
    var user = new User { Name = "Test User" };
    mockRepository.Setup(r => r.CreateAsync(user)).ReturnsAsync(user);
    
    var result = await userService.CreateUserAsync(user);
    
    Assert.That(result, Is.Not.Null);
}
```

2. **Implement Single Responsibility (SRP)**: Each test should verify one behavior
```csharp
// Good: Each test verifies one specific behavior
[Test] public async Task CreateUserAsync_WithValidUser_ReturnsUser() { }
[Test] public async Task CreateUserAsync_WithNullUser_ThrowsException() { }
[Test] public async Task CreateUserAsync_WithInvalidUser_ThrowsValidationException() { }
```

3. **Test Substitutability (LSP)**: Verify implementations work with interfaces
```csharp
[Test]
public async Task AnyUserRepository_ImplementsContract_Correctly()
{
    // Test that any IUserRepository implementation follows the contract
    IUserRepository repository = new MongoUserRepository(connectionString);
    // Or: IUserRepository repository = new InMemoryUserRepository();
    
    var user = new User { Name = "Test" };
    var created = await repository.CreateAsync(user);
    var retrieved = await repository.GetAsync(created.Id);
    
    Assert.That(retrieved.Name, Is.EqualTo(user.Name));
}
```

**Code Review Checklist with TDD and SOLID:**

When reviewing code or implementing new features, ask:
1. **SRP + TDD**: Can I test this class's single responsibility easily?
2. **OCP + TDD**: Can I add new functionality by adding tests and extending, not modifying?
3. **LSP + TDD**: Do my tests pass for all implementations of this interface?
4. **ISP + TDD**: Am I mocking only the methods my class actually uses?
5. **DIP + TDD**: Am I injecting all dependencies to make testing possible?

**Red Flags (Anti-Patterns):**
- Tests that require complex setup (violates SRP)
- Mocking concrete classes instead of interfaces (violates DIP)
- Tests that break when changing implementation details (violates OCP)
- Large mock setups indicating fat interfaces (violates ISP)
- Tests that can't substitute implementations (violates LSP)

### When Adding New Features

#### Discord Bot Features
- Add new slash commands in `src/Services/Discord/SlashCommands/`
- Implement command interfaces in `src/Abstractions/Services/`
- Register services in `src/Tony/ServiceExtensions/DiscordExtensions.cs`
- Use Discord.Net's interaction framework for command handling
- **SOLID**: Each command should have a single responsibility (SRP)
- **TDD**: Write tests first for command behavior and interactions

#### API Endpoints
- Add controllers in `src/Tony/Controllers/`
- Implement service interfaces in `src/Abstractions/Services/`
- Create service implementations in `src/Services/API/`
- Register services in `src/Tony/ServiceExtensions/ServicesExtensions.cs`
- **SOLID**: Controllers should only handle HTTP concerns, delegate business logic to services (SRP, DIP)
- **TDD**: Write controller tests first, then service tests, then implement

#### Data Models
- Add models in appropriate `src/Models/` subdirectories
- Implement MongoDB attributes for database mapping
- Create repository interfaces in `src/Abstractions/Repositories/`
- Implement repositories in `src/Repositories/Mongo/`
- **SOLID**: Repositories should be focused on single entity types (SRP, ISP)
- **TDD**: Write repository tests first, focusing on data access contracts

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
dotnet build Tony.sln --configuration Release

# Run tests if available
dotnet test Tony.sln --configuration Release

# Note: All projects are configured with TreatWarningsAsErrors=true
# Build will fail if any warnings are present
```

#### Development Setup Script
```powershell
# Restore packages
dotnet restore Tony.sln

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

#### Testing Framework and Tools
- **Unit Testing**: Use NUnit or xUnit for unit tests
- **Mocking**: Use Moq for creating test doubles
- **Integration Testing**: Use ASP.NET Core Test Host for API testing
- **Test Containers**: Use Testcontainers for database integration tests
- **Code Coverage**: Aim for 80%+ code coverage, 100% for critical business logic

#### Test Organization Structure
```
tests/
├── Models.Tests/           # Model validation and behavior tests
├── Repositories.Tests/     # Repository integration tests
├── Services.Tests/         # Service layer unit tests
└── Tony.Tests/            # API controller and integration tests
    ├── Controllers/        # Controller unit tests
    └── Integration/       # End-to-end integration tests
```

#### Test Categories

**Unit Tests (Fast, Isolated):**
- Test individual classes in isolation
- Mock all external dependencies
- Focus on business logic and edge cases
- Should run in milliseconds
- No database, file system, or network calls

**Integration Tests (Slower, Real Dependencies):**
- Test component interactions
- Use real database connections (TestContainers)
- Test API endpoints end-to-end
- Verify configuration and dependency injection
- Test data persistence and retrieval

**Test Naming Conventions:**
- `MethodName_Scenario_ExpectedResult`
- Examples:
  - `GetUserAsync_WithValidId_ReturnsUser`
  - `CreateUserAsync_WithNullUser_ThrowsArgumentNullException`
  - `DiscordCommand_WhenUserNotFound_ReturnsErrorMessage`

#### Testing Best Practices

**Test Data Management:**
- Use the Builder pattern for complex test objects
- Create test data factories for reusable objects
- Use AutoFixture for generating test data
- Isolate test data - each test should be independent

**Example Test Data Builder:**
```csharp
public class UserBuilder
{
    private User _user = new();

    public UserBuilder WithId(Guid id)
    {
        _user.Id = id;
        return this;
    }

    public UserBuilder WithName(string name)
    {
        _user.Name = name;
        return this;
    }

    public UserBuilder WithDiscordId(ulong discordId)
    {
        _user.DiscordId = discordId;
        return this;
    }

    public User Build() => _user;
}

// Usage in tests
var user = new UserBuilder()
    .WithId(Guid.NewGuid())
    .WithName("Test User")
    .WithDiscordId(123456789)
    .Build();
```

**Async Testing:**
- Always use `async Task` for async tests
- Don't use `.Result` or `.Wait()` - use `await`
- Test cancellation tokens where applicable

**Mock Setup Guidelines:**
- Setup mocks in test setup or arrange phase
- Verify interactions when testing behavior
- Use strict mocks when verifying all calls matter
- Reset mocks between tests if shared

**Discord Command Testing:**
```csharp
[Test]
public async Task UserInfoCommand_WithValidUser_DisplaysUserInformation()
{
    // Arrange
    var mockContext = new Mock<ISlashCommandContext>();
    var mockUser = new Mock<IUser>();
    mockUser.Setup(u => u.Id).Returns(123456789);
    mockUser.Setup(u => u.Username).Returns("TestUser");
    
    mockContext.Setup(c => c.User).Returns(mockUser.Object);
    
    var userService = new Mock<IUserService>();
    var user = new UserBuilder()
        .WithDiscordId(123456789)
        .WithName("TestUser")
        .Build();
    
    userService.Setup(s => s.GetByDiscordIdAsync(123456789))
               .ReturnsAsync(user);

    var command = new UserInfoCommand(userService.Object);

    // Act
    await command.HandleAsync(mockContext.Object);

    // Assert
    mockContext.Verify(c => c.RespondAsync(
        It.Is<string>(msg => msg.Contains("TestUser")),
        It.IsAny<Embed[]>(),
        It.IsAny<bool>(),
        It.IsAny<bool>(),
        It.IsAny<AllowedMentions>(),
        It.IsAny<RequestOptions>(),
        It.IsAny<MessageComponent>(),
        It.IsAny<Embed>(),
        It.IsAny<PollProperties>()), Times.Once);
}
```

**API Integration Testing:**
```csharp
[Test]
public async Task GetUser_WithValidId_ReturnsUserData()
{
    // Arrange - using TestServer and TestContainers
    var userId = Guid.NewGuid();
    await SeedTestData(userId);

    // Act
    var response = await _client.GetAsync($"/api/users/{userId}");
    var content = await response.Content.ReadAsStringAsync();
    var user = JsonSerializer.Deserialize<User>(content);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    user.Should().NotBeNull();
    user.Id.Should().Be(userId);
}
```

#### Test Environment Configuration

**Test Database Setup with TestContainers:**
```csharp
[SetUpFixture]
public class TestEnvironmentSetup
{
    private static MongoDbContainer _mongoContainer;

    [OneTimeSetUp]
    public async Task SetupTestEnvironment()
    {
        _mongoContainer = new MongoDbBuilder()
            .WithImage("mongo:latest")
            .WithPortBinding(27017, true)
            .Build();

        await _mongoContainer.StartAsync();
        
        // Set test connection string
        Environment.SetEnvironmentVariable("ConnectionStrings:MongoDB", 
            _mongoContainer.GetConnectionString());
    }

    [OneTimeTearDown]
    public async Task TearDownTestEnvironment()
    {
        await _mongoContainer.DisposeAsync();
    }
}
```

#### Continuous Testing

**Test Execution Strategy:**
- Run unit tests on every build (fast feedback)
- Run integration tests on pull requests
- Run full test suite before deployment
- Use parallel test execution for faster feedback

**PowerShell Test Scripts:**
```powershell
# Run only unit tests (fast)
dotnet test Tony.sln --filter TestCategory=Unit --verbosity normal

# Run integration tests
dotnet test Tony.sln --filter TestCategory=Integration --verbosity normal

# Run all tests with coverage
dotnet test Tony.sln --collect:"XPlat Code Coverage" --results-directory:TestResults
```

#### Testing Checklist

Before merging any code, ensure:
- [ ] All new code has corresponding tests (TDD)
- [ ] Tests follow naming conventions
- [ ] Tests are isolated and independent
- [ ] Mock external dependencies appropriately
- [ ] Integration tests cover critical paths
- [ ] Tests pass in both development and CI environments
- [ ] Code coverage meets project standards
- [ ] Tests are maintainable and readable

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

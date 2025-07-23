# Test Categorization Strategy

This document explains how tests are organized and categorized in the Tony Bot project.

## Test Categories

All tests are categorized using xUnit's `[Trait("Category", "CategoryName")]` attribute. This allows for flexible test filtering during CI/CD and development.

### 🔧 Unit Tests (`Category = "Unit"`)

**Purpose**: Test individual components in isolation with minimal dependencies.

**Characteristics**:

- Fast execution (< 100ms per test)
- No external dependencies (database, network, file system)
- Use mocking for dependencies
- Test single methods or small units of functionality
- Should make up 70-80% of total tests

**Examples**:

- Testing model validation logic
- Testing service methods with mocked dependencies
- Testing utility functions
- Testing controller logic with mocked services

**Current Examples**:

- `BaseTests.cs` - Tests for the base entity model
- `GitHubServiceTests.cs` - Tests for GitHub API service with mocked HTTP client
- `RepositoryTests.cs` - Tests for repository with mocked MongoDB collections

### 🔗 Integration Tests (`Category = "Integration"`)

**Purpose**: Test interactions between multiple components or external systems.

**Characteristics**:

- Moderate execution time (100ms - 5s per test)
- May use real external dependencies (database, APIs)
- Test component interactions and data flow
- Verify configuration and dependency injection
- Should make up 15-25% of total tests

**Examples**:

- Testing repository operations against real MongoDB
- Testing Discord client interactions
- Testing API integrations with external services
- Testing database migrations and seeding

**Setup Requirements**:

- Test database instance
- Test configuration files
- May require Docker containers for external dependencies

### 🌐 End-to-End Tests (`Category = "E2E"`)

**Purpose**: Test complete user workflows from start to finish.

**Characteristics**:

- Slow execution (1s - 30s+ per test)
- Use complete application stack
- Test real user scenarios
- Verify system behavior in production-like environment
- Should make up 5-10% of total tests

**Examples**:

- Complete Discord slash command workflow
- Full firmware download and response flow
- Health check system verification
- User role assignment workflows

**Setup Requirements**:

- Full application deployment
- Test Discord server
- External service mocks or test instances

### ⚡ Performance Tests (`Category = "Performance"`)

**Purpose**: Measure and validate system performance characteristics.

**Characteristics**:

- Focus on timing, throughput, and resource usage
- May have longer execution times
- Often run separately from functional tests
- Used to establish performance baselines

**Examples**:

- Database query performance
- API response times
- Memory usage patterns
- Concurrent user handling

### 🚭 Smoke Tests (`Category = "Smoke"`)

**Purpose**: Basic verification that core functionality works after deployment.

**Characteristics**:

- Quick validation of critical paths
- Run after deployments
- Cover essential features only
- Fast execution for rapid feedback

**Examples**:

- Application startup verification
- Basic health checks
- Critical API endpoint availability
- Database connectivity

## Running Tests by Category

### PowerShell Scripts

Use the enhanced `Run-Tests.ps1` script with category filtering:

```powershell
# Run all tests
.\scripts\Run-Tests.ps1

# Run only unit tests (fastest)
.\scripts\Run-Tests.ps1 -Category Unit

# Run integration tests with coverage
.\scripts\Run-Tests.ps1 -Category Integration -Coverage

# Run E2E tests with verbose output
.\scripts\Run-Tests.ps1 -Category E2E -Verbose
```

### .NET CLI

Run tests using specific run settings files:

```bash
# Unit tests only
dotnet test --settings tests.unit.runsettings

# Integration tests only  
dotnet test --settings tests.integration.runsettings

# E2E tests only
dotnet test --settings tests.e2e.runsettings

# Filter by category manually
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration|Category=E2E"
```

### Visual Studio

1. Open Test Explorer
2. Use the filter options to show tests by trait
3. Filter by `Category:Unit`, `Category:Integration`, etc.

## Test Organization Structure

```text
tests/
├── TestCategories.cs              # Constants for test categories
├── Models.Tests/
│   ├── BaseTests.cs              # Unit tests
│   └── Integration/              # Integration tests (future)
├── Repositories.Tests/
│   ├── Mongo/
│   │   ├── RepositoryTests.cs    # Unit tests
│   │   └── ReadOnlyRepositoryTests.cs
│   └── Integration/
│       └── MongoRepositoryIntegrationTests.cs
├── Services.Tests/
│   ├── API/
│   │   └── GitHubServiceTests.cs # Unit tests
│   └── Discord/
│       └── SlashCommands/        # Unit tests
└── Tony.Tests/
    ├── Controllers/
    │   └── HealthControllerTests.cs # Unit tests
    └── E2E/
        └── TonyBotE2ETests.cs    # E2E tests
```

## Best Practices

### Test Naming

Use descriptive test names that clearly indicate:

- What is being tested
- Under what conditions  
- What the expected outcome is

Example: `GetControllerUF2UrlAsync_WithValidController_ReturnsDownloadUrl`

### Test Categories Usage

1. **Start with Unit Tests**: Write unit tests first during TDD
2. **Add Integration Tests**: For complex multi-component interactions
3. **Minimal E2E Tests**: Only for critical user workflows
4. **Performance Tests**: Add when performance becomes a concern
5. **Smoke Tests**: Essential for deployment verification

### CI/CD Strategy

1. **Pull Request Builds**: Run Unit tests only for fast feedback
2. **Main Branch Builds**: Run Unit + Integration tests
3. **Deployment Pipeline**: Run all categories including E2E
4. **Nightly Builds**: Include Performance tests

### Coverage Goals

- **Unit Tests**: Aim for 80%+ code coverage
- **Integration Tests**: Focus on critical integration points
- **E2E Tests**: Cover main user workflows (not coverage-focused)

## Migration Strategy

All existing tests have been categorized as Unit tests using `[Trait("Category", "Unit")]`. As the application grows:

1. Identify tests that require real external dependencies
2. Move them to Integration category and update setup
3. Add E2E tests for critical user workflows
4. Consider Performance tests for bottlenecks

## Tools and Configuration

- **xUnit**: Primary testing framework with Trait support
- **Moq**: Mocking framework for unit tests
- **ASP.NET Core Test Host**: For integration and E2E tests
- **MongoDB Test Containers**: For integration tests (future)
- **Custom Run Settings**: Category-specific configurations
- **Coverage Reports**: Generated with ReportGenerator tool

This categorization strategy ensures efficient test execution, clear organization, and appropriate test coverage across all application layers.

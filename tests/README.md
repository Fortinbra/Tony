# Tony Bot Testing Guide

This document provides comprehensive information about the automated testing setup for the Tony Discord Bot project.

## 🧪 Test Structure

The testing solution is organized into several test projects, each focusing on a specific layer of the application:

```
tests/
├── Tony.Tests/              # Web API and Controller tests
│   ├── Controllers/         # Controller unit tests
│   └── Integration/         # Integration tests
├── Services.Tests/          # Service layer tests
│   └── HealthChecks/        # Health check service tests
├── Repositories.Tests/      # Repository layer tests
│   └── Mongo/              # MongoDB repository tests
└── Models.Tests/           # Model/Domain tests
```

## 🚀 Quick Start

### Prerequisites

- .NET 10.0 SDK or later
- MongoDB (for integration tests)
- PowerShell (for running test scripts)

### Running Tests

#### Option 1: PowerShell Script (Recommended)
```powershell
# Run all tests
.\scripts\Run-Tests.ps1

# Run tests with coverage
.\scripts\Run-Tests.ps1 -Coverage

# Run tests in watch mode
.\scripts\Run-Tests.ps1 -Watch

# Run tests with verbose output
.\scripts\Run-Tests.ps1 -Verbose
```

#### Option 2: Direct .NET CLI
```bash
# Restore packages
dotnet restore Tony.sln

# Build solution
dotnet build Tony.sln

# Run all tests
dotnet test Tony.sln

# Run tests with coverage
dotnet test Tony.sln --collect:"XPlat Code Coverage" --settings tests.runsettings
```

#### Option 3: Individual Test Projects
```bash
# Run specific test project
dotnet test tests/Tony.Tests/Tony.Tests.csproj
dotnet test tests/Services.Tests/Services.Tests.csproj
dotnet test tests/Repositories.Tests/Repositories.Tests.csproj
dotnet test tests/Models.Tests/Models.Tests.csproj
```

## 📊 Code Coverage

The test suite includes comprehensive code coverage reporting:

- **Minimum Coverage Target**: 80%
- **Reports Generated**: HTML, Cobertura, Markdown Summary
- **Exclusions**: Test projects, generated code, compiler-generated code

### Coverage Reports

After running tests with coverage, reports are generated in the `CodeCoverage/` directory:
- `index.html` - Interactive HTML coverage report
- `Cobertura.xml` - Cobertura format for CI/CD integration
- `Summary.md` - Markdown summary for pull requests

## 🧪 Test Categories

### Unit Tests

**Controller Tests** (`Tony.Tests/Controllers/`)
- `HealthControllerTests.cs` - Health check endpoints
- `UserControllerTests.cs` - User management endpoints  
- `WebhookControllerTests.cs` - GitHub webhook handling

**Service Tests** (`Services.Tests/`)
- `MongoHealthCheckTests.cs` - MongoDB health monitoring
- `DiscordHealthCheckTests.cs` - Discord connectivity monitoring

**Repository Tests** (`Repositories.Tests/`)
- `RepositoryTests.cs` - CRUD operations
- `ReadOnlyRepositoryTests.cs` - Read operations

**Model Tests** (`Models.Tests/`)
- `BaseTests.cs` - Base entity functionality

### Integration Tests

**Health Check Integration** (`Tony.Tests/Integration/`)
- End-to-end health check endpoint testing
- Real HTTP request/response validation
- Service dependency mocking

## 🎯 Testing Best Practices

### Test Structure
- **Arrange-Act-Assert (AAA)** pattern used consistently
- **Given-When-Then** naming for complex scenarios
- **Theory tests** for parameterized test cases

### Mocking Strategy
- **Moq** framework for creating test doubles
- **Interface-based** mocking for loose coupling
- **Behavior verification** for critical interactions

### Assertions
- **FluentAssertions** for readable and expressive assertions
- **Specific assertions** over generic ones
- **Meaningful error messages** when tests fail

### Coverage Goals
- **Controllers**: 100% - Full HTTP response testing
- **Services**: 95% - Business logic and error handling
- **Repositories**: 90% - CRUD operations and queries
- **Models**: 80% - Property validation and behavior

## 🔧 Tools and Frameworks

### Test Frameworks
- **xUnit** - Primary testing framework
- **FluentAssertions** - Assertion library
- **Moq** - Mocking framework

### Coverage Tools
- **Coverlet** - Code coverage collection
- **ReportGenerator** - Coverage report generation

### Additional Tools
- **Microsoft.AspNetCore.Mvc.Testing** - Integration testing
- **Microsoft.NET.Test.Sdk** - Test SDK

## 🚀 Continuous Integration

### GitHub Actions Workflow
The CI/CD pipeline (`.github/workflows/ci-cd.yml`) includes:

1. **Build Stage**
   - Multi-target framework compilation
   - NuGet package restoration
   - Build verification

2. **Test Stage**
   - Unit test execution
   - Integration test execution
   - Code coverage collection

3. **Quality Stage**
   - Coverage report generation
   - Security vulnerability scanning
   - Code quality analysis

4. **Deployment Stage** (main branch)
   - Docker image building
   - Container registry publishing

### Test Configuration
- **Parallel execution** enabled for faster test runs
- **MongoDB service** container for integration tests
- **Test result** artifacts uploaded
- **Coverage reports** commented on pull requests

## 📈 Metrics and Monitoring

### Key Metrics
- **Test Coverage**: Target 80%+ overall
- **Test Execution Time**: < 2 minutes for full suite
- **Test Reliability**: 99%+ pass rate
- **Build Success Rate**: 95%+ on main branch

### Quality Gates
- All tests must pass before merge
- Coverage cannot decrease from main branch
- No critical security vulnerabilities
- Build must succeed on all target platforms

## 🛠️ Troubleshooting

### Common Issues

**MongoDB Connection Failures**
```bash
# Ensure MongoDB is running
docker run -d -p 27017:27017 mongo:7.0

# Or use the provided docker-compose
docker-compose -f docker-compose.dev.yml up mongodb
```

**Coverage Report Generation Fails**
```bash
# Install ReportGenerator globally
dotnet tool install -g dotnet-reportgenerator-globaltool

# Or restore local tools
dotnet tool restore
```

**Test Discovery Issues**
```bash
# Clean and rebuild
dotnet clean Tony.sln
dotnet build Tony.sln
```

### Debug Mode
```bash
# Run tests with detailed output
dotnet test --verbosity diagnostic

# Run specific test with debugging
dotnet test --filter "HealthControllerTests" --logger "console;verbosity=detailed"
```

## 📚 Additional Resources

- [xUnit Documentation](https://xunit.net/docs/getting-started/netcore/cmdline)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Moq Documentation](https://github.com/moq/moq4)
- [.NET Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

## 🤝 Contributing

When adding new features:

1. **Write tests first** (TDD approach recommended)
2. **Maintain or improve** code coverage
3. **Follow existing** test patterns and naming conventions
4. **Update this documentation** if adding new test categories
5. **Ensure all tests pass** before submitting pull requests

## 📝 Test Naming Convention

```csharp
// Unit Tests
[MethodUnderTest]_[StateUnderTest]_[ExpectedBehavior]
GetHealthAsync_WhenServiceHealthy_ReturnsOkResult()

// Integration Tests  
[Feature]_[Scenario]_[ExpectedOutcome]
HealthEndpoint_WhenAllServicesUp_ReturnsHealthyStatus()

// Theory Tests
[MethodUnderTest]_[ParameterDescription]_[ExpectedBehavior]
ValidateInput_WithVariousInvalidInputs_ThrowsArgumentException()
```

---

For questions or issues with the testing setup, please create an issue in the repository or contact the development team.

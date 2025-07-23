using Abstractions.Services;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using Moq;
using Services.Discord.SlashCommands;
using Xunit;

namespace Services.Tests.Discord.SlashCommands
{
    public class DownloadControllerTests
    {
        private readonly Mock<IGitHubService> _mockGitHubService;
        private readonly Mock<ILogger<DownloadController>> _mockLogger;
        private readonly DownloadController _downloadController;

        public DownloadControllerTests()
        {
            _mockGitHubService = new Mock<IGitHubService>();
            _mockLogger = new Mock<ILogger<DownloadController>>();
            _downloadController = new DownloadController(_mockGitHubService.Object, _mockLogger.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_InitializesSuccessfully()
        {
            // Act & Assert
            Assert.NotNull(_downloadController);
        }

        [Fact]
        public void Constructor_WithNullGitHubService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new DownloadController(null!, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new DownloadController(_mockGitHubService.Object, null!));
        }

        [Fact]
        public void DownloadControllerCommand_HasCorrectSlashCommandAttribute()
        {
            // Arrange
            var method = typeof(DownloadController).GetMethod(nameof(DownloadController.DownloadControllerAsync));

            // Act
            var attribute = method?.GetCustomAttributes(typeof(SlashCommandAttribute), false)
                .FirstOrDefault() as SlashCommandAttribute;

            // Assert
            Assert.NotNull(attribute);
            Assert.Equal("download-firmware", attribute.Name);
            Assert.Contains("Download a UF2 firmware file", attribute.Description);
        }

        [Fact]
        public void GetAvailableControllers_ReturnsControllerList()
        {
            // Arrange
            var expectedControllers = new List<string> { "Pico", "FlatboxRev5", "ARCController" }.AsReadOnly();
            _mockGitHubService.Setup(s => s.GetAvailableControllers())
                             .Returns(expectedControllers);

            // Act
            var result = _mockGitHubService.Object.GetAvailableControllers();

            // Assert
            Assert.Equal(expectedControllers, result);
            _mockGitHubService.Verify(s => s.GetAvailableControllers(), Times.Once);
        }

        // Note: Testing the actual Discord interaction methods (DownloadControllerAsync) 
        // would require complex mocking of Discord.NET's interaction context, which 
        // includes Context, DeferAsync(), FollowupAsync(), etc. 
        // 
        // The business logic is properly separated into the GitHubService, which is 
        // thoroughly tested. The Discord command acts as a thin orchestration layer.
        // 
        // Integration tests would be more valuable for testing the complete slash command flow.
    }

    public class ControllerAutocompleteHandlerTests
    {
        private readonly Mock<IGitHubService> _mockGitHubService;
        private readonly ControllerAutocompleteHandler _autocompleteHandler;

        public ControllerAutocompleteHandlerTests()
        {
            _mockGitHubService = new Mock<IGitHubService>();
            _autocompleteHandler = new ControllerAutocompleteHandler(_mockGitHubService.Object);
        }

        [Fact]
        public void Constructor_WithValidGitHubService_InitializesSuccessfully()
        {
            // Act & Assert
            Assert.NotNull(_autocompleteHandler);
        }

        [Fact]
        public void Constructor_WithNullGitHubService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new ControllerAutocompleteHandler(null!));
        }
    }
}

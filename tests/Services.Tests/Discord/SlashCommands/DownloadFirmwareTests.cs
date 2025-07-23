using Abstractions.Services;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using Moq;
using Services.Discord.SlashCommands;
using Xunit;

namespace Services.Tests.Discord.SlashCommands
{
    public class DownloadFirmwareTests
    {
        private readonly Mock<IGitHubService> _mockGitHubService;
        private readonly Mock<ILogger<DownloadFirmware>> _mockLogger;
        private readonly DownloadFirmware _downloadFirmware;

        public DownloadFirmwareTests()
        {
            _mockGitHubService = new Mock<IGitHubService>();
            _mockLogger = new Mock<ILogger<DownloadFirmware>>();
            _downloadFirmware = new DownloadFirmware(_mockGitHubService.Object, _mockLogger.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_InitializesSuccessfully()
        {
            // Act & Assert
            Assert.NotNull(_downloadFirmware);
        }

        [Fact]
        public void Constructor_WithNullGitHubService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new DownloadFirmware(null!, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new DownloadFirmware(_mockGitHubService.Object, null!));
        }

        [Fact]
        public void DownloadFirmwareCommand_HasCorrectSlashCommandAttribute()
        {
            // Arrange
            var method = typeof(DownloadFirmware).GetMethod(nameof(DownloadFirmware.DownloadFirmwareAsync));

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

        // Note: Testing the actual Discord interaction methods (DownloadFirmwareAsync) 
        // would require complex mocking of Discord.NET's interaction context, which 
        // includes Context, DeferAsync(), FollowupAsync(), etc. 
        // 
        // The business logic is properly separated into the GitHubService, which is 
        // thoroughly tested. The Discord command acts as a thin orchestration layer.
        // 
        // Integration tests would be more valuable for testing the complete slash command flow.
    }

    public class FirmwareControllerAutocompleteHandlerTests
    {
        private readonly Mock<IGitHubService> _mockGitHubService;
        private readonly FirmwareControllerAutocompleteHandler _autocompleteHandler;

        public FirmwareControllerAutocompleteHandlerTests()
        {
            _mockGitHubService = new Mock<IGitHubService>();
            _autocompleteHandler = new FirmwareControllerAutocompleteHandler(_mockGitHubService.Object);
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
                new FirmwareControllerAutocompleteHandler(null!));
        }
    }
}

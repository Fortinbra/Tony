using Abstractions.Services;
using Moq;
using Services.Discord.SlashCommands;
using Xunit;

namespace Services.Tests.Discord.SlashCommands
{
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
        [Trait("Category", "Unit")]
        public void Constructor_WithValidGitHubService_InitializesSuccessfully()
        {
            // Act & Assert
            Assert.NotNull(_autocompleteHandler);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Constructor_WithNullGitHubService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new FirmwareControllerAutocompleteHandler(null!));
        }
    }
}

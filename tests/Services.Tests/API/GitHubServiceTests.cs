using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using Xunit;
using Services.API;
using Models.GitHub;

namespace Services.Tests.API
{
    public class GitHubServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly Mock<ILogger<GitHubService>> _mockLogger;
        private readonly HttpClient _httpClient;
        private readonly GitHubService _gitHubService;

        public GitHubServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockLogger = new Mock<ILogger<GitHubService>>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _gitHubService = new GitHubService(_httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task GetControllerUF2UrlAsync_WithValidController_ReturnsDownloadUrl()
        {
            // Arrange
            var controllerName = "Pico";
            var tag = "v0.7.11";
            var versionNumber = "0.7.11"; // GitHub assets use version without 'v' prefix
            var expectedUrl = $"https://github.com/OpenStickCommunity/GP2040-CE/releases/download/{tag}/GP2040-CE_{versionNumber}_{controllerName}.uf2";

            var mockRelease = new GitHubRelease
            {
                Assets = new List<GitHubReleaseAsset>
                {
                    new() 
                    { 
                        Name = $"GP2040-CE_{versionNumber}_{controllerName}.uf2",
                        BrowserDownloadUrl = expectedUrl
                    }
                }
            };

            var jsonContent = JsonSerializer.Serialize(mockRelease);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonContent)
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _gitHubService.GetControllerUF2UrlAsync(controllerName, tag);

            // Assert
            Assert.Equal(expectedUrl, result);
        }

        [Fact]
        public async Task GetControllerUF2UrlAsync_WithInvalidController_ReturnsNull()
        {
            // Arrange
            var controllerName = "InvalidController";
            var tag = "v0.7.11";

            var mockRelease = new GitHubRelease
            {
                Assets = new List<GitHubReleaseAsset>
                {
                    new() 
                    { 
                        Name = "GP2040-CE_0.7.11_Pico.uf2", // Use actual GitHub asset filename format
                        BrowserDownloadUrl = "https://example.com/file.uf2"
                    }
                }
            };

            var jsonContent = JsonSerializer.Serialize(mockRelease);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonContent)
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _gitHubService.GetControllerUF2UrlAsync(controllerName, tag);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetControllerUF2UrlAsync_WhenHttpRequestFails_ReturnsNull()
        {
            // Arrange
            var controllerName = "Pico";
            var tag = "v0.7.11";

            var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _gitHubService.GetControllerUF2UrlAsync(controllerName, tag);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetControllerUF2UrlAsync_WhenExceptionThrown_ReturnsNull()
        {
            // Arrange
            var controllerName = "Pico";
            var tag = "v0.7.11";

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            var result = await _gitHubService.GetControllerUF2UrlAsync(controllerName, tag);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetControllerUF2UrlAsync_WithNullAssets_ReturnsNull()
        {
            // Arrange
            var controllerName = "Pico";
            var tag = "v0.7.11";

            var mockRelease = new GitHubRelease
            {
                Assets = null
            };

            var jsonContent = JsonSerializer.Serialize(mockRelease);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonContent)
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _gitHubService.GetControllerUF2UrlAsync(controllerName, tag);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetAvailableControllers_ReturnsAllControllers()
        {
            // Act
            var result = _gitHubService.GetAvailableControllers();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("Pico", result);
            Assert.Contains("ARCController", result);
            Assert.Contains("FlatboxRev5", result);
        }

        [Theory]
        [InlineData("v0.7.11", "0.7.11")] // Version with 'v' prefix
        [InlineData("0.7.11", "0.7.11")]  // Version without 'v' prefix
        public async Task GetControllerUF2UrlAsync_HandlesVersionFormatsCorrectly(string inputTag, string expectedVersionInFilename)
        {
            // Arrange
            var controllerName = "Pico";
            var expectedUrl = $"https://github.com/OpenStickCommunity/GP2040-CE/releases/download/{inputTag}/GP2040-CE_{expectedVersionInFilename}_{controllerName}.uf2";

            var mockRelease = new GitHubRelease
            {
                Assets = new List<GitHubReleaseAsset>
                {
                    new() 
                    { 
                        Name = $"GP2040-CE_{expectedVersionInFilename}_{controllerName}.uf2",
                        BrowserDownloadUrl = expectedUrl
                    }
                }
            };

            var jsonContent = JsonSerializer.Serialize(mockRelease);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonContent)
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _gitHubService.GetControllerUF2UrlAsync(controllerName, inputTag);

            // Assert
            Assert.Equal(expectedUrl, result);
        }

        [Fact]
        public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new GitHubService(null!, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new GitHubService(_httpClient, null!));
        }

        [Fact]
        public void Constructor_SetsUserAgentHeader()
        {
            // Arrange & Act
            var service = new GitHubService(_httpClient, _mockLogger.Object);

            // Assert
            Assert.Contains(_httpClient.DefaultRequestHeaders.UserAgent, 
                header => header.ToString().Contains("Tony-Bot/1.0"));
        }
    }
}

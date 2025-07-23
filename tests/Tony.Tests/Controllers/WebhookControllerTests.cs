using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Tony.Controllers;
using Xunit;

namespace Tony.Tests.Controllers
{
    public class WebhookControllerTests
    {
        private readonly Mock<ILogger<WebhookController>> _mockLogger;
        private readonly WebhookController _controller;

        public WebhookControllerTests()
        {
            _mockLogger = new Mock<ILogger<WebhookController>>();
            _controller = new WebhookController(_mockLogger.Object);
        }

        [Fact]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new WebhookController(null!));
            Assert.Equal("logger", exception.ParamName);
        }

        [Fact]
        public void ProcessWebhook_ReturnsOkResult()
        {
            // Act
            var result = _controller.ProcessWebhook();

            // Assert
            Assert.IsType<OkResult>(result);
        }
    }
}

using Abstractions.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Services.Discord
{
    public class DiscordHostedService : BackgroundService
    {
        private readonly IDiscordClientManager _clientManager;
        private readonly IDiscordInteractionSetup _interactionSetup;
        private readonly ILogger<DiscordHostedService> _logger;

        public DiscordHostedService(
            IDiscordClientManager clientManager,
            IDiscordInteractionSetup interactionSetup,
            ILogger<DiscordHostedService> logger)
        {
            _clientManager = clientManager ?? throw new ArgumentNullException(nameof(clientManager));
            _interactionSetup = interactionSetup ?? throw new ArgumentNullException(nameof(interactionSetup));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _clientManager.StartAsync(stoppingToken);
                await _interactionSetup.SetupInteractionsAsync();
                
                _logger.LogInformation("Discord hosted service started successfully");
                
                // Keep the service running until cancellation is requested
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Discord hosted service is stopping");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Discord hosted service encountered an error");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Discord hosted service is stopping");
            await _clientManager.StopAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }
    }
}

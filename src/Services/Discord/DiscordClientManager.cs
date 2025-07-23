using Abstractions.Services;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;

namespace Services.Discord
{
    public class DiscordClientManager : IDiscordClientManager
    {
        private readonly DiscordSocketClient _client;
        private readonly IDiscordEventHandler _eventHandler;
        private readonly IDiscordInteractionSetup _interactionSetup;
        private readonly ILogger<DiscordClientManager> _logger;
        private readonly string _botToken;

        public DiscordClientManager(
            DiscordSocketClient client,
            IDiscordEventHandler eventHandler,
            IDiscordInteractionSetup interactionSetup,
            IOptions<DiscordOptions> options,
            ILogger<DiscordClientManager> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
            _interactionSetup = interactionSetup ?? throw new ArgumentNullException(nameof(interactionSetup));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _botToken = options.Value.Token;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.Log += LogAsync;
            _client.Ready += _eventHandler.HandleReadyAsync;
            _client.InteractionCreated += _eventHandler.HandleInteractionCreatedAsync;

            await _client.LoginAsync(TokenType.Bot, _botToken);
            await _client.StartAsync();

            _logger.LogInformation("Discord client started successfully");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.StopAsync();
            await _client.LogoutAsync();
            _logger.LogInformation("Discord client stopped");
        }

        private Task LogAsync(LogMessage msg)
        {
            _logger.LogInformation("Discord: {Message}", msg.ToString());
            return Task.CompletedTask;
        }
    }
}

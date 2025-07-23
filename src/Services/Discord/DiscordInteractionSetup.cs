using Abstractions.Services;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using System.Reflection;

namespace Services.Discord
{
    public class DiscordInteractionSetup : IDiscordInteractionSetup
    {
        private readonly InteractionService _interactionService;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DiscordInteractionSetup> _logger;
        private readonly ulong _guildId;

        public DiscordInteractionSetup(
            InteractionService interactionService,
            DiscordSocketClient client,
            IServiceProvider serviceProvider,
            IOptions<DiscordOptions> options,
            ILogger<DiscordInteractionSetup> logger)
        {
            _interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _guildId = options.Value.GuildId;
        }

        public async Task SetupInteractionsAsync()
        {
            try
            {
                await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);
                await _interactionService.RegisterCommandsToGuildAsync(_guildId);
                _logger.LogInformation("Discord interaction commands registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to setup Discord interactions");
                throw;
            }
        }
    }
}

using Abstractions.Services;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Services.Discord
{
    public class DiscordEventHandler : IDiscordEventHandler
    {
        private readonly InteractionService _interactionService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DiscordEventHandler> _logger;

        public DiscordEventHandler(
            InteractionService interactionService,
            IServiceProvider serviceProvider,
            ILogger<DiscordEventHandler> logger)
        {
            _interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleReadyAsync()
        {
            _logger.LogInformation("Discord client is ready");
            await Task.CompletedTask;
        }

        public async Task HandleInteractionCreatedAsync(SocketInteraction interaction)
        {
            try
            {
                var context = new SocketInteractionContext(_serviceProvider.GetRequiredService<DiscordSocketClient>(), interaction);
                await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing interaction command");
                
                if (interaction.Type == InteractionType.ApplicationCommand)
                {
                    await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => 
                        await interaction.FollowupAsync(text: "An error occurred while processing your command.", ephemeral: true));
                }
            }
        }
    }
}

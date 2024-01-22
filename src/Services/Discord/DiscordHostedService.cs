using Abstractions.Repositories;
using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Models;
using Models.Users;
using Newtonsoft.Json;
using System.Reflection;

namespace Services.Discord
{
    public class DiscordHostedService : BackgroundService
    {
        private readonly IRepository<User> _userRepo;
        private readonly DiscordSocketClient _client;
        private readonly string BotToken;
        private readonly InteractionService _interactionService;
        private readonly IServiceProvider _serviceProvider;
        public DiscordHostedService(IRepository<User> userRepo, DiscordSocketClient client, IOptions<DiscordOptions> options, InteractionService interactionService, IServiceProvider serviceProvider)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(_serviceProvider));
            BotToken = options.Value.Token;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _client.Log += async (msg) =>
            {
                await Task.CompletedTask;
                Console.WriteLine(msg);
            };
            _client.Ready += async () =>
            {
                await Task.CompletedTask;
                Console.WriteLine("Ready!");
                await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);
#if DEBUG
                await _interactionService.RegisterCommandsToGuildAsync(1049366310389289001);
#else
                await _interactionService.RegisterCommandsGloballyAsync();
#endif
            };
            _client.InteractionCreated += async (x) =>
            {
                var ctx = new SocketInteractionContext(_client, x);
                await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
            };


            await _client.LoginAsync(TokenType.Bot, BotToken);

            await _client.StartAsync();

        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}

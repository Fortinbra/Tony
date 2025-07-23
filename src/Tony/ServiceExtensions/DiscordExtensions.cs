using Abstractions.Services;
using Discord.Interactions;
using Discord.WebSocket;
using Models;
using Services.Discord;
using Services.Discord.SlashCommands;

namespace Tony.ServiceExtensions
{
    public static class DiscordExtensions
    {
        public static void AddDiscordBot(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<DiscordOptions>().Configure(options =>
            {
                options.Token = configuration["Discord:Token"];
                options.GuildId = ulong.Parse(configuration["Discord:GuildId"] ?? "1049366310389289001");
            });
            
            // Register Discord client first
            services.AddSingleton<DiscordSocketClient>();
            
            // Register interaction service with proper dependencies
            services.AddSlashCommands();
            
            // Register the new SOLID-compliant services
            services.AddSingleton<IDiscordClientManager, DiscordClientManager>();
            services.AddSingleton<IDiscordEventHandler, DiscordEventHandler>();
            services.AddSingleton<IDiscordInteractionSetup, DiscordInteractionSetup>();
            
            services.AddHostedService<DiscordHostedService>();
        }
        public static void AddSlashCommands(this IServiceCollection services)
        {
            // Register InteractionService with factory to ensure proper construction
            services.AddSingleton<InteractionService>(provider =>
            {
                var client = provider.GetRequiredService<DiscordSocketClient>();
                return new InteractionService(client);
            });

            services.AddSingleton<Bite>();
            services.AddSingleton<DownloadFirmware>();
        }
    }
}

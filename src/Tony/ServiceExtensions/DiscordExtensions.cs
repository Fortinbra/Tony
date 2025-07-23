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
                options.Token = configuration["DiscordToken"];
                options.GuildId = ulong.Parse(configuration["DiscordGuildId"] ?? "1049366310389289001");
            });
            
            services.AddSlashCommands();
            services.AddSingleton<DiscordSocketClient>();
            
            // Register the new SOLID-compliant services
            services.AddSingleton<IDiscordClientManager, DiscordClientManager>();
            services.AddSingleton<IDiscordEventHandler, DiscordEventHandler>();
            services.AddSingleton<IDiscordInteractionSetup, DiscordInteractionSetup>();
            
            services.AddHostedService<DiscordHostedService>();
        }
        public static void AddSlashCommands(this IServiceCollection services)
        {
            var servConfig = new InteractionServiceConfig()
            {
            };
            services.AddSingleton(servConfig);
            services.AddSingleton<InteractionService>();

            services.AddSingleton<Bite>();
        }
    }
}

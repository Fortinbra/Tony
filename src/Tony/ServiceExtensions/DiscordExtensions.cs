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
            });
            services.AddSlashCommands();
            services.AddSingleton<DiscordSocketClient>();

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

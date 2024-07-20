using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Net.Http.Headers;
using Models;
using Services.Discord;
using Services.Discord.SlashCommands;
using System.Net.Http.Headers;

namespace Tony.ServiceExtensions
{
    public static class DiscordExtensions
    {
        public static void AddDiscordBot(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DiscordOptions>(configuration.GetSection("DiscordOptions"));
            services.AddSlashCommands();
            var socketConfig = new DiscordSocketConfig()
            {

            };
            services.AddSingleton(socketConfig);
            services.AddSingleton<DiscordSocketClient>();
            services.AddHostedService<DiscordHostedService>();
        }
        public static void AddSlashCommands(this IServiceCollection services)
        {
            var servConfig = new InteractionServiceConfig()
            {
            };
            services.AddSingleton(servConfig);
            services.AddSingleton<InteractionService>(sp =>
            {
                var client = sp.GetRequiredService<DiscordSocketClient>();
                return new InteractionService(client.Rest, null);
            });
            services.AddSingleton<Yeet>();
            services.AddSingleton<Bite>();
            services.AddSingleton<GetLatest>();
        }

        public static void AddHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient("GitHub", httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://api.github.com/");

                // using Microsoft.Net.Http.Headers;
                // The GitHub API requires two headers.
                httpClient.DefaultRequestHeaders.Add(
                    HeaderNames.Accept, "application/vnd.github.v3+json");
                httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("AppName", "1.0"));
            });
        }

    }
}

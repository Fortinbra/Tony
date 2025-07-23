using Discord.WebSocket;

namespace Abstractions.Services
{
    public interface IDiscordClientManager
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}

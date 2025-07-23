using Discord.WebSocket;

namespace Abstractions.Services
{
    public interface IDiscordEventHandler
    {
        Task HandleReadyAsync();
        Task HandleInteractionCreatedAsync(SocketInteraction interaction);
    }
}

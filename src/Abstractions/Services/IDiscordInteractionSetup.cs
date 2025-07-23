using Discord.Interactions;

namespace Abstractions.Services
{
    public interface IDiscordInteractionSetup
    {
        Task SetupInteractionsAsync();
    }
}

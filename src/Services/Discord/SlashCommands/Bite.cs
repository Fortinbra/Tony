using Discord;
using Discord.Interactions;

namespace Services.Discord.SlashCommands
{
    public class Bite : InteractionModuleBase
    {
        public Bite()
        {
        }

        [SlashCommand("bite", "Bites a user")]
        public async Task BiteAsync(IUser target)
        {
            await RespondAsync($"Biting {target.Mention}");
        }
    }
}

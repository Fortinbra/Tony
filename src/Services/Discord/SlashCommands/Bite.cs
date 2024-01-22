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
            Random random = new();
            if (random.Next(1, 101) <= 30)
            {
                await RespondAsync($"Biting {target.Mention}\n {target.Mention} flinched");
            }

            await RespondAsync($"Biting {target.Mention}");
        }
    }
}

using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Discord.SlashCommands
{
    public class ColorGrantor : InteractionModuleBase
    {
        public ColorGrantor() { }

        [SlashCommand("givecolor", "So you want a color?")]
        public async Task GrantColorAsync()
        {
            var user = Context.User;
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "You wanted a color.");
            if (user is IGuildUser gUser)
            {
                await gUser.AddRoleAsync(role);
            }
            await RespondAsync("Be careful what you wish for.");
        }
    }
}

using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Discord.SlashCommands
{
    public class Yeet : InteractionModuleBase
    {
        public Yeet() { }

        [SlashCommand("yeet", "Yeet!")]
        public async Task YeetAsync()
        {
            await RespondAsync("https://tenor.com/view/yeet-lion-king-simba-rafiki-throw-gif-16194362");
        }
    }
}

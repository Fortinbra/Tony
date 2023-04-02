using Abstractions.Repositories;
using Abstractions.Services;
using Discord;
using Discord.Interactions;
using Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Services.Discord.SlashCommands
{
    public class RegisterUser : InteractionModuleBase
    {
        private readonly IUserService _userService;

        public RegisterUser(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [SlashCommand("register-user", "Registers a User with the Bot.")]
        public async Task RegisterUserAsync(IGuildUser user)
        {
            await DeferAsync();
            try
            {
                var registeredUser = await _userService.CreateUser(new User(user));
                await FollowupAsync($"Congratulations, Tony officially recognizes you, {user.Mention}");
            }
            catch (Exception ex)
            {
                await FollowupAsync($"You've already been recognized.");
            }
        }
    }
}

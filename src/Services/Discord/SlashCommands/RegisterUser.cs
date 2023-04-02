using Abstractions.Repositories;
using Discord;
using Discord.Interactions;
using Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Discord.SlashCommands
{
    public class RegisterUser : InteractionModuleBase
    {
        private readonly IRepository<User> _repository;

        public RegisterUser(IRepository<User> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [SlashCommand("register-user", "Registers a User with the Bot.")]
        public async Task RegisterUserAsync(IGuildUser user)
        {
            await DeferAsync();
            await _repository.CreateAsync(new User()
            {
                AvatarId = user.AvatarId,
                Discriminator = user.Discriminator,
                DiscriminatorValue = user.DiscriminatorValue,
                IsBot = user.IsBot,
                IsWebhook = user.IsWebhook,
                Username = user.Username,
                PublicFlags = user.PublicFlags,
                JoinedAt = user.JoinedAt,
                DisplayName = user.DisplayName,
                Nickname = user.Nickname,
                DisplayAvatarId = user.DisplayAvatarId,
                GuildAvatarId = user.GuildAvatarId,
                GuildId = user.GuildId,
                PremiumSince = user.PremiumSince,
                RoleIds = user.RoleIds.ToList(),
                IsPending = user.IsPending,
                Hierarchy = user.Hierarchy,
                TimedOutUntil = user.TimedOutUntil,
                Flags = user.Flags,
            });
            await FollowupAsync($"Congratulations, Tony officially recognizes you, {user.Mention}");
        }
    }
}

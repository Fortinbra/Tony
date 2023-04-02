#nullable disable
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Users
{
    public class User : Base
    {
        public User() { }
        public User(IGuildUser user)
        {
            DiscordId = user.Id;
            AvatarId = user.AvatarId;
            DiscriminatorValue = user.DiscriminatorValue;
            IsBot = user.IsBot;
            IsWebhook = user.IsWebhook;
            Username = user.Username;
            PublicFlags = user.PublicFlags;
            JoinedAt = user.JoinedAt;
            DisplayName = user.DisplayName;
            Nickname = user.Nickname;
            DisplayAvatarId = user.DisplayAvatarId;
            GuildAvatarId = user.GuildAvatarId;
            GuildId = user.GuildId;
            PremiumSince = user.PremiumSince;
            RoleIds = user.RoleIds.ToList();
            IsPending = user.IsPending;
            Hierarchy = user.Hierarchy;
            TimedOutUntil = user.TimedOutUntil;
            Flags = user.Flags;
        }
        public ulong DiscordId { get; set; }
        public string AvatarId { get; set; }
        public ushort DiscriminatorValue { get; set; }
        public bool IsBot { get; set; }
        public bool IsWebhook { get; set; }
        public string Username { get; set; }
        public UserProperties? PublicFlags { get; set; }
        public DateTimeOffset? JoinedAt { get; set; }
        public string DisplayName { get; set; }
        public string Nickname { get; set; }
        public string DisplayAvatarId { get; set; }
        public string GuildAvatarId { get; set; }
        public ulong GuildId { get; set; }
        public DateTimeOffset? PremiumSince { get; set; }
        public IList<ulong> RoleIds { get; set; }
        public bool? IsPending { get; set; }
        public int Hierarchy { get; set; }
        public DateTimeOffset? TimedOutUntil { get; set; }
        public GuildUserFlags Flags { get; set; }
    }
}

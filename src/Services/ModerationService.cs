using DEA.Common.Extensions.DiscordExtensions;
using DEA.Database.Models;
using DEA.Database.Repositories;
using Discord;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Services
{
    public sealed class ModerationService
    {
        private readonly GuildRepository _guildRepo;

        public ModerationService(GuildRepository guildRepo)
        {
            _guildRepo = guildRepo;
        }

        public int GetPermLevel(Guild dbGuild, IGuildUser user)
        {
            if (user.Guild.OwnerId == user.Id)
            {
                return 3;
            }

            var permLevel = 0;

            if (dbGuild.ModRoles.ElementCount != 0)
            {
                foreach (var role in dbGuild.ModRoles.OrderBy(x => x.Value))
                {
                    if (user.Guild.GetRole(ulong.Parse(role.Name)) != null)
                    {
                        if (user.RoleIds.Any(x => x.ToString() == role.Name))
                        {
                            permLevel = role.Value.AsInt32;
                        }
                    }
                }
            }

            return user.GuildPermissions.Administrator && permLevel < 2 ? 2 : permLevel;
        }

        public async Task<bool> TryInformSubjectAsync(IUser moderator, string action, IUser subject, string reason = null)
        {
            try
            {
                var channel = await subject.CreateDMChannelAsync();
                var message = $"{moderator} has attempted to {action.ToLower()} you.";
                if (!string.IsNullOrWhiteSpace(reason))
                {
                    message = message.Remove(message.Length - 1) + $" for the following reason: \"{reason}\".";
                }

                await channel.SendAsync(message);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> TryModLogAsync(Guild dbGuild, IGuild guild, string action, Color color, string reason = "", IUser moderator = null, IUser subject = null, string extraInfoType = "", string extraInfo = "")
        {
            var channel = await guild.GetTextChannelAsync(dbGuild.ModLogChannelId);

            if (channel == null)
            {
                return false;
            }

            var builder = new EmbedBuilder()
                .WithColor(color)
                .WithFooter(x =>
                {
                    x.IconUrl = "http://i.imgur.com/BQZJAqT.png";
                    x.Text = $"Case #{dbGuild.CaseNumber}";
                })
                .WithCurrentTimestamp();

            if (moderator != null)
            {
                builder.WithAuthor(x =>
                {
                    x.IconUrl = moderator.GetAvatarUrl();
                    x.Name = $"{moderator}";
                });
            }

            var description = $"**Action:** {action}\n";

            if (!string.IsNullOrWhiteSpace(extraInfoType))
            {
                description += $"**{extraInfoType}:** {extraInfo}\n";
            }

            if (subject != null)
            {
                description += $"**User:** {subject} ({subject.Id})\n";
            }

            if (!string.IsNullOrWhiteSpace(reason))
            {
                description += $"**Reason:** {reason}\n";
            }

            builder.WithDescription(description);

            try
            {
                await channel.SendMessageAsync(string.Empty, embed: builder);
                await _guildRepo.ModifyAsync(x=> x.Id == dbGuild.Id, x => x.CaseNumber++);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

using DEA.Common;
using DEA.Common.Extensions.DiscordExtensions;
using DEA.Database.Repositories;
using Discord;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Services
{
    public class ModerationService
    {
        private readonly GuildRepository _guildRepo;

        public ModerationService(GuildRepository guildRepo)
        {
            _guildRepo = guildRepo;
        }

        /// <summary>
        /// Checks whether a user is a moderator.
        /// </summary>
        /// <param name="context">The context to get the guild data information.</param>
        /// <param name="user">The user in question.</param>
        /// <returns>Whether the user is a moderator.</returns>
        public bool IsMod(DEAContext context, IGuildUser user)
        {
            if (user.GuildPermissions.Administrator)
            {
                return true;
            }

            if (context.DbGuild.ModRoles.ElementCount != 0)
            {
                foreach (var role in context.DbGuild.ModRoles)
                {
                    if (user.Guild.GetRole(ulong.Parse(role.Name)) != null)
                    {
                        if (user.RoleIds.Any(x => x.ToString() == role.Name))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if a user has a higher permission level than another user.
        /// </summary>
        /// <param name="context">The context to get the guild data information.</param>
        /// <param name="user1">The first user in question.</param>
        /// <param name="user2">The second user in question.</param>
        /// <returns>Whether the first user has a higher permission level than the second.</returns>
        public bool IsHigherMod(DEAContext context, IGuildUser user1, IGuildUser user2)
        {
            int highest = user1.GuildPermissions.Administrator ? 2 : 0;
            int highestForUser = user2.GuildPermissions.Administrator ? 2 : 0;
            if (context.DbGuild.ModRoles.ElementCount == 0)
            {
                return highest > highestForUser;
            }

            foreach (var role in context.DbGuild.ModRoles.OrderBy(x => x.Value))
            {
                if (user1.Guild.GetRole(ulong.Parse(role.Name)) != null)
                {
                    if (user1.RoleIds.Any(x => x.ToString() == role.Name))
                    {
                        highest = role.Value.AsInt32;
                    }
                }
            }

            foreach (var role in context.DbGuild.ModRoles.OrderBy(x => x.Value))
            {
                if (user2.Guild.GetRole(ulong.Parse(role.Name)) != null)
                {
                    if (user2.RoleIds.Any(x => x.ToString() == role.Name))
                    {
                        highestForUser = role.Value.AsInt32;
                    }
                }
            }

            return highest > highestForUser;
        }

        /// <summary>
        /// Informs a user of an action regarding them including the responsible moderator.
        /// </summary>
        /// <param name="moderator">The moderator in question.</param>
        /// <param name="action">The action.</param>
        /// <param name="subject">The user in question.</param>
        /// <param name="reason">The reason for the action.</param>
        public async Task InformSubjectAsync(IUser moderator, string action, IUser subject, string reason = null)
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
            }
            catch { }
        }

        /// <summary>
        /// If the moderation log channel exists, it will log all moderation commands.
        /// </summary>
        /// <param name="context">The context of the command use.</param>
        /// <param name="action">The action that was taken.</param>
        /// <param name="color">The color of the embed.</param>
        /// <param name="reason">The reason for the action.</param>
        /// <param name="subject">The user in question.</param>
        /// <param name="extra">An extra line for more information.</param>
        public async Task ModLogAsync(DEAContext context, string action, Color color, string reason, IUser subject = null, string extra = "")
        {
            var channel = context.Guild.GetTextChannel(context.DbGuild.ModLogChannelId);

            if (channel == null)
            {
                return;
            }

            EmbedFooterBuilder footer = new EmbedFooterBuilder()
            {
                IconUrl = "http://i.imgur.com/BQZJAqT.png",
                Text = $"Case #{context.DbGuild.CaseNumber}"
            };
            EmbedAuthorBuilder author = new EmbedAuthorBuilder()
            {
                IconUrl = context.User.GetAvatarUrl(),
                Name = $"{context.User.Username}#{context.User.Discriminator}"
            };

            string userText = string.Empty;
            if (subject != null)
            {
                userText = $"\n**User:** {subject} ({subject.Id})";
            }

            var description = $"**Action:** {action}{extra}{userText}";
            if (reason != null)
            {
                description += $"\n**Reason:** {reason}";
            }

            var builder = new EmbedBuilder()
            {
                Author = author,
                Color = color,
                Description = description,
                Footer = footer
            }.WithCurrentTimestamp();

            try
            {
                await channel.SendMessageAsync(string.Empty, embed: builder);
                await _guildRepo.ModifyAsync(context.Guild.Id, x => x.CaseNumber, ++context.DbGuild.CaseNumber);
            }
            catch { }
        }
    }
}

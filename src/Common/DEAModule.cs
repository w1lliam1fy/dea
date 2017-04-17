using DEA.Database.Models;
using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Common
{
    public abstract class DEAModule : ModuleBase<DEAContext>
    {

        public async Task<IUserMessage> Reply( string description, string title = null, Color color = default(Color))
        {
            var rand = new Random();
            var builder = new EmbedBuilder()
            {
                Description = $"{await NameAsync()}, {description}",
                Color = Config.COLORS[rand.Next(1, Config.COLORS.Length) - 1]
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            return await ReplyAsync(string.Empty, embed: builder);
        }

        public Task<IUserMessage> Send(string description, string title = null, Color color = default(Color))
        {
            var rand = new Random();
            var builder = new EmbedBuilder()
            {
                Description = description,
                Color = Config.COLORS[rand.Next(1, Config.COLORS.Length) - 1]
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            return ReplyAsync(string.Empty, embed: builder);
        }

        public Task<string> NameAsync()
        {
            var user = Context.User as IGuildUser;
            if (string.IsNullOrWhiteSpace(Context.DbUser.Name))
                return Task.FromResult((string.IsNullOrWhiteSpace(user.Nickname)) ? $"**{user.Username}**" : $"**{user.Nickname}**");
            else
                return Task.FromResult($"**{Context.DbUser.Name}**");
        }

        public Task<string> TitleNameAsync()
        {
            var user = Context.User as IGuildUser;
            if (string.IsNullOrWhiteSpace(Context.DbUser.Name))
                return Task.FromResult((string.IsNullOrWhiteSpace(user.Nickname)) ? user.Username : user.Nickname);
            else
                return Task.FromResult(Context.DbUser.Name);
        }

        public Task<string> NameAsync(IGuildUser user, User dbUser)
        {
            if (string.IsNullOrWhiteSpace(dbUser.Name))
                return Task.FromResult(string.IsNullOrWhiteSpace(user.Nickname) ? $"**{user.Username}**" : $"**{user.Nickname}**");
            else
                return Task.FromResult($"**{dbUser.Name}**");
        }

        public Task<string> TitleNameAsync(IGuildUser user, User dbUser)
        {
            if (string.IsNullOrWhiteSpace(dbUser.Name))
                return Task.FromResult(string.IsNullOrWhiteSpace(user.Nickname) ? user.Username : user.Nickname);
            else
                return Task.FromResult(dbUser.Name);
        }

        public Task<bool> IsModAsync(DEAContext context, IGuildUser user)
        {
            if (user.GuildPermissions.Administrator) return Task.FromResult(true);
            if (context.DbGuild.ModRoles.ElementCount != 0)
                foreach (var role in context.DbGuild.ModRoles)
                    if (user.Guild.GetRole(Convert.ToUInt64(role.Name)) != null)
                        if (user.RoleIds.Any(x => x.ToString() == role.Name)) return Task.FromResult(true);
            return Task.FromResult(false);
        }

        public Task<bool> IsHigherModAsync(DEAContext context, IGuildUser mod, IGuildUser user)
        {
            int highest = mod.GuildPermissions.Administrator ? 2 : 0;
            int highestForUser = user.GuildPermissions.Administrator ? 2 : 0;
            if (context.DbGuild.ModRoles.ElementCount == 0) return Task.FromResult(highest > highestForUser);

            foreach (var role in context.DbGuild.ModRoles.OrderBy(x => x.Value))
                if (mod.Guild.GetRole(Convert.ToUInt64(role.Name)) != null)
                    if (mod.RoleIds.Any(x => x.ToString() == role.Name)) highest = role.Value.AsInt32;

            foreach (var role in context.DbGuild.ModRoles.OrderBy(x => x.Value))
                if (user.Guild.GetRole(Convert.ToUInt64(role.Name)) != null)
                    if (user.RoleIds.Any(x => x.ToString() == role.Name)) highestForUser = role.Value.AsInt32;

            return Task.FromResult(highest > highestForUser);
        }

        public Task<IUserMessage> DM(IDMChannel channel, string description, string title = null, Color color = default(Color))
        {
            var rand = new Random();
            var builder = new EmbedBuilder()
            {
                Description = description,
                Color = Config.COLORS[rand.Next(1, Config.COLORS.Length) - 1]
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            return channel.SendMessageAsync(string.Empty, embed: builder);
        }

        public async Task<IUserMessage> DM(ulong userId, string description, string title = null, Color color = default(Color))
        {
            var user = Context.Guild.GetUser(userId);

            if (user != null)
            {
                try
                {
                    var channel = await user.CreateDMChannelAsync();

                    var rand = new Random();
                    var builder = new EmbedBuilder()
                    {
                        Description = description,
                        Color = Config.COLORS[rand.Next(1, Config.COLORS.Length) - 1]
                    };
                    if (title != null) builder.Title = title;
                    if (color.RawValue != default(Color).RawValue) builder.Color = color;

                    return await channel.SendMessageAsync(string.Empty, embed: builder);
                }
                catch { }
            }
            return null;
        }

        public async Task InformSubjectAsync(IUser moderator, string action, IUser subject, string reason)
        {
            try
            {
                var channel = await subject.CreateDMChannelAsync();
                if (reason == "No reason.")
                    await DM(channel, $"{moderator} has attempted to {action.ToLower()} you.");
                else
                    await DM(channel, $"{moderator} has attempted to {action.ToLower()} you for the following reason: \"{reason}\"");
            }
            catch { }
        }

        public Task ErrorAsync(string message)
        {
            throw new DEAException(message);
        }
    }
}

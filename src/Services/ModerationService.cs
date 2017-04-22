using DEA.Common;
using DEA.Common.Extensions.DiscordExtensions;
using Discord;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Services
{
    public class ModerationService
    {
        public Task<bool> IsModAsync(DEAContext context, IGuildUser user)
        {
            if (user.GuildPermissions.Administrator) return Task.FromResult(true);
            if (context.DbGuild.ModRoles.ElementCount != 0)
                foreach (var role in context.DbGuild.ModRoles)
                    if (user.Guild.GetRole(ulong.Parse(role.Name)) != null)
                        if (user.RoleIds.Any(x => x.ToString() == role.Name)) return Task.FromResult(true);
            return Task.FromResult(false);
        }

        public Task<bool> IsHigherModAsync(DEAContext context, IGuildUser mod, IGuildUser user)
        {
            int highest = mod.GuildPermissions.Administrator ? 2 : 0;
            int highestForUser = user.GuildPermissions.Administrator ? 2 : 0;
            if (context.DbGuild.ModRoles.ElementCount == 0) return Task.FromResult(highest > highestForUser);

            foreach (var role in context.DbGuild.ModRoles.OrderBy(x => x.Value))
                if (mod.Guild.GetRole(ulong.Parse(role.Name)) != null)
                    if (mod.RoleIds.Any(x => x.ToString() == role.Name)) highest = role.Value.AsInt32;

            foreach (var role in context.DbGuild.ModRoles.OrderBy(x => x.Value))
                if (user.Guild.GetRole(ulong.Parse(role.Name)) != null)
                    if (user.RoleIds.Any(x => x.ToString() == role.Name)) highestForUser = role.Value.AsInt32;

            return Task.FromResult(highest > highestForUser);
        }

        public async Task InformSubjectAsync(IUser moderator, string action, IUser subject, string reason = "")
        {
            var channel = await subject.CreateDMChannelAsync();

            if (channel != null)
            {
                var message = $"{moderator} has attempted to {action.ToLower()} you.";
                if (!string.IsNullOrWhiteSpace(reason))
                    message = message.Remove(message.Length - 1) + $"for the following reason: {reason}";
                await channel.SendAsync(message);
            }
        }
    }
}

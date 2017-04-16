using DEA.Services;
using DEA.Database.Repository;
using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using DEA.Services.Handlers;

namespace DEA.Events
{
    class UserEvents
    {

        public UserEvents()
        {
            DEABot.Client.UserJoined += HandleUserJoin;
            DEABot.Client.UserBanned += HandleUserBanned;
            DEABot.Client.UserLeft += HandleUserLeft;
            DEABot.Client.UserUnbanned += HandleUserUnbanned;
        }

        private async Task HandleUserJoin(SocketGuildUser u)
        {
            await Logger.DetailedLog(u.Guild, "Event", "User Joined", "User", $"{u}", u.Id, new Color(12, 255, 129), false);
            var user = u as IGuildUser;
            var mutedRole = user.Guild.GetRole(((await GuildRepository.FetchGuildAsync(user.Guild.Id)).MutedRoleId));
            if (mutedRole != null && u.Guild.CurrentUser.GuildPermissions.ManageRoles &&
                mutedRole.Position < u.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
            {
                await RankHandler.HandleAsync(u.Guild, u.Id);
                if (await MuteRepository.IsMutedAsync(user.Id, user.Guild.Id) && mutedRole != null && user != null) await user.AddRoleAsync(mutedRole);
            }
        }

        private async Task HandleUserBanned(SocketUser u, SocketGuild guild)
        {
            await Logger.DetailedLog(guild, "Action", "Ban", "User", $"{u}", u.Id, new Color(255, 0, 0));
        }

        private async Task HandleUserLeft(SocketGuildUser u)
        {
            await Logger.DetailedLog(u.Guild, "Event", "User Left", "User", $"{u}", u.Id, new Color(255, 114, 14));
        }

        private async Task HandleUserUnbanned(SocketUser u, SocketGuild guild)
        {
            await Logger.DetailedLog(guild, "Action", "Unban", "User", $"<@{u.Id}>", u.Id, new Color(12, 255, 129));
        }
    }
}

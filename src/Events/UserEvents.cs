using DEA.Services;
using DEA.Database.Repository;
using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using DEA.Services.Handlers;
using Discord.Commands;

namespace DEA.Events
{
    class UserEvents
    {
        private IDependencyMap _map;
        private DiscordSocketClient _client;
        private LoggingService _loggingService;
        private UserRepository _userRepo;
        private GuildRepository _guildRepo;
        private MuteRepository _muteRepo;
        private RankingService _rankingService;

        public UserEvents(IDependencyMap map)
        {
            _map = map;
            _loggingService = _map.Get<LoggingService>();
            _userRepo = _map.Get<UserRepository>();
            _guildRepo = map.Get<GuildRepository>();
            _muteRepo = map.Get<MuteRepository>();
            _rankingService = map.Get<RankingService>();
            _client = _map.Get<DiscordSocketClient>();
            _client.UserJoined += HandleUserJoin;
            _client.UserBanned += HandleUserBanned;
            _client.UserLeft += HandleUserLeft;
            _client.UserUnbanned += HandleUserUnbanned;
        }

        private  Task HandleUserJoin(SocketGuildUser u)
        {
            return Task.Run(async () =>
            {
                await _loggingService.DetailedLogAsync(u.Guild, "Event", "User Joined", "User", $"{u}", u.Id, new Color(12, 255, 129), false);
                var user = u as IGuildUser;
                var mutedRole = user.Guild.GetRole(((await _guildRepo.FetchGuildAsync(user.Guild.Id)).MutedRoleId));
                if (mutedRole != null && u.Guild.CurrentUser.GuildPermissions.ManageRoles &&
                    mutedRole.Position < u.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                {
                    await _rankingService.HandleAsync(u.Guild, user, await _guildRepo.FetchGuildAsync(u.Guild.Id), await _userRepo.FetchUserAsync(user));
                    if (await _muteRepo.IsMutedAsync(user.Id, user.Guild.Id) && mutedRole != null && user != null) await user.AddRoleAsync(mutedRole);
                }
            });
        }

        private Task HandleUserBanned(SocketUser u, SocketGuild guild)
            => Task.Run(async () => await _loggingService.DetailedLogAsync(guild, "Action", "Ban", "User", $"{u}", u.Id, new Color(255, 0, 0)));

        private Task HandleUserLeft(SocketGuildUser u)
            => Task.Run(async () => await _loggingService.DetailedLogAsync(u.Guild, "Event", "User Left", "User", $"{u}", u.Id, new Color(255, 114, 14)));

        private Task HandleUserUnbanned(SocketUser u, SocketGuild guild)
            => Task.Run(async () => await _loggingService.DetailedLogAsync(guild, "Action", "Unban", "User", $"<@{u.Id}>", u.Id, new Color(12, 255, 129)));
    }
}

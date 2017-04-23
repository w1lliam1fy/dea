using DEA.Services;
using DEA.Database.Repositories;
using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using DEA.Services.Handlers;
using Discord.Commands;
using DEA.Services.Static;

namespace DEA.Events
{
    class UserEvents
    {
        private readonly IDependencyMap _map;
        private readonly DiscordSocketClient _client;
        private readonly LoggingService _loggingService;
        private readonly UserRepository _userRepo;
        private readonly GuildRepository _guildRepo;
        private readonly MuteRepository _muteRepo;
        private readonly RankHandler _rankHandler;

        public UserEvents(IDependencyMap map)
        {
            _map = map;
            _loggingService = _map.Get<LoggingService>();
            _userRepo = _map.Get<UserRepository>();
            _guildRepo = map.Get<GuildRepository>();
            _muteRepo = map.Get<MuteRepository>();
            _rankHandler = map.Get<RankHandler>();
            _client = _map.Get<DiscordSocketClient>();
            _client.UserJoined += HandleUserJoin;
            _client.UserBanned += HandleUserBanned;
            _client.UserLeft += HandleUserLeft;
            _client.UserUnbanned += HandleUserUnbanned;
        }

        private Task HandleUserJoin(SocketGuildUser u)
            => Task.Run(async () =>
            {
                await _loggingService.DetailedLogAsync(u.Guild, "Event", "User Joined", "User", $"{u}", u.Id, new Color(12, 255, 129), false);
                var user = u as IGuildUser;
                var dbGuild = await _guildRepo.FetchGuildAsync(user.Guild.Id);
                var mutedRole = user.Guild.GetRole((dbGuild.MutedRoleId));
                if (mutedRole != null && u.Guild.CurrentUser.GuildPermissions.ManageRoles &&
                    mutedRole.Position < u.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                {
                    await _rankHandler.HandleAsync(u.Guild, user, dbGuild, await _userRepo.FetchUserAsync(user));
                    if (await _muteRepo.IsMutedAsync(user.Id, user.Guild.Id) && mutedRole != null && user != null) await user.AddRoleAsync(mutedRole);
                }
            });

        private Task HandleUserBanned(SocketUser u, SocketGuild guild)
            => Task.Run(() => _loggingService.DetailedLogAsync(guild, "Action", "Ban", "User", $"{u}", u.Id, Config.ERROR_COLOR));

        private Task HandleUserLeft(SocketGuildUser u)
            => Task.Run(() => _loggingService.DetailedLogAsync(u.Guild, "Event", "User Left", "User", $"{u}", u.Id, new Color(255, 114, 14)));

        private Task HandleUserUnbanned(SocketUser u, SocketGuild guild)
            => Task.Run(() => _loggingService.DetailedLogAsync(guild, "Action", "Unban", "User", $"<@{u.Id}>", u.Id, new Color(12, 255, 129)));
    }
}

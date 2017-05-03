using DEA.Database.Models;
using DEA.Database.Repositories;
using DEA.Services.Static;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Events
{
    class UpdatedGuild
    {
        private readonly IDependencyMap _map;
        private readonly DiscordSocketClient _client;
        private readonly BlacklistRepository _blaclistRepo;

        public UpdatedGuild(IDependencyMap map)
        {
            _map = map;
            _client = _map.Get<DiscordSocketClient>();
            _blaclistRepo = _map.Get<BlacklistRepository>();
            _client.GuildUpdated += HandleGuildUpdated;
        }

        private Task HandleGuildUpdated(SocketGuild guildBefore, SocketGuild guildAfter)
        {
            return Task.Run(async () =>
            {
                Logger.Log(LogSeverity.Debug, $"Event", "Guild Updated");
                var blacklists = await _blaclistRepo.AllAsync();

                var isBlacklistedOwner = blacklists.Any(x => x.UserId == guildAfter.OwnerId);

                var isBlacklistedGuild = blacklists.Any(x => x.GuildIds.Any(y => y == guildAfter.Id));

                if (isBlacklistedOwner && !isBlacklistedGuild)
                {
                    await _blaclistRepo.AddGuildAsync(guildAfter.OwnerId, guildAfter.Id);
                    await guildAfter.LeaveAsync();
                }
                else if (isBlacklistedGuild && !isBlacklistedOwner)
                {
                    await _blaclistRepo.InsertAsync(new Blacklist(guildAfter.OwnerId));
                    await guildAfter.LeaveAsync();
                }
                else if (isBlacklistedOwner && isBlacklistedGuild)
                {
                    await guildAfter.LeaveAsync();
                }
            });
        }
    }
}

using DEA.Database.Repositories;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Events
{
    class JoinedGuild
    {
        private readonly IDependencyMap _map;
        private readonly DiscordSocketClient _client;
        private readonly BlacklistRepository _blaclistRepo;

        public JoinedGuild(IDependencyMap map)
        {
            _map = map;
            _client = _map.Get<DiscordSocketClient>();
            _blaclistRepo = _map.Get<BlacklistRepository>();
            _client.JoinedGuild += HandleJoinedGuild;
        }

        private Task HandleJoinedGuild(SocketGuild guild)
        {
            return Task.Run(async () =>
            {
                var blacklists = await _blaclistRepo.AllAsync();

                var isBlacklistedGuild = blacklists.Any(x => x.GuildIds.Any(y => y == guild.Id));

                foreach (var guildUser in (await (guild as IGuild).GetUsersAsync()).Where(x => blacklists.Any(y => y.UserId == x.Id)))
                {
                    try
                    {
                        await guild.AddBanAsync(guildUser);
                    }
                    catch
                    {
                        if (guild.OwnerId == guildUser.Id && !isBlacklistedGuild)
                        {
                            await _blaclistRepo.AddGuildAsync(guildUser.Id, guild.Id);
                            await guild.LeaveAsync();
                        }
                    }
                }

                if (isBlacklistedGuild)
                {
                    await guild.LeaveAsync();
                }
            });
        }
    }
}

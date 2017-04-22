using DEA.Database.Models;
using DEA.Database.Repositories;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Common
{
    public class DEAContext : SocketCommandContext
    {
        public User DbUser { get; private set; }
        public Guild DbGuild { get; private set; }
        public Gang Gang { get; private set; }
        public string Prefix { get; private set; }
        public decimal Cash { get; private set; }

        private readonly IDependencyMap _map;
        private readonly UserRepository _userRepo;
        private readonly GuildRepository _guildRepo;
        private readonly GangRepository _gangRepo;

        public DEAContext(DiscordSocketClient client, SocketUserMessage msg, IDependencyMap map) : base(client, msg)
        {
            _map = map;
            _userRepo = _map.Get<UserRepository>();
            _guildRepo = _map.Get<GuildRepository>();
            _gangRepo = _map.Get<GangRepository>();
        }

        public async Task InitializeAsync()
        {
            DbUser = await _userRepo.FetchUserAsync(this);
            DbGuild = await _guildRepo.FetchGuildAsync(Guild.Id);
            if (await _gangRepo.InGangAsync(User as IGuildUser))
                Gang = await _gangRepo.FetchGangAsync(this);
            Prefix = DbGuild.Prefix;
            Cash = DbUser.Cash;
        }
    }
}

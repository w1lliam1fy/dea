using DEA.Database.Models;
using DEA.Database.Repository;
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

        private IDependencyMap _map;
        private UserRepository _userRepo;
        private GuildRepository _guildRepo;
        private GangRepository _gangRepo;

        public DEAContext(DiscordSocketClient client, SocketUserMessage msg, IDependencyMap map) : base(client, msg)
        {
            _map = map;
            _userRepo = map.Get<UserRepository>();
            _guildRepo = map.Get<GuildRepository>();
            _gangRepo = map.Get<GangRepository>();
        }

        public async Task InitializeAsync()
        {
            DbUser = await _userRepo.FetchUserAsync(User as IGuildUser);
            DbGuild = await _guildRepo.FetchGuildAsync(Guild.Id);
            if (await _gangRepo.InGangAsync(User as IGuildUser))
                Gang = await _gangRepo.FetchGangAsync(User as IGuildUser);
            Prefix = DbGuild.Prefix;
            Cash = DbUser.Cash;
        }
    }
}

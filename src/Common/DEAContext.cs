using DEA.Database.Models;
using DEA.Database.Repositories;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Common
{
    /// <summary>
    /// Custom context containing the guild user of the command user and the data information of the guild and the user.
    /// </summary>
    public class DEAContext : SocketCommandContext
    {
        public IGuildUser GUser { get; }
        public User DbUser { get; private set; }
        public Guild DbGuild { get; private set; }
        public Gang Gang { get; private set; }
        public string Prefix { get; private set; }
        public decimal Cash { get; set; }

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
            GUser = User as IGuildUser;
        }

        /// <summary>
        /// Fetches the guild and user database information for the custom context object.
        /// </summary>
        public async Task InitializeAsync()
        {
            DbUser = await _userRepo.FetchUserAsync(GUser);
            DbGuild = await _guildRepo.FetchGuildAsync(Guild.Id);
            if (await _gangRepo.InGangAsync(GUser))
            {
                Gang = await _gangRepo.FetchGangAsync(GUser);
            }
            Prefix = DbGuild.Prefix;
            Cash = DbUser.Cash;
        }
    }
}

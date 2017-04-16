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

        public DEAContext(DiscordSocketClient client, SocketUserMessage msg) : base(client, msg)
        { 
        }

        public async Task InitializeAsync()
        {
            DbUser = await UserRepository.FetchUserAsync(User as IGuildUser);
            DbGuild = await GuildRepository.FetchGuildAsync(Guild.Id);
            if (await GangRepository.InGangAsync(User as IGuildUser))
                Gang = await GangRepository.FetchGangAsync(User as IGuildUser);
            Prefix = DbGuild.Prefix;
            Cash = DbUser.Cash;
        }
    }
}

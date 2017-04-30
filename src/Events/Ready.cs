using DEA.Common.Extensions.DiscordExtensions;
using DEA.Database.Models;
using Discord.Commands;
using Discord.WebSocket;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace DEA.Events
{
    class Ready
    {
        private readonly IDependencyMap _map;
        private readonly IMongoCollection<Guild> _guilds;
        private readonly DiscordSocketClient _client;

        public Ready(IDependencyMap map)
        {
            _map = map;
            _client = _map.Get<DiscordSocketClient>();
            _guilds = _map.Get<IMongoCollection<Guild>>();
            _client.Ready += HandleReady;
        }

        private Task HandleReady()
        {
            return Task.Run(async () => 
            {
                foreach (var guild in _client.Guilds)
                {
                    try
                    {
                        var channel = await guild.Owner.CreateDMChannelAsync();
                        await channel.SendAsync("Hey! It has come to my attention that you have me added as a bot in one of your servers, " +
                                                "and I would like to say, thanks!\n\nIncase you wanted an easier way to keep up with all the" +
                                                "new features that are coming out with every DEA update, there is now a command for that!\n\n" +
                                                "With the `$SetUpdateChannel #channel` command, you can set a channel in which DEA will explain " +
                                                "each update right when it comes out! Don't worry though, this is fully optional if you aren't " +
                                                "interested.");
                    }
                    catch { }
                }
                await _client.SetGameAsync("USE $help");
            });
        }
    }
}

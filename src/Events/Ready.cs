using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    class Ready
    {
        private readonly IDependencyMap _map;
        private readonly DiscordSocketClient _client;

        public Ready(IDependencyMap map)
        {
            _map = map;
            _client = _map.Get<DiscordSocketClient>();
            _client.Ready += HandleReady;
        }

        private Task HandleReady()
        {
            return Task.Run(async () =>
            {
                await _client.SetGameAsync("USE $help");            
            });
        }
    }
}

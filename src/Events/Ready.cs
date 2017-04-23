using DEA.Services.Timers;
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

        private async Task HandleReady()
        {
            await _client.SetGameAsync("USE $help");
            new UserEvents(_map);
            new ApplyIntrestRate(_map);
            new AutoDeletePolls(_map);
            new AutoTrivia(_map);
            new AutoUnmute(_map);
            new ResetTempMultiplier(_map);
        }

    }
}

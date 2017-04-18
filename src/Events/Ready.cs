using DEA.Services.Timers;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    class Ready
    {
        private IDependencyMap _map;
        private DiscordSocketClient _client;

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
            new RoleEvents(_map);
            new ChannelEvents(_map);
            new ApplyIntrestRate(_map);
            new AutoTrivia(_map);
            new AutoInvite(_map);
            new AutoUnmute(_map);
            new ResetTempMultiplier(_map);
        }

    }
}

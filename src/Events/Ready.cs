using DEA.Services.Static;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    /// <summary>
    /// An event that is run when DEA is ready to access the Discord API Client.
    /// </summary>
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
                Logger.Log(LogSeverity.Debug, $"Event", "Ready");
                await _client.SetGameAsync("USE $help");            
            });
        }
    }
}

using DEA.Services.Static;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;
using System.Threading.Tasks;
using System;
using Discord.Commands;

namespace DEA.Events
{
    class Ready
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CommandService _commandService;
        private readonly DiscordSocketClient _client;

        public Ready(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _commandService = _serviceProvider.GetService<CommandService>();
            _client = _serviceProvider.GetService<DiscordSocketClient>();
            _client.Ready += HandleReady;
        }

        private Task HandleReady()
        {
            return Task.Run(async () =>
            {
                Logger.Log(LogSeverity.Debug, $"Event", "Ready");
                await _client.SetGameAsync("USE $help");
                Documentation.CreateAndSave(_commandService);
            });
        }
    }
}

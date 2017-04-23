using DEA.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    class ChannelEvents
    {
        private readonly IDependencyMap _map;
        private readonly DiscordSocketClient _client;
        private readonly LoggingService _loggingService;

        public ChannelEvents(IDependencyMap map)
        {
            _map = map;
            _client = _map.Get<DiscordSocketClient>();
            _loggingService = _map.Get<LoggingService>();
            _client.ChannelCreated += HandleChannelCreated;
            _client.ChannelDestroyed += HandleChannelDestroyed;
            _client.ChannelUpdated += HandleChannelUpdated;
        }

        private Task HandleChannelCreated(SocketChannel socketChannel)
            => Task.Run(() => _loggingService.DetailedLogsForChannel(socketChannel, "Channel Creation", new Color(12, 255, 129)));

        private Task HandleChannelUpdated(SocketChannel socketChannelBefore, SocketChannel socketChannelAfter)
            => Task.Run(() => _loggingService.DetailedLogsForChannel(socketChannelAfter, "Channel Modification", new Color(12, 255, 129)));

        private Task HandleChannelDestroyed(SocketChannel socketChannel)
            => Task.Run(() => _loggingService.DetailedLogsForChannel(socketChannel, "Channel Deletion", Config.ERROR_COLOR));


    }
}

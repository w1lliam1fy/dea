using DEA.Database.Repositories;
using DEA.Services.Static;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Services.Timers
{
    /// <summary>
    /// Periodically sends out trivia messages and awaits for the answers in guilds with auto trivia enabled.
    /// </summary>
    class AutoTrivia
    { 
        private readonly IDependencyMap _map;
        private readonly GuildRepository _guildRepo;
        private readonly DiscordSocketClient _client;
        private readonly UserRepository _userRepo;
        private readonly GameService _gameService;
        private readonly InteractiveService _interactiveService;

        private readonly Timer _timer;

        public AutoTrivia(IDependencyMap map)
        {
            _map = map;
            _guildRepo = _map.Get<GuildRepository>();
            _userRepo = map.Get<UserRepository>();
            _client = map.Get<DiscordSocketClient>();
            _gameService = map.Get<GameService>();
            _interactiveService = map.Get<InteractiveService>();

            ObjectState StateObj = new ObjectState();

            TimerCallback TimerDelegate = new TimerCallback(Trivia);

            _timer = new Timer(TimerDelegate, StateObj, TimeSpan.FromMilliseconds(750), Config.AUTO_TRIVIA_COOLDOWN);

            StateObj.TimerReference = _timer;
        }

        private void Trivia(object stateObj)
        {
            Task.Run(async () =>
            {
                Logger.Log(LogSeverity.Debug, $"Timers", "Auto Trivia");
                foreach (var dbGuild in await _guildRepo.AllAsync())
                {
                    if (dbGuild.AutoTrivia)
                    {
                        var guild = _client.GetGuild(dbGuild.GuildId);
                        if (guild != null)
                        {
                            var defaultChannel = guild.DefaultChannel;
                            try
                            {
                                await _gameService.TriviaAsync(defaultChannel, dbGuild);
                            }
                            catch { }
                        }
                    }
                }
            });
        }
    }
}
using DEA.Database.Models;
using DEA.Database.Repository;
using Discord.Commands;
using Discord.WebSocket;
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Services.Timers
{
    class AutoTrivia
    { 
        private readonly IDependencyMap _map;
        private readonly IMongoCollection<Guild> _guilds;
        private readonly DiscordSocketClient _client;
        private readonly UserRepository _userRepo;
        private readonly GameService _gameService;
        private readonly InteractiveService _interactiveService;

        private readonly Timer _timer;

        public AutoTrivia(IDependencyMap map)
        {
            _map = map;
            _guilds = _map.Get<IMongoCollection<Guild>>();
            _userRepo = map.Get<UserRepository>();
            _client = map.Get<DiscordSocketClient>();
            _gameService = map.Get<GameService>();
            _interactiveService = map.Get<InteractiveService>();

            ObjectState StateObj = new ObjectState();

            TimerCallback TimerDelegate = new TimerCallback(Trivia);

            _timer = new Timer(TimerDelegate, StateObj, 0, Config.AUTO_TRIVIA_COOLDOWN);

            StateObj.TimerReference = _timer;
        }

        private void Trivia(object stateObj)
        {
            Task.Run(async () =>
            {
                var builder = Builders<Guild>.Filter;
                foreach (var dbGuild in await (await _guilds.FindAsync(builder.Empty)).ToListAsync())
                {
                    if (dbGuild.AutoTrivia)
                    {
                        var guild = _client.GetGuild(dbGuild.Id);
                        var defaultChannel = guild.DefaultChannel;
                        if (guild != null)
                        {
                            try
                            {
                                await _gameService.Trivia(defaultChannel, dbGuild);
                            }
                            catch { }
                        }
                    }
                }
            });
        }
    }
}
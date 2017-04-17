using DEA.Common;
using DEA.Database.Models;
using DEA.Database.Repository;
using DEA.Events;
using DEA.Services;
using DEA.Services.Handlers;
using DEA.Services.Static;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DEA
{
    public class DEABot
    {
        private DiscordSocketClient _client;
        private CommandService _commandService;

        private Credentials _credentials;
        private IDependencyMap _map;

        private GamblingService _gamblingService;
        private InteractiveService _interactiveService;
        private ErrorHandler _errorHandler;
        private ResponseService _responseService;
        private LoggingService _loggingService;
        private RankingService _rankingService;

        private MongoClient _dbClient;
        private IMongoDatabase _database;

        private IMongoCollection<Guild> _guilds;
        private IMongoCollection<User> _users;
        private IMongoCollection<Gang> _gangs;
        private IMongoCollection<Mute> _mutes;

        private GuildRepository _guildRepo;
        private UserRepository _userRepo;
        private GangRepository _gangRepo;
        private MuteRepository _muteRepo;

        public static int Commands { get; set; }
        public static int Messages { get; set; }

        public DEABot()
        {
            try
            {
                using (StreamReader file = File.OpenText(@"Credentials.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    _credentials = (Credentials)serializer.Deserialize(file, typeof(Credentials));
                }
            }
            catch (IOException e)
            {
                Logger.LogAsync(LogSeverity.Critical, "Error while loading up Credentials.json, please fix this issue and restart the bot", e.Message).RunSynchronously();
                Console.ReadLine();
                Environment.Exit(0);
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                MessageCacheSize = 10,
                TotalShards = _credentials.ShardCount,
            });

            _commandService = new CommandService(new CommandServiceConfig()
            {
                CaseSensitiveCommands = false,
                LogLevel = LogSeverity.Debug,
                DefaultRunMode = RunMode.Async,
            });

            _dbClient = new MongoClient(_credentials.MongoDBConnectionString);
            _database = _dbClient.GetDatabase(_credentials.DatabaseName);

            _guilds = _database.GetCollection<Guild>("guilds");
            _users = _database.GetCollection<User>("users");
            _gangs = _database.GetCollection<Gang>("gangs");
            _mutes = _database.GetCollection<Mute>("mutes");

            _guildRepo = new GuildRepository(_guilds);
            _rankingService = new RankingService(_guildRepo);
            _userRepo = new UserRepository(_users, _rankingService);
            _gangRepo = new GangRepository(_gangs);
            _muteRepo = new MuteRepository(_mutes);

            _interactiveService = new InteractiveService(_client);
            _responseService = new ResponseService();
            _errorHandler = new ErrorHandler(_commandService, _responseService);
            _gamblingService = new GamblingService(_userRepo, _responseService);
            _loggingService = new LoggingService(_guildRepo, _responseService);
        }

        private async Task RunAsync(params string[] args)
        {
            await Logger.NewLine("===   DEA   ===");
            await Logger.NewLine();

            var sw = Stopwatch.StartNew();
            try
            {
                await _client.LoginAsync(TokenType.Bot, _credentials.Token);
            }
            catch (HttpException httpEx)
            {
                await Logger.LogAsync(LogSeverity.Critical, $"Login failed", httpEx.Reason);
                Console.ReadLine();
                Environment.Exit(0);
            }
               
            await _client.StartAsync().ConfigureAwait(false);
            sw.Stop();
            await Logger.LogAsync(LogSeverity.Info, "Successfully connected", $"Elapsed time: {sw.Elapsed.TotalSeconds.ToString()} seconds.");

            _map = new DependencyMap();
            ConfigureServices(_map);
            
            await Logger.LogAsync(LogSeverity.Info, "Mapping successfully configured", $"Services ready.");

            new Ready(_map);
            await new CommandHandler(_commandService, _map).InitializeAsync();
            await Logger.LogAsync(LogSeverity.Info, "Events and command handler successfully initialized", $"Client ready.");
        }

        public async Task RunAndBlockAsync(params string[] args)
        {
            await RunAsync(args).ConfigureAwait(false);
            await Task.Delay(-1).ConfigureAwait(false);
        }

        public void ConfigureServices(IDependencyMap map)
        {
            map.Add(_client);
            map.Add(_credentials);
            map.Add(_dbClient);
            map.Add(_database);
            map.Add(_guilds);
            map.Add(_users);
            map.Add(_gangs);
            map.Add(_mutes);
            map.Add(_guildRepo);
            map.Add(_userRepo);
            map.Add(_gangRepo);
            map.Add(_muteRepo);
            map.Add(_gamblingService);
            map.Add(_interactiveService);
            map.Add(_responseService);
            map.Add(_errorHandler);
            map.Add(_loggingService);
            map.Add(_rankingService);
        }

    }
}

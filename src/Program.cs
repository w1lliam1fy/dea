using DEA.Common.Data;
using DEA.Common.Utilities;
using DEA.Database.Models;
using DEA.Database.Repositories;
using DEA.Events;
using DEA.Services;
using DEA.Services.Handlers;
using DEA.Services.Static;
using DEA.Services.Timers;
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
    internal class Program
    {
        private static void Main()
        {
            new Program().RunAsync().GetAwaiter().GetResult();
        }

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;

        private readonly Credentials _credentials;

        private readonly IMongoCollection<Guild> _guilds;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Gang> _gangs;
        private readonly IMongoCollection<Poll> _polls;
        private readonly IMongoCollection<Mute> _mutes;
        private readonly IMongoCollection<Blacklist> _blacklists;

        public Program()
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
                Logger.Log(LogSeverity.Critical, "Error while loading up Credentials.json, please fix this issue and restart the bot", e.Message);
                Console.ReadLine();
                Environment.Exit(0);
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                MessageCacheSize = 10,
                TotalShards = _credentials.ShardCount,
                AlwaysDownloadUsers = true,
            });

            _commandService = new CommandService(new CommandServiceConfig()
            {
                CaseSensitiveCommands = false,
                LogLevel = LogSeverity.Error,
                DefaultRunMode = RunMode.Async,
            });

             var dbClient = new MongoClient(_credentials.MongoDBConnectionString);
             var database = dbClient.GetDatabase(_credentials.DatabaseName);

            _guilds = database.GetCollection<Guild>("guilds");
            _users = database.GetCollection<User>("users");
            _gangs = database.GetCollection<Gang>("gangs");
            _polls = database.GetCollection<Poll>("polls");
            _mutes = database.GetCollection<Mute>("mutes");
            _blacklists = database.GetCollection<Blacklist>("blacklists");
        }

        private async Task RunAsync()
        {
            Logger.NewLine("===   DEA   ===");
            Logger.NewLine();

            var sw = Stopwatch.StartNew();
            try
            {
                await _client.LoginAsync(TokenType.Bot, _credentials.Token);
            }
            catch (HttpException httpEx)
            {
                Logger.Log(LogSeverity.Critical, $"Login failed", httpEx.Reason);
                Console.ReadLine();
                Environment.Exit(0);
            }
               
            await _client.StartAsync();
            sw.Stop();
            Logger.Log(LogSeverity.Info, "Successfully connected", $"Elapsed time: {sw.Elapsed.TotalSeconds.ToString("N3")} seconds.");

            var map = new DependencyMap();
            ConfigureServices(map);
            Logger.Log(LogSeverity.Info, "Mapping successfully configured", $"Services ready.");

            Logger.Log(LogSeverity.Info, "MongoDb Connection Verification", "Test connection has commenced...");
            sw.Restart();
            await _users.CountAsync(y => y.Cash > 0);
            sw.Stop();
            Logger.Log(LogSeverity.Info, "Test connection has succeeded", $"Elapsed time: {sw.Elapsed.TotalSeconds.ToString("N3")} seconds.");

            InitializeTimersAndEvents(map);
            await new CommandHandler(_commandService, map).InitializeAsync();
            Logger.Log(LogSeverity.Info, "Events and command handler successfully initialized", $"Client ready.");

            await Task.Delay(-1);
        }

        private void ConfigureServices(IDependencyMap map)
        {
            map.Add(_client);
            map.Add(_credentials);
            map.Add(new PollRepository(_polls));
            map.Add(new GuildRepository(_guilds));
            map.Add(new BlacklistRepository(_blacklists));
            map.Add(new RankHandler(map.Get<GuildRepository>()));
            map.Add(new UserRepository(_users, map.Get<RankHandler>()));
            map.Add(new InteractiveService(_client));
            map.Add(new GameService(map.Get<InteractiveService>(), map.Get<UserRepository>()));
            map.Add(new ModerationService(map.Get<GuildRepository>()));
            map.Add(new ErrorHandler(_commandService));
            map.Add(new GangRepository(_gangs));
            map.Add(new MuteRepository(_mutes));
            map.Add(new Statistics());
        }

        private void InitializeTimersAndEvents(IDependencyMap map)
        {
            new Ready(map);
            new JoinedGuild(map);
            new GuildUpdated(map);
            new UserJoined(map);
            new ApplyIntrestRate(map);
            new AutoDeletePolls(map);
            new AutoTrivia(map);
            new AutoUnmute(map);
            new ResetTempMultiplier(map);
        }

    }
}

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
    internal class Program
    {
        private static void Main() =>
            new Program().RunAsync().GetAwaiter().GetResult();

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;

        private readonly Credentials _credentials;

        private readonly IMongoCollection<Guild> _guilds;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Gang> _gangs;
        private readonly IMongoCollection<Mute> _mutes;

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

             var dbClient = new MongoClient(_credentials.MongoDBConnectionString);
             var database = dbClient.GetDatabase(_credentials.DatabaseName);

            _guilds = database.GetCollection<Guild>("guilds");
            _users = database.GetCollection<User>("users");
            _gangs = database.GetCollection<Gang>("gangs");
            _mutes = database.GetCollection<Mute>("mutes");
        }

        private async Task RunAsync()
        {
            await Logger.NewLineAsync("===   DEA   ===");
            await Logger.NewLineAsync();

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
               
            await _client.StartAsync();
            sw.Stop();
            await Logger.LogAsync(LogSeverity.Info, "Successfully connected", $"Elapsed time: {sw.Elapsed.TotalSeconds.ToString()} seconds.");

            var map = new DependencyMap();
            ConfigureServices(map);
            
            await Logger.LogAsync(LogSeverity.Info, "Mapping successfully configured", $"Services ready.");

            new Ready(map);
            await new CommandHandler(_commandService, map).InitializeAsync();
            await Logger.LogAsync(LogSeverity.Info, "Events and command handler successfully initialized", $"Client ready.");

            await Task.Delay(-1);
        }

        private void ConfigureServices(IDependencyMap map)
        {
            map.Add(_client);
            map.Add(_credentials);
            map.Add(_guilds);
            map.Add(_users);
            map.Add(_gangs);
            map.Add(_mutes);
            map.Add(new GuildRepository(_guilds));
            map.Add(new RankHandler(map.Get<GuildRepository>()));
            map.Add(new UserRepository(_users, map.Get<RankHandler>()));
            map.Add(new GamblingService(map.Get<UserRepository>()));
            map.Add(new InteractiveService(_client));
            map.Add(new GameService(map.Get<InteractiveService>(), map.Get<UserRepository>()));
            map.Add(new ModerationService());
            map.Add(new ErrorHandler(_commandService));
            map.Add(new LoggingService(map.Get<GuildRepository>()));
            map.Add(new GangRepository(_gangs));
            map.Add(new MuteRepository(_mutes));
        }

    }
}

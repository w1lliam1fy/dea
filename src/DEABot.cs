using DEA.Database.Models;
using DEA.Events;
using DEA.Resources;
using DEA.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DEA
{
    public class DEABot
    {
        public static Credentials Credentials { get; private set; }
        public static CommandService CommandService { get; private set; }
        public static DiscordSocketClient Client { get; private set; }

        public static MongoClient DBClient { get; private set; }
        public static IMongoDatabase Database { get; private set; }

        public static IMongoCollection<Guild> Guilds { get; private set; }
        public static IMongoCollection<User> Users { get; private set; }
        public static IMongoCollection<Gang> Gangs { get; private set; }
        public static IMongoCollection<Mute> Mutes { get; private set; }

        public static UpdateDefinitionBuilder<Guild> GuildUpdateBuilder { get; private set; }
        public static UpdateDefinitionBuilder<User> UserUpdateBuilder { get; private set; }
        public static UpdateDefinitionBuilder<Gang> GangUpdateBuilder { get; private set; }

        static DEABot()
        {
            try
            {
                using (StreamReader file = File.OpenText(@"Credentials.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    Credentials = (Credentials)serializer.Deserialize(file, typeof(Credentials));
                }
            }
            catch (IOException e)
            {
                PrettyConsole.Log(LogSeverity.Critical, "Error while loading up Credentials.json, please fix this issue and restart the bot", e.Message);
                Console.ReadLine();
                Environment.Exit(0);
            }

            DBClient = new MongoClient(Credentials.MongoDBConnectionString);
            Database = DBClient.GetDatabase(Credentials.DatabaseName);

            Guilds = Database.GetCollection<Guild>("guilds");
            Users = Database.GetCollection<User>("users");
            Gangs = Database.GetCollection<Gang>("gangs");
            Mutes = Database.GetCollection<Mute>("mutes");

            GuildUpdateBuilder = Builders<Guild>.Update;
            UserUpdateBuilder = Builders<User>.Update;
            GangUpdateBuilder = Builders<Gang>.Update;
        }

        public async Task RunAsync(params string[] args)
        {
            PrettyConsole.NewLine("===   DEA   ===");
            PrettyConsole.NewLine();
            Client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                MessageCacheSize = 10,
                TotalShards = Credentials.ShardCount,
                //AlwaysDownloadUsers = true,
            });

            CommandService = new CommandService(new CommandServiceConfig()
            {
                CaseSensitiveCommands = false,
                LogLevel = LogSeverity.Debug,
                DefaultRunMode = RunMode.Async,
            });

            var sw = Stopwatch.StartNew();
            //Connection
            try
            {
                await Client.LoginAsync(TokenType.Bot, Credentials.Token);
            }
            catch (Discord.Net.HttpException httpEx)
            {
                switch (httpEx.HttpCode)
                {
                    case HttpStatusCode.Unauthorized:
                        PrettyConsole.Log(LogSeverity.Critical, "Login failed", "Invalid token.");
                        break;
                    default:
                        PrettyConsole.Log(LogSeverity.Critical, $"Login failed", httpEx.Reason);
                        break;
                }
                Console.ReadLine();
                Environment.Exit(0);
            }
            await Client.StartAsync().ConfigureAwait(false);
            //await Client.DownloadAllUsersAsync().ConfigureAwait(false);
            sw.Stop();
            PrettyConsole.Log(LogSeverity.Info, "Successfully connected", $"Elapsed time: {sw.Elapsed.TotalSeconds.ToString()} seconds.");

            var Map = new DependencyMap();
            ConfigureServices(Map);
            await new MessageRecieved().InitializeAsync(Map);
            new Ready();
            PrettyConsole.Log(LogSeverity.Info, "Events and mapping successfully initialized", $"Client ready.");
        }

        public async Task RunAndBlockAsync(params string[] args)
        {
            await RunAsync(args).ConfigureAwait(false);
            await Task.Delay(-1).ConfigureAwait(false);
        }

        private void ConfigureServices(IDependencyMap map)
        {
            map.Add(Client);
        }

    }
}

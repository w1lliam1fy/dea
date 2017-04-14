using DEA.Common;
using DEA.Database.Models;
using DEA.Events;
using DEA.Services;
using DEA.Services.Handlers;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
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

        public static int Commands { get; set; }
        public static int Messages { get; set; }

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
                Logger.Log(LogSeverity.Critical, "Error while loading up Credentials.json, please fix this issue and restart the bot", e.Message).RunSynchronously();
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

            var builder = Builders<Gang>.Filter;
            foreach (var gang in Gangs.Find(builder.Empty).ToList())
                Gangs.UpdateOne(y => y.Id == gang.Id,
                    GangUpdateBuilder.Set(x => x.Name, new Regex(@"[^A-Za-z0-9 ]").Replace(gang.Name, string.Empty)));
        }

        public async Task RunAsync(params string[] args)
        {
            await Logger.NewLine("===   DEA   ===");
            await Logger.NewLine();
            Client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                MessageCacheSize = 10,
                TotalShards = Credentials.ShardCount,
            });

            CommandService = new CommandService(new CommandServiceConfig()
            {
                CaseSensitiveCommands = false,
                LogLevel = LogSeverity.Debug,
                DefaultRunMode = RunMode.Async,
            });

            var sw = Stopwatch.StartNew();
            try
            {
                await Client.LoginAsync(TokenType.Bot, Credentials.Token);
            }
            catch (HttpException httpEx)
            {
                await ErrorHandler.HandleLoginFailure(httpEx);
            }
               
            await Client.StartAsync().ConfigureAwait(false);
            sw.Stop();
            await Logger.Log(LogSeverity.Info, "Successfully connected", $"Elapsed time: {sw.Elapsed.TotalSeconds.ToString()} seconds.");

            await new CommandHandler().InitializeAsync();
            new Ready();
            await Logger.Log(LogSeverity.Info, "Events and mapping successfully initialized", $"Client ready.");
        }

        public async Task RunAndBlockAsync(params string[] args)
        {
            await RunAsync(args).ConfigureAwait(false);
            await Task.Delay(-1).ConfigureAwait(false);
        }

    }
}

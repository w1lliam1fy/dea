using DEA.Database.Models;
using DEA.Services;
using DEA.Services.Handlers;
using DEA.Services.Static;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DEA
{
    internal sealed class Program
    {
        private static void Main()
        {
            new Program().RunAsync().GetAwaiter().GetResult();
        }

        private readonly IServiceProvider _serviceProvider;
        private readonly ServiceManager _serviceManager;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;

        public Program()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                MessageCacheSize = 10,
                TotalShards = Data.Credentials.ShardCount,
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Warning,
            });

            _commandService = new CommandService(new CommandServiceConfig()
            {
                CaseSensitiveCommands = false,
                LogLevel = LogSeverity.Warning,
                DefaultRunMode = RunMode.Async,
            });

            _serviceManager = new ServiceManager(_client, _commandService);

            _serviceProvider = _serviceManager.ServiceProvider;
            _serviceProvider.GetService<IMongoCollection<Guild>>().UpdateMany(Builders<Guild>.Filter.Empty, Builders<Guild>.Update.Unset("AutoTrivia"));
        }

        private async Task RunAsync()
        {
            Logger.NewLine("===   DEA   ===");
            Logger.NewLine();

            var sw = Stopwatch.StartNew();
            try
            {
                await _client.LoginAsync(TokenType.Bot, Data.Credentials.Token);
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

            Logger.Log(LogSeverity.Info, "MongoDb Connection Verification", "Test connection has commenced...");
            sw.Restart();
            await _serviceProvider.GetService<IMongoCollection<User>>().CountAsync(y => y.Cash > 0);
            sw.Stop();
            Logger.Log(LogSeverity.Info, "Test connection has succeeded", $"Elapsed time: {sw.Elapsed.TotalSeconds.ToString("N3")} seconds.");

            _serviceManager.AddTypeReaders();
            Logger.Log(LogSeverity.Info, "Addition of custom Type Readers", $"Successfully initialized.");

            _serviceManager.InitializeTimersAndEvents();
            await new CommandHandler(_commandService, _serviceProvider).InitializeAsync();
            Logger.Log(LogSeverity.Info, "Events and command handler successfully initialized", $"Client ready.");

            await Task.Delay(-1);
        }
    }
}

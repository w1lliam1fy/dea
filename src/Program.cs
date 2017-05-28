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
            _serviceManager = new ServiceManager();

            _serviceManager.InitiliazeData();

            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                MessageCacheSize = 10,
                TotalShards = _serviceManager.Credentials.ShardCount,
                AlwaysDownloadUsers = true,
            });

            _commandService = new CommandService(new CommandServiceConfig()
            {
                CaseSensitiveCommands = false,
                LogLevel = LogSeverity.Error,
                DefaultRunMode = RunMode.Async,
            });

            _serviceProvider = _serviceManager.ConfigureServices(_client, _commandService);
        }

        private async Task RunAsync()
        {
            Logger.NewLine("===   DEA   ===");
            Logger.NewLine();

            var sw = Stopwatch.StartNew();
            try
            {
                await _client.LoginAsync(TokenType.Bot, _serviceManager.Credentials.Token);
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

            _serviceManager.AddTypeReaders(_commandService, _serviceProvider);
            Logger.Log(LogSeverity.Info, "Addition of custom Type Readers", $"Successfully initialized.");

            _serviceManager.InitializeTimersAndEvents(_serviceProvider);
            await new CommandHandler(_commandService, _serviceProvider).InitializeAsync();
            Logger.Log(LogSeverity.Info, "Events and command handler successfully initialized", $"Client ready.");

            await Task.Delay(-1);
        }
    }
}

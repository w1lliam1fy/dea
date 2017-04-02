using Discord;
using Discord.WebSocket;
using DEA.Services;
using DEA.Events;
using System.Threading.Tasks;
using Discord.Commands;
using System.IO;
using Newtonsoft.Json;
using DEA.Resources;
using DEA.SQLite.Models;

namespace DEA
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        public async Task Start()
        {
            PrettyConsole.NewLine("===   DEA   ===");
            PrettyConsole.NewLine();

            using (var db = new DbContext())
            {
                db.Database.EnsureCreated();
            }

            using (StreamReader file = File.OpenText(@"..\..\Credentials.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                Config.CREDENTIALS = (Credentials)serializer.Deserialize(file, typeof(Credentials));
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Error,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 10,
                TotalShards = Config.CREDENTIALS.ShardCount
            });

            _client.Log += (l)
                => Task.Run(()
                => PrettyConsole.Log(l.Severity, l.Source, l.Exception?.ToString() ?? l.Message));

            await _client.LoginAsync(TokenType.Bot, Config.CREDENTIALS.Token);
            
            await _client.StartAsync();

            var map = new DependencyMap();
            ConfigureServices(map);

            await new MessageRecieved().InitializeAsync(_client, map);

            new Ready(_client);
            new UserEvents(_client);
            new RoleEvents(_client);
            new ChannelEvents(_client);
            new RecurringFunctions(_client);

            await Task.Delay(-1);
        }

        public void ConfigureServices(IDependencyMap map)
        {
            map.Add(_client);
        }
    }
}
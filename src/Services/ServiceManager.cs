using DEA.Common.Items;
using DEA.Common.TypeReaders;
using DEA.Common.Utilities;
using DEA.Database.Models;
using DEA.Database.Repositories;
using DEA.Events;
using DEA.Services.Handlers;
using DEA.Services.Static;
using DEA.Services.Timers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace DEA.Services
{
    public sealed class ServiceManager
    {
        private Credentials _credentials;
        
        private Armour[] _armour;
        private Crate[] _crates;
        private CrateItem[] _crateItems;
        private Fish[] _fish;
        private Food[] _food;
        private Gun[] _guns;
        private Item[] _items;
        private Knife[] _knives;
        private Meat[] _meat;
        private Weapon[] _weapons;

        public Credentials Credentials => _credentials;

        public void InitiliazeData()
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();

                using (StreamReader file = File.OpenText(Config.MAIN_DIRECTORY + @"src\Credentials.json"))
                {
                    _credentials = (Credentials)serializer.Deserialize(file, typeof(Credentials));
                }
                using (StreamReader file = File.OpenText(Config.MAIN_DIRECTORY + @"src\Data\Items\Armour.json"))
                {
                    _armour = (Armour[])serializer.Deserialize(file, typeof(Armour[]));
                }
                using (StreamReader file = File.OpenText(Config.MAIN_DIRECTORY + @"src\Data\Items\Crates.json"))
                {
                    _crates = ((Crate[])serializer.Deserialize(file, typeof(Crate[]))).OrderBy(x => x.Price).ToArray();
                }
                using (StreamReader file = File.OpenText(Config.MAIN_DIRECTORY + @"src\Data\Items\Fish.json"))
                {
                    _fish = ((Fish[])serializer.Deserialize(file, typeof(Fish[]))).OrderByDescending(x => x.AcquireOdds).ToArray();
                }
                using (StreamReader file = File.OpenText(Config.MAIN_DIRECTORY + @"src\Data\Items\Guns.json"))
                {
                    _guns = (Gun[])serializer.Deserialize(file, typeof(Gun[]));
                }
                using (StreamReader file = File.OpenText(Config.MAIN_DIRECTORY + @"src\Data\Items\Knives.json"))
                {
                    _knives = (Knife[])serializer.Deserialize(file, typeof(Knife[]));
                }
                using (StreamReader file = File.OpenText(Config.MAIN_DIRECTORY + @"src\Data\Items\Meat.json"))
                {
                    _meat = ((Meat[])serializer.Deserialize(file, typeof(Meat[]))).OrderByDescending(x => x.AcquireOdds).ToArray();
                }
                using (StreamReader file = File.OpenText(Config.MAIN_DIRECTORY + @"src\Data\Items\Miscellanea.json"))
                {
                    _items = (Item[])serializer.Deserialize(file, typeof(Item[]));
                }
            }
            catch (IOException e)
            {
                Logger.Log(LogSeverity.Critical, "Error while loading up data, please fix this issue and restart the bot", e.Message);
                Console.ReadLine();
                Environment.Exit(0);
            }

            _food = (_fish as Food[]).Concat(_meat).ToArray();
            _weapons = (_guns as Weapon[]).Concat(_knives).ToArray();
            _crateItems = (_weapons as CrateItem[]).Concat(_armour).OrderByDescending(x => x.CrateOdds).ToArray();
            _items = _items.Concat(_crates).Concat(_crateItems).Concat(_food).ToArray();
        }

        public IServiceProvider ConfigureServices(DiscordSocketClient client, CommandService commandService)
        {
            var database = ConfigureDatabase();

            var services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commandService)
                .AddSingleton(_credentials)
                .AddSingleton(database.GetCollection<User>("users"))
                .AddSingleton(database.GetCollection<Guild>("guilds"))
                .AddSingleton(database.GetCollection<Gang>("gangs"))
                .AddSingleton(database.GetCollection<Mute>("mutes"))
                .AddSingleton(database.GetCollection<Blacklist>("blacklists"))
                .AddSingleton(database.GetCollection<Poll>("polls"))
                .AddSingleton(_items)
                .AddSingleton(_crates)
                .AddSingleton(_crateItems)
                .AddSingleton(_armour)
                .AddSingleton(_weapons)
                .AddSingleton(_guns)
                .AddSingleton(_knives)
                .AddSingleton(_food)
                .AddSingleton(_fish)
                .AddSingleton(_meat)
                .AddSingleton<PollRepository>()
                .AddSingleton<GuildRepository>()
                .AddSingleton<BlacklistRepository>()
                .AddSingleton<RankHandler>()
                .AddSingleton<UserRepository>()
                .AddSingleton<InteractiveService>()
                .AddSingleton<GameService>()
                .AddSingleton<ModerationService>()
                .AddSingleton<RateLimitService>()
                .AddSingleton<ErrorHandler>()
                .AddSingleton<GangRepository>()
                .AddSingleton<MuteRepository>()
                .AddSingleton<Statistics>();

            return new DefaultServiceProviderFactory().CreateServiceProvider(services);
        }

        public void InitializeTimersAndEvents(IServiceProvider serviceProvider)
        {
            new JoinedGuild(serviceProvider);
            new GuildUpdated(serviceProvider);
            new UserJoined(serviceProvider);
            new AutoIntrestRate(serviceProvider);
            new AutoDeletePolls(serviceProvider);
            new AutoTrivia(serviceProvider);
            new AutoUnmute(serviceProvider);
            new Ready(serviceProvider);
        }

        public void AddTypeReaders(CommandService commandService, IServiceProvider serviceProvider)
        {
            commandService.AddTypeReader<Crate>(new CrateTypeReader(serviceProvider));
            commandService.AddTypeReader<Food>(new FoodTypeReader(serviceProvider));
            commandService.AddTypeReader<Gun>(new GunTypeReader(serviceProvider));
            commandService.AddTypeReader<Item>(new ItemTypeReader(serviceProvider));
            commandService.AddTypeReader<Knife>(new KnifeTypeReader(serviceProvider));
            commandService.AddTypeReader<Weapon>(new WeaponTypeReader(serviceProvider));
        }

        private IMongoDatabase ConfigureDatabase()
        {
            var dbClient = new MongoClient(_credentials.MongoDBConnectionString);
            return dbClient.GetDatabase(_credentials.DatabaseName);
        }
    }
}

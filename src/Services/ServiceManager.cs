using DEA.Common.Items;
using DEA.Common.TypeReaders;
using DEA.Common.Utilities;
using DEA.Database.Models;
using DEA.Database.Repositories;
using DEA.Events;
using DEA.Services.Handlers;
using DEA.Services.Timers;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using DEA.Services.Static;

namespace DEA.Services
{
    public sealed class ServiceManager
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;

        public IServiceProvider ServiceProvider { get; }

        public ServiceManager(DiscordSocketClient client, CommandService commandService)
        {
            _client = client;
            _commandService = commandService;

            var database = ConfigureDatabase();

            var services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commandService)
                .AddSingleton(database.GetCollection<User>("users"))
                .AddSingleton(database.GetCollection<Guild>("guilds"))
                .AddSingleton(database.GetCollection<Gang>("gangs"))
                .AddSingleton(database.GetCollection<Mute>("mutes"))
                .AddSingleton(database.GetCollection<Blacklist>("blacklists"))
                .AddSingleton(database.GetCollection<Poll>("polls"))
                .AddSingleton<PollRepository>()
                .AddSingleton<GuildRepository>()
                .AddSingleton<BlacklistRepository>()
                .AddSingleton<RankHandler>()
                .AddSingleton<UserRepository>()
                .AddSingleton<InteractiveService>()
                .AddSingleton<GameService>()
                .AddSingleton<ModerationService>()
                .AddSingleton<CooldownService>()
                .AddSingleton<RateLimitService>()
                .AddSingleton<ErrorHandler>()
                .AddSingleton<GangRepository>()
                .AddSingleton<MuteRepository>()
                .AddSingleton<Statistics>();

            ServiceProvider = new DefaultServiceProviderFactory().CreateServiceProvider(services);
        }

        public void InitializeTimersAndEvents()
        {
            new JoinedGuild(ServiceProvider);
            new GuildUpdated(ServiceProvider);
            new UserJoined(ServiceProvider);
            new AutoIntrestRate(ServiceProvider);
            new AutoDeletePolls(ServiceProvider);
            new AutoUnmute(ServiceProvider);
            new Log(ServiceProvider);
            new Ready(ServiceProvider);
        }

        public void AddTypeReaders()
        {
            _commandService.AddTypeReader<Crate>(new CrateTypeReader());
            _commandService.AddTypeReader<Food>(new FoodTypeReader());
            _commandService.AddTypeReader<Gun>(new GunTypeReader());
            _commandService.AddTypeReader<Item>(new ItemTypeReader());
            _commandService.AddTypeReader<Knife>(new KnifeTypeReader());
            _commandService.AddTypeReader<Weapon>(new WeaponTypeReader());
        }

        private IMongoDatabase ConfigureDatabase()
        {
            var dbClient = new MongoClient(Data.Credentials.MongoDBConnectionString);
            return dbClient.GetDatabase(Data.Credentials.DatabaseName);
        }
    }
}

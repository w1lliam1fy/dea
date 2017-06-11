using DEA.Database.Models;
using DEA.Database.Repositories;
using DEA.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DEA.Common
{
    public sealed class Context : SocketCommandContext
    {
        public IGuildUser GUser { get; }
        public CommandInfo Command { get; internal set; }
        public User DbUser { get; private set; }
        public Guild DbGuild { get; private set; }
        public Gang Gang { get; private set; }
        public string Prefix { get; private set; }
        public decimal Cash { get; internal set; }

        private readonly IServiceProvider _serviceProvider;
        private readonly UserRepository _userRepo;
        private readonly GuildRepository _guildRepo;
        private readonly GangRepository _gangRepo;
        private readonly GameService _gameService;

        public Context(DiscordSocketClient client, SocketUserMessage msg, IServiceProvider serviceProvider) : base(client, msg)
        {
            _serviceProvider = serviceProvider;
            _userRepo = _serviceProvider.GetService<UserRepository>();
            _guildRepo = _serviceProvider.GetService<GuildRepository>();
            _gangRepo = _serviceProvider.GetService<GangRepository>();
            _gameService = _serviceProvider.GetService<GameService>();
            
            GUser = User as IGuildUser;
        }

        public async Task InitializeAsync()
        {
            DbUser = await _userRepo.GetUserAsync(GUser);
            DbGuild = await _guildRepo.GetGuildAsync(Guild.Id);
            Gang = await _gangRepo.GetGangAsync(GUser);
            Prefix = DbGuild.Prefix;
            Cash = DbUser.Cash;
        }
    }
}

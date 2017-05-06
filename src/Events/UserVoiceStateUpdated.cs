using DEA.Database.Repositories;
using DEA.Services.Static;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    /// <summary>
    /// An event that is run every time a user's voice state is updated.
    /// </summary>
    class UserVoiceStateUpdated
    {
        private readonly IDependencyMap _map;
        private readonly DiscordSocketClient _client;
        private readonly UserRepository _userRepo;
        private readonly GuildRepository _guildRepo;

        public UserVoiceStateUpdated(IDependencyMap map)
        {
            _map = map;
            _client = _map.Get<DiscordSocketClient>();
            _userRepo = _map.Get<UserRepository>();
            _guildRepo = _map.Get<GuildRepository>();
            _client.UserVoiceStateUpdated += HandleUserVoiceStateUpdated;
        }

        private Task HandleUserVoiceStateUpdated(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            return Task.Run(async () =>
            {
                Logger.Log(LogSeverity.Debug, $"Event", "User Voice State Updated");

                System.Console.WriteLine(oldState.Equals(newState));

                var guildUser = user as IGuildUser;
                if (guildUser == null) return;

                var dbUser = await _userRepo.GetUserAsync(guildUser);
                var dbGuild = await _guildRepo.GetGuildAsync(guildUser.GuildId);

                await _userRepo.ApplyCash(guildUser, dbUser, dbGuild);
            });
        }
    }
}

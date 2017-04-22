using DEA.Common;
using DEA.Database.Repository;
using DEA.Services.Static;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System.Threading.Tasks;

namespace DEA.Services.Handlers
{
    public class CommandHandler
    {
        private IDependencyMap _map;
        private DiscordSocketClient _client;
        private CommandService _commandService;
        private ErrorHandler _errorHandler;
        private UserRepository _userRepo;

        public CommandHandler(CommandService commandService, IDependencyMap map)
        {
            _map = map;
            _commandService = commandService;
            _errorHandler = map.Get<ErrorHandler>();
            _userRepo = map.Get<UserRepository>();
            _client = map.Get<DiscordSocketClient>();
            _client.MessageReceived += HandleCommandAsync;
        }

        public async Task InitializeAsync()
        {
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task HandleCommandAsync(SocketMessage s)
        {
            Config.MESSAGES++;
            var msg = s as SocketUserMessage;
            if (msg == null) return;

            var context = new DEAContext(_client, msg, _map);
            if (context.Guild == null) return;
            if (context.User.IsBot) return;

            var perms = (context.Guild.CurrentUser as IGuildUser).GetPermissions(context.Channel as SocketTextChannel);

            if (!perms.SendMessages || !perms.EmbedLinks) return;

            int argPos = 0;

            await context.InitializeAsync();

            if (msg.HasStringPrefix(context.DbGuild.Prefix, ref argPos) ||
                msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _commandService.ExecuteAsync(context, argPos, _map);
                if (!result.IsSuccess)
                    await _errorHandler.HandleCommandFailureAsync(context, result, argPos);
                else
                    Config.COMMANDS_RUN++;
            }
            else if (msg.Content.Length >= Config.MIN_CHAR_LENGTH)
                await CashPerMsg.Apply(_userRepo, context);
        }

    }
}

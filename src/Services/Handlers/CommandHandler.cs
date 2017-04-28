using DEA.Common;
using DEA.Common.Extensions.DiscordExtensions;
using DEA.Database.Repositories;
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
        private readonly IDependencyMap _map;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly ErrorHandler _errorHandler;
        private readonly UserRepository _userRepo;

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

        public Task HandleCommandAsync(SocketMessage s) =>
            Task.Run(async () =>
            {
                Config.MESSAGES++;
                var msg = s as SocketUserMessage;
                if (msg == null)
                    return;

                var context = new DEAContext(_client, msg, _map);
                if (context.Guild == null)
                    return;
                if (context.User.IsBot)
                    return;

                var perms = (context.Guild.CurrentUser as IGuildUser).GetPermissions(context.Channel as SocketTextChannel);

                if (!perms.SendMessages || !perms.EmbedLinks)
                {
                    try
                    {
                        var channel = await context.User.CreateDMChannelAsync();

                        await channel.SendAsync($"DEA cannot execute any commands without the permission to send embedded messages.");
                    }
                    catch { }
                    return;
                }

                await context.InitializeAsync();

                int argPos = 0;

                if (msg.HasStringPrefix(context.DbGuild.Prefix, ref argPos) ||
                    msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
                {
                    Logger.Log(LogSeverity.Debug, $"Guild: {context.Guild}, User: {context.User}", msg.Content);

                    var result = await _commandService.ExecuteAsync(context, argPos, _map);
                    if (!result.IsSuccess)
                        await _errorHandler.HandleCommandFailureAsync(context, result, argPos);
                    else
                        Config.COMMANDS_RUN++;
                }
                else if (msg.Content.Length >= Config.MIN_CHAR_LENGTH)
                    await CashPerMsg.Apply(_userRepo, context);
            });
    }
}

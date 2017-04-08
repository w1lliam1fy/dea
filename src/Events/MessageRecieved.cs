using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DEA.Database.Repository;
using MongoDB.Driver;
//using Discord.Addons.InteractiveCommands;

namespace DEA.Services
{
    public class MessageRecieved
    {
        private DiscordSocketClient _client;
        private CommandService _service;
        private IDependencyMap _map;

        public async Task InitializeAsync(DiscordSocketClient c, IDependencyMap map)
        {
            _client = c;
            _service = new CommandService(new CommandServiceConfig()
            {
                CaseSensitiveCommands = false,
#if DEBUG
                DefaultRunMode = RunMode.Sync
#elif RELEASE
                DefaultRunMode = RunMode.Async
#endif
            });

            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
            _map = map;
            //_map.Add(new InteractiveService(_client));

            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;

            var Context = new SocketCommandContext(_client, msg);

            if (Context.User.IsBot) return;

            if (!(Context.Channel is SocketTextChannel)) return;

            var perms = (Context.Guild.CurrentUser as IGuildUser).GetPermissions(Context.Channel as SocketTextChannel);

            if (!perms.SendMessages || !perms.EmbedLinks) return;

            int argPos = 0;

            var guild = GuildRepository.FetchGuild(Context.Guild.Id);

            string prefix = guild.Prefix;

            if (msg.HasStringPrefix(prefix, ref argPos) ||
                msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                PrettyConsole.Log(LogSeverity.Debug, $"Guild: {Context.Guild.Name}, User: {Context.User}", msg.Content);
                var result = await _service.ExecuteAsync(Context, argPos, _map);
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    var cmd = _service.Search(Context, argPos).Commands.First().Command;
                    if (result.ErrorReason.Length == 0) return;
                    var message = "";
                    switch (result.Error)
                    {
                        case CommandError.BadArgCount:
                            message = $"You are incorrectly using this command. Usage: `{prefix}{cmd.Remarks}`";
                            break;
                        case CommandError.ParseFailed:
                            message = $"Invalid number.";
                            break;
                        default:
                            message = $"{result.ErrorReason}";
                            break;
                    }

                    await ResponseMethods.Reply(Context, message, null, new Color(255, 0, 0));
                }
            }
            else if (msg.ToString().Length >= Config.MIN_CHAR_LENGTH && !msg.ToString().StartsWith(":"))
            {
                var user = UserRepository.FetchUser(Context);

                if (DateTime.UtcNow.Subtract(user.Message).TotalMilliseconds > user.MessageCooldown)
                {
                    UserRepository.Modify(DEABot.UserUpdateBuilder.Combine(
                        DEABot.UserUpdateBuilder.Set(x => x.Cash, guild.GlobalChattingMultiplier * user.TemporaryMultiplier * user.InvestmentMultiplier + user.Cash),
                        DEABot.UserUpdateBuilder.Set(x => x.TemporaryMultiplier, user.TemporaryMultiplier + guild.TempMultiplierIncreaseRate),
                        DEABot.UserUpdateBuilder.Set(x => x.Message, DateTime.UtcNow)), Context);
                    await RankHandler.Handle(Context.Guild, Context.User.Id);
                }
            }
        }
    }
}

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DEA.Database.Repository;
using MongoDB.Driver;
using DEA.Services.Handlers;
//using Discord.Addons.InteractiveCommands;

namespace DEA.Services
{
    public class MessageRecieved
    {

        private IDependencyMap _map;

        public async Task InitializeAsync(IDependencyMap map)
        {
            await DEABot.CommandService.AddModulesAsync(Assembly.GetEntryAssembly());
            _map = map;
            //_map.Add(new InteractiveService(_client));

            DEABot.Client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;

            var Context = new SocketCommandContext(DEABot.Client, msg);

            if (Context.User.IsBot) return;

            if (!(Context.Channel is SocketTextChannel)) return;

            var perms = (Context.Guild.CurrentUser as IGuildUser).GetPermissions(Context.Channel as SocketTextChannel);

            if (!perms.SendMessages || !perms.EmbedLinks) return;

            int argPos = 0;

            var guild = GuildRepository.FetchGuild(Context.Guild.Id);

            string prefix = guild.Prefix;

            if (msg.HasStringPrefix(prefix, ref argPos) ||
                msg.HasMentionPrefix(DEABot.Client.CurrentUser, ref argPos))
            {
                PrettyConsole.Log(LogSeverity.Debug, $"Guild: {Context.Guild.Name}, User: {Context.User}", msg.Content);
                var result = await DEABot.CommandService.ExecuteAsync(Context, argPos, _map);
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    var cmd = DEABot.CommandService.Search(Context, argPos).Commands.First().Command;
                    var message = string.Empty;
                    switch (result.Error)
                    {
                        case CommandError.BadArgCount:
                            message = $"You are incorrectly using this command. Usage: `{guild.Prefix}{CommandHelper.GetUsage(cmd)}`";
                            break;
                        case CommandError.ParseFailed:
                            message = $"Invalid number.";
                            break;
                        default:
                            message = result.ErrorReason;
                            break;
                    }
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        var builder = new EmbedBuilder()
                        {
                            Description = $"{Context.User.Mention}, {message}",
                            Color = new Color(255, 0, 0)
                        };

                        await Context.Channel.SendMessageAsync(string.Empty, embed: builder);
                    }
                        
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

using DEA.Common;
using DEA.Common.Extensions.DiscordExtensions;
using DEA.Database.Repositories;
using DEA.Services.Static;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;
using DEA.Common.Utilities;

namespace DEA.Services.Handlers
{
    internal sealed class CommandHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly Statistics _statistics;
        private readonly ErrorHandler _errorHandler;
        private readonly RankHandler _RankHandler;
        private readonly UserRepository _userRepo;
        private readonly BlacklistRepository _blacklistRepo;

        public CommandHandler(CommandService commandService, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _commandService = commandService;
            _statistics = _serviceProvider.GetService<Statistics>();
            _errorHandler = _serviceProvider.GetService<ErrorHandler>();
            _RankHandler = _serviceProvider.GetService<RankHandler>();
            _userRepo = _serviceProvider.GetService<UserRepository>();
            _blacklistRepo = _serviceProvider.GetService<BlacklistRepository>();
            _client = _serviceProvider.GetService<DiscordSocketClient>();
            _client.MessageReceived += HandleCommandAsync;
        }

        public async Task InitializeAsync()
        {
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public Task HandleCommandAsync(SocketMessage s)
        {
            return Task.Run(async () =>
            {
                _statistics.MessagesRecieved++;

                var msg = s as SocketUserMessage;
                if (msg == null)
                {
                    return;
                }
                else if (await _blacklistRepo.AnyAsync(x => x.UserId == msg.Author.Id))
                {
                    return;
                }

                var context = new DEAContext(_client, msg, _serviceProvider);

                if (context.Guild == null)
                {
                    return;
                }
                else if (context.User.IsBot)
                {
                    return;
                }

                await context.InitializeAsync();

                int argPos = 0;

                if (msg.HasStringPrefix(context.DbGuild.Prefix, ref argPos) ||
                msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
                {
                    var perms = (context.Guild.CurrentUser as IGuildUser).GetPermissions(context.Channel as SocketTextChannel);

                    if (!perms.SendMessages || !perms.EmbedLinks)
                    {
                        await context.User.TryDMAsync("DEA cannot execute any commands without the permission to send embedded messages.");
                        return;
                    }

                    Logger.Log(LogSeverity.Debug, $"Guild: {context.Guild}, User: {context.User}", msg.Content);

                    var result = await _commandService.ExecuteAsync(context, argPos, _serviceProvider);
                    if (!result.IsSuccess)
                    {
                        await _errorHandler.HandleCommandFailureAsync(context, result);
                    }
                    else
                    {
                        _statistics.CommandsRun++;
                    }
                }
                else if (msg.Content.Length >= Config.MIN_CHAR_LENGTH)
                {
                    if (DateTime.UtcNow.Subtract(context.DbUser.LastMessage).TotalMilliseconds > Config.MSG_COOLDOWN * 1000)
                    {
                        await _userRepo.ModifyAsync(context.DbUser, x =>
                        {
                            x.LastMessage = DateTime.UtcNow;
                            x.Cash += context.DbGuild.GlobalChattingMultiplier * Config.CASH_PER_MSG;
                        });
                        await _RankHandler.HandleAsync(context.GUser, context.DbGuild, context.DbUser);
                    }
                }
            });
        }
    }
}

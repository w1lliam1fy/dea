using DEA.Common;
using DEA.Database.Models;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DEA.Services.Handlers
{
    class ErrorHandler
    {
        public ErrorHandler()
        {
            DEABot.CommandService.Log += HandleLog;
        }

        public async Task HandleLog(LogMessage logMessage)
        {
            if (logMessage.Exception is CommandException cmdEx)
            {
                if (cmdEx.InnerException is DEAException)
                    await ResponseMethods.Reply(cmdEx.Context as DEAContext, cmdEx.InnerException.Message, null, new Color(255, 0, 0));
                else if (cmdEx.InnerException is HttpException httpEx)
                {
                    var message = string.Empty;
                    switch (httpEx.DiscordCode)
                    {
                        case null:
                        case 50013:
                            message = "DEA does not have permission to do that.";
                            break;
                        case 50007:
                            message = "DEA does not have permission to send messages to this user.";
                            break;
                        default:
                            message = httpEx.Message.Remove(0, 39) + ".";
                            break;
                    }
                    await ResponseMethods.Reply(cmdEx.Context as DEAContext, message, null, new Color(255, 0, 0));
                }
                else if (cmdEx.InnerException.GetType() != typeof(RateLimitedException))
                {
                    var message = cmdEx.InnerException.Message;
                    if (cmdEx.InnerException.InnerException != null) message += $"\n**Inner Exception:** {cmdEx.InnerException.InnerException.Message}";

                    await ResponseMethods.Reply(cmdEx.Context as DEAContext, message, null, new Color(255, 0, 0));

                    if ((await cmdEx.Context.Guild.GetCurrentUserAsync() as IGuildUser).GetPermissions(cmdEx.Context.Channel as SocketTextChannel).AttachFiles)
                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(cmdEx.ToString() ?? string.Empty)))
                            await cmdEx.Context.Channel.SendFileAsync(ms, "Stack_Trace.txt");
                }
            }
            else if (logMessage.Exception != null)
                await Logger.Log(LogSeverity.Error, logMessage.Exception.Source, $"{logMessage.Exception.Message}: {logMessage.Exception.StackTrace}");
        }

        public static async Task HandleCommandFailureAsync(DEAContext context, IResult result, int argPos)
        {
            var args = context.Message.Content.Split(' ');
            var commandName = args.First().StartsWith(context.DbGuild.Prefix) ? args.First().Remove(0, context.DbGuild.Prefix.Length) : args[1];
            var message = string.Empty;
            switch (result.Error)
            {
                case CommandError.UnknownCommand:
                    foreach (var command in DEABot.CommandService.Commands)
                    {
                        foreach (var alias in command.Aliases)
                        {
                            if (LevenshteinDistance.Compute(commandName, alias) == 1)
                                message = $"Did you mean `{context.DbGuild.Prefix}{CommandHandler.UpperFirstChar(alias)}`?";
                        }
                    }
                    break;
                case CommandError.BadArgCount:
                    var cmd = DEABot.CommandService.Search(context, argPos).Commands.First().Command;
                    message = $"You are incorrectly using this command. Usage: `{context.DbGuild.Prefix}{CommandHandler.GetUsage(cmd, commandName)}`";
                    break;
                case CommandError.ParseFailed:
                    message = $"Invalid number.";
                    break;
                case CommandError.UnmetPrecondition:
                    if (result.ErrorReason.StartsWith("Command requires guild permission "))
                    {
                        var permission = result.ErrorReason.Replace("Command requires guild permission ", string.Empty);
                        permission = Regex.Replace(permission, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled).Trim();
                        message = $"DEA requires the server pemission \"{permission}\" in order to be able to execute this command.";
                    }
                    else
                        message = result.ErrorReason;
                    break;
                default:
                    message = result.ErrorReason;
                    break;
            }

            if (!string.IsNullOrWhiteSpace(message))
                await ResponseMethods.Reply(context, message, null, new Color(255, 0, 0));
        }

        public static async Task HandleLoginFailure(HttpException httpEx)
        {
            switch (httpEx.HttpCode)
            {
                case HttpStatusCode.Unauthorized:
                    await Logger.Log(LogSeverity.Critical, "Login failed", "Invalid token.");
                    break;
                default:
                    await Logger.Log(LogSeverity.Critical, $"Login failed", httpEx.Reason);
                    break;
            }
            Console.ReadLine();
            Environment.Exit(0);
        }

    }
}
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
                    await ResponseMethods.Send(cmdEx.Context as SocketCommandContext, $"{ResponseMethods.Name(cmdEx.Context.User as IGuildUser)}, {cmdEx.InnerException.Message}", null, new Color(255, 0, 0));
                else
                {
                    var message = cmdEx.InnerException.Message;
                    if (cmdEx.InnerException.InnerException != null) message += $"\n**Inner Exception:** {cmdEx.InnerException.InnerException.Message}";

                    await ResponseMethods.Send(cmdEx.Context as SocketCommandContext, $"{ResponseMethods.Name(cmdEx.Context.User as IGuildUser)}, {message}", null, new Color(255, 0, 0));

                    if ((await cmdEx.Context.Guild.GetCurrentUserAsync() as IGuildUser).GetPermissions(cmdEx.Context.Channel as SocketTextChannel).AttachFiles)
                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(cmdEx.ToString() ?? string.Empty)))
                            await cmdEx.Context.Channel.SendFileAsync(ms, "Stack_Trace.txt");
                }
            }
        }

        public static async Task HandleCommandFailureAsync(SocketCommandContext context, IResult result, int argPos, Guild guild)
        {
            var args = context.Message.Content.Split(' ');
            var commandName = args.First().StartsWith(guild.Prefix) ? args.First().Remove(0, guild.Prefix.Length) : args[1];
            var message = string.Empty;
            switch (result.Error)
            {
                case CommandError.UnknownCommand:
                    foreach (var command in DEABot.CommandService.Commands)
                    {
                        foreach (var alias in command.Aliases)
                        {
                            if (LevenshteinDistance.Compute(commandName, alias) == 1)
                                message = $"Did you mean `{guild.Prefix}{CommandHandler.UpperFirstChar(alias)}`?";
                        }
                    }
                    break;
                case CommandError.BadArgCount:
                    var cmd = DEABot.CommandService.Search(context, argPos).Commands.First().Command;
                    message = $"You are incorrectly using this command. Usage: `{guild.Prefix}{CommandHandler.GetUsage(cmd, commandName)}`";
                    break;
                case CommandError.ParseFailed:
                    message = $"Invalid number.";
                    break;
                default:
                    message = result.ErrorReason;
                    break;
            }
            if (!string.IsNullOrWhiteSpace(message))
                await ResponseMethods.Send(context, $"{ResponseMethods.Name(context.User as IGuildUser)}, {message}", null, new Color(255, 0, 0));
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
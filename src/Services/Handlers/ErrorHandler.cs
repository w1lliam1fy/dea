using DEA.Common;
using DEA.Common.Extensions;
using DEA.Common.Extensions.DiscordExtensions;
using Discord.Commands;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DEA.Services.Handlers
{
    internal sealed class ErrorHandler
    {
        private readonly CommandService _commandService;

        public ErrorHandler(CommandService commandService)
        {
            _commandService = commandService;
        }

        public Task HandleCommandFailureAsync(Context context, IResult result)
        {
            var args = context.Message.Content.Split(' ');
            var commandName = args.First().StartsWith(context.DbGuild.Prefix) ? args.First().Remove(0, context.DbGuild.Prefix.Length) : args[1];
            var message = string.Empty;

            switch (result.Error)
            {
                case CommandError.Exception:
                    return Task.CompletedTask; // Exceptions are handled by the log event from the command service.
                case CommandError.UnknownCommand:
                    foreach (var command in _commandService.Commands)
                    {
                        foreach (var alias in command.Aliases)
                        {
                            if (commandName.SimilarTo(alias))
                            {
                                message = $"Did you mean `{context.DbGuild.Prefix}{alias.UpperFirstChar()}`?";
                            }
                        }
                    }
                    break;
                case CommandError.BadArgCount:
                    commandName = commandName.UpperFirstChar();
                    var example = context.Command.Parameters.Count == 0 ? string.Empty : $"**Example:** `{context.DbGuild.Prefix}{commandName} {context.Command.Remarks}`";

                    message = $"You are incorrectly using this command. \n\n**Usage:** `{context.DbGuild.Prefix}{commandName}{context.Command.GetUsage()}`\n\n" + example;   
                    break;
                case CommandError.ParseFailed:
                    message = $"Invalid number. Please ensure you are correctly using this command by entering: `{context.DbGuild.Prefix}Help {commandName}`.";
                    break;
                case CommandError.UnmetPrecondition:
                    if (result.ErrorReason.StartsWith("Command requires guild permission "))
                    {
                        var permission = result.ErrorReason.Replace("Command requires guild permission ", string.Empty);
                        permission = Regex.Replace(permission, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled).Trim();
                        message = $"DEA requires the server pemission \"{permission}\" in order to be able to execute this command.";
                    }
                    else
                    {
                        message = result.ErrorReason;
                    }
                    break;
                default:
                    message = result.ErrorReason;
                    break;
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                return context.Channel.ReplyAsync(context.User, message, null, Config.ErrorColor);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

    }
}
using Discord.Commands;
using Discord.WebSocket;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using DEA.Common.Extensions.DiscordExtensions;
using DEA.Services.Static;
using DEA.Common;
using System.Net;

namespace DEA.Events
{
    internal sealed class Log
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;

        public Log(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _client = _serviceProvider.GetService<DiscordSocketClient>();
            _commandService = _serviceProvider.GetService<CommandService>();

            _client.Log += LogAsync;
            _commandService.Log += LogAsync;
        }

        public Task LogAsync(LogMessage log)
        {
            return Task.Run(async () =>
            {
                if (!string.IsNullOrWhiteSpace(log.Message))
                {
                    Logger.Log(log.Severity, log.Source, log.Message);
                }

                if (log.Exception == null)
                {
                    return;
                }

                if (!(log.Exception.InnerException is FriendlyException))
                {
                    Logger.Log(log.Severity, log.Message + ":" + log.Exception.Message, log.Exception.StackTrace);
                }

                if (log.Exception is CommandException cmdEx)
                {
                    if (cmdEx.InnerException is RateLimitedException)
                    {
                        return;
                    }
                    else if (cmdEx.InnerException is FriendlyException)
                    {
                        await cmdEx.Context.Channel.ReplyAsync(cmdEx.Context.User, cmdEx.InnerException.Message, null, Config.ErrorColor);
                    }
                    else if (cmdEx.InnerException is HttpException httpEx)
                    {
                        string message;

                        switch (httpEx.DiscordCode)
                        {
                            case null:
                            case 50013:
                                switch (httpEx.HttpCode)
                                {
                                    case HttpStatusCode.BadRequest:
                                        message = "There seems to have been a bad request. Please report this issue with context at: " +
                                                  "https://github.com/RealBlazeIt/DEA/issues.\n" + httpEx.InnerException.StackTrace;
                                        break;
                                    case HttpStatusCode.Forbidden:
                                        message = "DEA does not have permission to do that. This issue *may* be fixed by moving the DEA role " +
                                                  "to the top of the roles list, and giving DEA the \"Administrator\" server permission.";
                                        break;
                                    case HttpStatusCode.InternalServerError:
                                    case HttpStatusCode.ServiceUnavailable:
                                    case HttpStatusCode.NotImplemented:
                                    case HttpStatusCode.HttpVersionNotSupported:
                                    case HttpStatusCode.GatewayTimeout:
                                    case HttpStatusCode.BadGateway:
                                        message = "Looks like Discord fucked up. An error has occured on Discord's part which is " +
                                                  "entirely unrelated with DEA. Sorry, nothing we can do.";
                                        break;
                                    default:
                                        message = httpEx.Reason ?? httpEx.HttpCode.ToString();
                                        break;
                                }
                                break;
                            case 50007:
                                message = "DEA does not have permission to send messages to this user.";
                                break;
                            default:
                                message = httpEx.Reason ?? httpEx.HttpCode.ToString();
                                break;
                        }
                        await cmdEx.Context.Channel.ReplyAsync(cmdEx.Context.User, message, null, Config.ErrorColor);
                    }
                    else
                    {
                        var message = cmdEx.InnerException.Message;
                        if (cmdEx.InnerException.InnerException != null)
                        {
                            message += $"\n**Inner Exception:** {cmdEx.InnerException.InnerException.Message}";
                        }

                        await cmdEx.Context.Channel.ReplyAsync(cmdEx.Context.User, message, null, Config.ErrorColor);
                    }
                }
            });
        }
    }
}

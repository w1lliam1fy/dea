using DEA.Resources;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.IO;
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
                {
                    var builder = new EmbedBuilder()
                    {
                        Description = $"{ResponseMethods.Name(cmdEx.Context.User as IGuildUser)}, {cmdEx.InnerException.Message}",
                        Color = new Color(255, 0, 0)
                    };

                    await cmdEx.Context.Channel.SendMessageAsync(string.Empty, embed: builder);
                }
                else
                {
                    var message = cmdEx.InnerException.Message;
                    if (cmdEx.InnerException.InnerException != null) message += $"\n**Inner Exception:** {cmdEx.InnerException.InnerException.Message}";

                    var builder = new EmbedBuilder()
                    {
                        Description = $"{ResponseMethods.Name(cmdEx.Context.User as IGuildUser)}, {message}",
                        Color = new Color(255, 0, 0)
                    };

                    await cmdEx.Context.Channel.SendMessageAsync(string.Empty, embed: builder);

                    if ((await cmdEx.Context.Guild.GetCurrentUserAsync() as IGuildUser).GetPermissions(cmdEx.Context.Channel as SocketTextChannel).AttachFiles)
                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(cmdEx.ToString() ?? string.Empty)))
                            await cmdEx.Context.Channel.SendFileAsync(ms, "Stack_Trace.txt");
                }
            }
        }

    }
}
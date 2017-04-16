using DEA.Common;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System.Threading.Tasks;

namespace DEA.Services.Handlers
{
    public class CommandHandler
    {
        public IDependencyMap _map;

        public async Task InitializeAsync()
        {
            await DEABot.CommandService.AddModulesAsync(Assembly.GetEntryAssembly());

            DEABot.Client.MessageReceived += HandleCommandAsync;
        }

        public async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;

            var context = new DEAContext(DEABot.Client, msg);
            if (context.Guild == null) return;
            if (context.User.IsBot) return;

            var perms = (context.Guild.CurrentUser as IGuildUser).GetPermissions(context.Channel as SocketTextChannel);

            if (!perms.SendMessages || !perms.EmbedLinks) return;

            int argPos = 0;

            await context.InitializeAsync();

            if (msg.HasStringPrefix(context.DbGuild.Prefix, ref argPos) ||
                msg.HasMentionPrefix(DEABot.Client.CurrentUser, ref argPos))
            {
                var result = await DEABot.CommandService.ExecuteAsync(context, argPos);
                if (!result.IsSuccess)
                    await ErrorHandler.HandleCommandFailureAsync(context, result, argPos);
                else
                    DEABot.Commands++;
            }
            else
                await CashPerMsg.Apply(context.DbGuild, context.DbUser);
        }

        public static string GetUsage(CommandInfo cmd, string name)
        {
            string usage = string.Empty;
            foreach (var param in cmd.Parameters)
            {
                string before = "<";
                string after = ">";
                if (param.IsOptional)
                {
                    before = "[";
                    after = "]";
                }
                if (param.Type == typeof(IRole) || param.Type == typeof(IGuildUser)) before = before + "@";
                if (param.Type == typeof(ITextChannel)) before = before + "#";
                usage += $" {before}{param.Summary ?? param.Name}{after} ";
            }
            return UpperFirstChar(name) + usage;
        }

        public static string UpperFirstChar(string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;

            char[] a = s.ToLower().ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }
}

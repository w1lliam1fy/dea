using DEA.Database.Repository;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System.Threading.Tasks;

namespace DEA.Services.Handlers
{
    public class CommandHandler
    {
        public async Task InitializeAsync()
        {
            await DEABot.CommandService.AddModulesAsync(Assembly.GetEntryAssembly());

            DEABot.Client.MessageReceived += HandleCommandAsync;
        }

        public async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;

            var Context = new SocketCommandContext(DEABot.Client, msg);
            if (Context.Guild == null) return;
            if (Context.User.IsBot) return;

            var perms = (Context.Guild.CurrentUser as IGuildUser).GetPermissions(Context.Channel as SocketTextChannel);

            if (!perms.SendMessages || !perms.EmbedLinks) return;

            var guild = GuildRepository.FetchGuild(Context.Guild.Id);

            int argPos = 0;

            if (msg.HasStringPrefix(guild.Prefix, ref argPos) ||
                msg.HasMentionPrefix(DEABot.Client.CurrentUser, ref argPos))
            {
                var result = await DEABot.CommandService.ExecuteAsync(Context, argPos);
                if (!result.IsSuccess)
                    await ErrorHandler.HandleCommandFailureAsync(Context, result, argPos, guild);
                else
                    DEABot.Commands++;
            }
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

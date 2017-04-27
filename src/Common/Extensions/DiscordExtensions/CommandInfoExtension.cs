using Discord;
using Discord.Commands;

namespace DEA.Common.Extensions.DiscordExtensions
{
    public static class CommandInfoExtension
    {
        public static string GetUsage(this CommandInfo cmd)
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
                if (param.Type == typeof(bool)) after = ": True/False" + after;
                if (param.Type == typeof(IRole) || param.Type == typeof(IGuildUser)) before += "@";
                if (param.Type == typeof(ITextChannel)) before += "#";
                usage += $" {before}{param.Summary ?? param.Name}{after} ";
            }
            return usage;
        }
    }
}

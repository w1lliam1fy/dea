using Discord;
using Discord.Commands;

namespace DEA.Services
{
    public static class CommandHelper
    {
        public static string GetUsage(CommandInfo cmd)
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
                if (param.Type == typeof(IRole)|| param.Type == typeof(IGuildUser)) before = before + "@";
                if (param.Type == typeof(ITextChannel)) before = before + "#";
                usage += $" {before}{param.Summary ?? param.Name}{after} ";
            }
            return cmd.Name + usage;
        }
    }
}

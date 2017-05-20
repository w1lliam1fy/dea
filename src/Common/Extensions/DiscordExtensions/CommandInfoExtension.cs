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
                if (param.Type == typeof(IRole) || param.Type == typeof(IGuildUser))
                {
                    before += "@";
                }

                if (param.Type == typeof(ITextChannel))
                {
                    before += "#";
                }

                usage += $" {before}{param.Name}{after} ";
            }
            return usage;
        }

        public static string GetExample(this CommandInfo cmd)
        {
            string example = string.Empty;
            foreach (var param in cmd.Parameters)
            {
                if (param.Summary == null && param.DefaultValue == null)
                {
                    var paramExample = string.Empty;
                    if (param.Type == typeof(IGuildUser))
                    {
                        paramExample = "Sexy John#0007";
                    }
                    else if (param.Type == typeof(IRole))
                    {
                        paramExample = "Spicy Role";
                    }
                    else if (param.Type == typeof(ITextChannel))
                    {
                        paramExample = "CleanAssChannel";
                    }
                    else if (param.Type == typeof(decimal))
                    {
                        paramExample = "50";
                    }
                    else if (param.Type == typeof(int))
                    {
                        paramExample = "2";
                    }
                    else if (param.Type == typeof(string))
                    {
                        paramExample = "Oh yes baby, oh yes.";
                    }
                    else if (param.Type == typeof(ulong))
                    {
                        paramExample = "290759415362224139";
                    }
                    else
                    {
                        paramExample = "UNSPECIFIED ARGUMENT EXAMPLE";
                    }

                    example += param.IsRemainder || !paramExample.Contains(" ") ? " " + paramExample : $" \"{paramExample}\"";
                }
                else
                {
                    if (!param.IsRemainder && param.Summary != null && param.Summary.Contains(" "))
                    {
                        example += $" \"{param.Summary}\"";
                    }
                    else
                    {
                        example += $" {param.Summary ?? param.DefaultValue}";
                    }
                }    
            }
            return example;
        }
    }
}

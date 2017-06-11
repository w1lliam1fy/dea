using DEA.Common.Extensions.DiscordExtensions;
using Discord.Commands;
using System.IO;
using System.Linq;

namespace DEA.Services.Static
{
    internal static class Documentation
    {
        public static void CreateAndSave(CommandService commandService)
        {
            var commandsDocumentation =
                "All commands are catagorized by modules. Each of the following sections is a module, and to gain more information about a " +
                "specific module, you may use the `$help [Module name]` command, or simply read below.\n\nThe syntax of the command usage is:\n\n" +
                "`Optional paramater: []`\n\n`Required paramater: <>`\n\n##Table Of Contents\n"; 

            var tableOfContents = string.Empty;
            var commandInfo = string.Empty;

            var sortedModules = commandService.Modules.OrderBy(x => x.Name);

            foreach (var module in sortedModules)
            {
                tableOfContents += $"- [{module.Name}](#{module.Name.ToLower()})\n";

                commandInfo += $"\n### {module.Name}\n";

                if (!string.IsNullOrWhiteSpace(module.Summary))
                {
                    commandInfo += $"\n{module.Summary}\n\n";
                }

                commandInfo += "Command | Description | Usage\n---------------- | --------------| -------\n";

                foreach (var command in module.Commands)
                {
                    commandInfo += $"{command.Name}|{command.Summary}|`${command.Name}{command.GetUsage()}`\n";
                }
            }

            commandsDocumentation += tableOfContents + commandInfo;

            File.WriteAllText(Config.MainDirectory + @"docs\docs\commands.md", commandsDocumentation);
        }
    }
}

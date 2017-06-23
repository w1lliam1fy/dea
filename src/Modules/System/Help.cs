using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;
using DEA.Common.Extensions;
using DEA.Common.Extensions.DiscordExtensions;

namespace DEA.Modules.System
{
    public partial class System
    {
        [Command("Help")]
        [Alias("commands", "cmd", "cmds", "command")]
        [Remarks("Moderation")]
        [Summary("All command information.")]
        public async Task Help([Remainder] string commandOrModule = null)
        {
            if (commandOrModule != null)
            {
                commandOrModule = commandOrModule.ToLower();
                if (commandOrModule.StartsWith(Context.Prefix))
                {
                    commandOrModule = commandOrModule.Remove(0, Context.Prefix.Length);
                }

                foreach (var module in _commandService.Modules)
                {
                    if (module.Name.ToLower() == commandOrModule || module.Aliases.Any(x => x.ToLower() == commandOrModule))
                    {
                        var longestInModule = 0;
                        foreach (var cmd in module.Commands)
                        {
                            if (cmd.Aliases.First().Length > longestInModule)
                            {
                                longestInModule = cmd.Aliases.First().Length;
                            }
                        }

                        var moduleInfo = $"**{module.Name} Commands **: ```asciidoc\n";
                        foreach (var cmd in module.Commands)
                        {
                            moduleInfo += $"{Context.Prefix}{cmd.Aliases.First()}{new string(' ', (longestInModule + 1) - cmd.Aliases.First().Length)} :: {cmd.Summary}\n";
                        }
                        moduleInfo += "\nUse the $help command for more information on any of these commands.```";
                        await Context.Channel.SendMessageAsync(moduleInfo);
                        return;
                    }

                    var command = module.Commands.FirstOrDefault(x => x.Aliases.Any(y => y.ToLower() == commandOrModule));
                    if (command != default(CommandInfo))
                    {
                        var commmandNameUpperFirst = commandOrModule.UpperFirstChar();
                        var example = command.Parameters.Count == 0 ? string.Empty : $"**Example:** `{Context.Prefix}{commmandNameUpperFirst} {command.Remarks}`";

                        await SendAsync($"**Description:** {command.Summary}\n\n" +
                                        $"**Usage:** `{Context.Prefix}{commmandNameUpperFirst}{command.GetUsage()}`\n\n" + example,
                                        commandOrModule.UpperFirstChar());
                        return;
                    }
                }

                await ReplyAsync($"This command/module does not exist.");
            }
            else
            {
                var channel = await Context.User.GetOrCreateDMChannelAsync();

                string modules = string.Empty;
                foreach (var module in _commandService.Modules)
                {
                    modules += $"{module.Name}, ";
                }

                await channel.SendAsync(
                    $@"DEA is a multi-purpose Discord Bot mainly known for it's infamous Cash System with multiple subtleties referencing to the show Narcos, which inspired the creation of this masterpiece.

For all information about command usage and setup on your Discord Sever, view the documentation: <https://realblazeit.github.io/DEA/>

This command may be used for view the commands for each of the following modules: {modules.Remove(modules.Length - 2)}. It may also be used the view the usage of a specific command.

In order to **add DEA to your Discord Server**, click the following link: <https://discordapp.com/oauth2/authorize?client_id={Context.Guild.CurrentUser.Id}&scope=bot&permissions=410119182> 

If you have any other questions, you may join the **Official DEA Discord Server:** <https://discord.gg/gvyma7H>, a server home to infamous meme events such as insanity.",
                    "Welcome to DEA");

                await ReplyAsync($"You have been DMed with all the command information!");
            }
        }
    }
}

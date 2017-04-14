using Discord;
using Discord.Commands;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using DEA.Database.Repository;
using DEA.Services;
using DEA;
using DEA.Common;
using DEA.Services.Handlers;

namespace System.Modules
{
    public class System : DEAModule
    {

        protected override void BeforeExecute()
        {
            InitializeData();
        }

        [Command("Invite")]
        [Summary("Invite DEA to your server!")]
        public async Task Invite()
        {
            await Reply($"Click on the following link to add DEA to your server: <https://discordapp.com/oauth2/authorize?client_id={Context.Guild.CurrentUser.Id}&scope=bot&permissions=410119182>");
        }

        [Command("Information")]
        [Alias("info")]
        [Summary("Information about the DEA Cash System.")]
        public async Task Info()
        {
            string p = Prefix;

            var channel = await Context.User.CreateDMChannelAsync();

            await ResponseMethods.DM(channel, $@"In order to gain money, you must send a message that is at least {Config.MIN_CHAR_LENGTH} characters in length. There is a 30 second cooldown between each message that will give you cash. However, these rates are not fixed. For every message you send, your chatting multiplier (which increases the amount of money you get per message) is increased by {DbGuild.TempMultiplierIncreaseRate}. This rate is reset every hour.

To view your steadily increasing chatting multiplier, you may use the `{p}rate` command, and the `{p}money` command to see your cash grow. This command shows you every single variable taken into consideration for every message you send. If you wish to improve these variables, you may use investments. With the `{p}investments` command, you may pay to have *permanent* changes to your message rates. These will stack with the chatting multiplier.");

            await ResponseMethods.DM(channel, $@"Another common way of gaining money is by gambling, there are loads of different gambling commands, which can all be viewed with the `{p}help` command. You might be wondering what is the point of all these commands. This is where ranks come in. The full list of ranks may be viewed with the `{p}rank` command. Depending on how much money you have, you will get a certain rank, and mainly, gain access to more commands. As your cash stack grows, so do the quantity commands you can use:

**{Config.JUMP_REQUIREMENT.ToString("C", Config.CI)}:** `{p}jump`
**{Config.STEAL_REQUIREMENT.ToString("C", Config.CI)}:** `{p}steal`
**{Config.ROB_REQUIREMENT.ToString("C", Config.CI)}:** `{p}rob <Resources>`
**{Config.BULLY_REQUIREMENT.ToString("C", Config.CI)}:** `{p}bully`
**{Config.FIFTYX2_REQUIREMENT.ToString("C", Config.CI)}:** `{p}50x2 <Bet>`");

            await Reply($"Information about the DEA Cash System has been DMed to you!");
        }

        [Command("Modules")]
        [Alias("module")]
        [Summary("All command modules.")]
        public async Task Modules()
        {
            string modules = string.Empty;
            foreach (var module in DEABot.CommandService.Modules)
                modules += $"{module.Name}, ";
            modules = modules.Replace("DEAModule, ", string.Empty);
            await Reply("Current command modules: " + modules.Substring(0, modules.Length - 2) + ".");
            
        }

        [Command("Help")]
        [Alias("commands", "cmd", "cmds", "command")]
        [Summary("All command information.")]
        public async Task Help([Remainder] string commandOrModule = null)
        {
            string prefix = GuildRepository.FetchGuild(Context.Guild.Id).Prefix;

            if (commandOrModule != null)
            {
                commandOrModule = commandOrModule.Replace(" ", "_");
                if (commandOrModule.StartsWith(prefix)) commandOrModule = commandOrModule.Remove(0, prefix.Length);
                foreach (var module in DEABot.CommandService.Modules)
                {
                    if (module.Name.ToLower() == commandOrModule.ToLower())
                    {
                        var longestInModule = 0;
                        foreach (var cmd in module.Commands)
                            if (cmd.Aliases.First().Length > longestInModule) longestInModule = cmd.Aliases.First().Length;
                        var moduleInfo = $"**{module.Name} Commands **: ```asciidoc\n";
                        foreach (var cmd in module.Commands)
                        {
                            moduleInfo += $"{prefix}{cmd.Aliases.First()}{new string(' ', (longestInModule + 1) - cmd.Aliases.First().Length)} :: {cmd.Summary}\n";
                        }
                        moduleInfo += "```";
                        await ReplyAsync(moduleInfo);
                        return;
                    }
                }

                foreach (var module in DEABot.CommandService.Modules)
                {
                    foreach (var cmd in module.Commands)
                    {
                        foreach (var alias in cmd.Aliases)
                            if (alias.ToLower() == commandOrModule.ToLower())
                            {
                                await Send($"**Description:** {cmd.Summary}\n**Usage:** `{prefix}{CommandHandler.GetUsage(cmd, commandOrModule)}`", CommandHandler.UpperFirstChar(commandOrModule));
                                return;
                            }
                    }
                }

                await Reply($"This command/module does not exist.");
            }
            else
            {
                var channel = await Context.User.CreateDMChannelAsync();

                string modules = string.Empty;
                foreach (var module in DEABot.CommandService.Modules)
                    modules += $"{module.Name}, ";
                modules = modules.Replace("DEAModule, ", string.Empty);

                await ResponseMethods.DM(channel,
                    $@"DEA is a multi-purpose Discord Bot mainly known for it's infamous Cash System with multiple subtleties referencing to the show Narcos, which inspired the creation of this masterpiece.

For all information about command usage and setup on your Discord Sever, view the documentation: <https://realblazeit.github.io/DEA/>

This command may be used for view the commands for each of the following modules: {modules.Substring(0, modules.Length - 2)}. It may also be used the view the usage of a specific command.

In order to **add DEA to your Discord Server**, click the following link: <https://discordapp.com/oauth2/authorize?client_id={Context.Guild.CurrentUser.Id}&scope=bot&permissions=410119182> 

If you have any other questions, you may join the **Official DEA Discord Server:** <https://discord.gg/Tuptja9>, a server home to infamous meme events such as insanity.",
                    "Welcome to DEA");

                await Reply($"You have been DMed with all the command information!");
            }        
        }

        [Command("Stats")]
        [Alias("statistics")]
        [Summary("All the statistics about DEA.")]
        public async Task Stats()
        {
            var builder = new EmbedBuilder();
            using (var process = Process.GetCurrentProcess())
            {
                var uptime = (DateTime.Now - process.StartTime);
                builder.AddInlineField("Author", "John#0969")
                .AddInlineField("Shard", $"#{DEABot.Client.ShardId}/{DEABot.Credentials.ShardCount}")
                .AddInlineField("Library", $"Discord.Net {DiscordConfig.Version}")
                .AddInlineField("Servers", $"{DEABot.Client.Guilds.Count}")
                .AddInlineField("Channels", $"{DEABot.Client.Guilds.Sum(g => g.Channels.Count) + DEABot.Client.DMChannels.Count}")
                .AddInlineField("Memory", $"{(process.PrivateMemorySize64 / 1000000d).ToString("N2")} MB")
                .AddInlineField("Uptime", $"Days: {uptime.Days}\nHours: {uptime.Hours}\nMinutes: {uptime.Minutes}")
                .AddInlineField("Messages", $"{DEABot.Messages} ({(DEABot.Messages / uptime.TotalSeconds).ToString("N2")}/sec)")
                .AddInlineField("Commands Used", $"{DEABot.Commands}")
                .WithColor(Config.COLORS[new Random().Next(1, Config.COLORS.Length) - 1]);
            }
            
            var channel = await Context.User.CreateDMChannelAsync();
            await channel.SendMessageAsync(string.Empty, embed: builder);
            await Reply($"You have been DMed with all the statistics!");
        }
    }
}
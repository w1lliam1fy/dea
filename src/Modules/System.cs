using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DEA.Database.Repository;
using DEA.Resources;
using DEA.Services;

namespace System.Modules
{
    public class System : DEAModule
    {

        private Process _process;
        private CommandService _service;

        protected override void BeforeExecute()
        {
            _process = Process.GetCurrentProcess();
        }

        public System(CommandService service)
        {
            _service = service;
        }

        [Command("Invite")]
        [Remarks("Invite")]
        [Summary("Invite DEA to your server!")]
        public async Task Invite(string investString = null)
        {
            await Reply($"Click on the following link to add DEA to your server: <https://discordapp.com/oauth2/authorize?client_id={Context.Guild.CurrentUser.Id}&scope=bot&permissions=410119182>");
        }

        [Command("Information")]
        [Alias("info")]
        [Remarks("Information")]
        [Summary("Information about the DEA Cash System.")]
        public async Task Info(string investString = null)
        {
            var guild = GuildRepository.FetchGuild(Context.Guild.Id);
            string p = guild.Prefix;

            var channel = await Context.User.CreateDMChannelAsync();

            await ResponseMethods.DM(channel, $@"In order to gain money, you must send a message that is at least {Config.MIN_CHAR_LENGTH} characters in length. There is a 30 second cooldown between each message that will give you cash. However, these rates are not fixed. For every message you send, your chatting multiplier(which increases the amount of money you get per message) is increased by {guild.TempMultiplierIncreaseRate}. This rate is reset every hour.

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
        [Remarks("Modules")]
        public async Task Modules()
        {
            string modules = "";
            foreach (var module in _service.Modules) modules += $"{module.Name}, ";
            await Reply("Current command modules: " + modules.Substring(0, modules.Length - 2) + ".");
        }

        [Command("Help")]
        [Alias("commands", "cmd", "cmds", "command")]
        [Summary("All command information.")]
        [Remarks("Help [Command or Module]")]
        public async Task HelpAsync(string commandOrModule = null)
        {
            string prefix = GuildRepository.FetchGuild(Context.Guild.Id).Prefix;

            if (commandOrModule != null)
            {
                if (commandOrModule.StartsWith(prefix)) commandOrModule = commandOrModule.Remove(0, prefix.Length);
                foreach (var module in _service.Modules)
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

                foreach (var module in _service.Modules)
                {
                    foreach (var cmd in module.Commands)
                    {
                        foreach (var alias in cmd.Aliases)
                            if (alias == commandOrModule.ToLower())
                            {
                                await Send($"**Description:** {cmd.Summary}\n**Usage:** `{prefix}{cmd.Remarks}`", cmd.Name);
                                return;
                            }
                    }
                }

                await Reply($"This command/module does not exist.");
            }
            else
            {
                var channel = await Context.User.CreateDMChannelAsync();

                string modules = "";
                foreach (var module in _service.Modules) modules += $"{module.Name}, ";

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
        [Remarks("Stats")]
        [Summary("All the statistics about DEA.")]
        public async Task Info()
        {
            var uptime = (DateTime.Now - _process.StartTime);
            var application = await Context.Client.GetApplicationInfoAsync();
            var message = $@"```asciidoc
= STATISTICS =
• Memory   :: {(_process.PrivateMemorySize64 / 1000000).ToString("N2")} MB
• Uptime   :: Days: {uptime.Days}, Hours: {uptime.Hours}, Minutes: {uptime.Minutes}, Seconds: {uptime.Seconds}
• Users    :: {(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count)}
• Servers  :: {(Context.Client as DiscordSocketClient).Guilds.Count}
• Channels :: {(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Channels.Count)}
• Library  :: Discord.Net {DiscordConfig.Version}
• Runtime  :: {RuntimeInformation.FrameworkDescription} {RuntimeInformation.OSArchitecture}```";
            var channel = await Context.User.CreateDMChannelAsync();
            await channel.SendMessageAsync(message);
            await Reply($"You have been DMed with all the statistics!");
        }
    }
}
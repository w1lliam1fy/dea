using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace DEA.Modules.System
{
    public partial class System
    {
        [Command("Statistics")]
        [Alias("Stats")]
        [Summary("All the statistics about DEA.")]
        public async Task Stats()
        {
            var builder = new EmbedBuilder();
            using (var process = Process.GetCurrentProcess())
            {
                var uptime = (DateTime.Now - process.StartTime);
                builder.AddInlineField("Author", "John#0969")
                .AddInlineField("Framework", $".NET Core 1.0.3")
                .AddInlineField("Memory", $"{(process.PrivateMemorySize64 / 1000000d).ToString("N2")} MB")
                .AddInlineField("Servers", $"{Context.Client.Guilds.Count}")
                .AddInlineField("Channels", $"{Context.Client.Guilds.Sum(g => g.Channels.Count) + Context.Client.DMChannels.Count}")
                .AddInlineField("Users", $"{Context.Client.Guilds.Sum(g => g.MemberCount)}")
                .AddInlineField("Uptime", $"Days: {uptime.Days}\nHours: {uptime.Hours}\nMinutes: {uptime.Minutes}")
                .AddInlineField("Messages", $"{_statistics.MessagesRecieved} ({(_statistics.MessagesRecieved / uptime.TotalSeconds).ToString("N2")}/sec)")
                .AddInlineField("Commands Run", _statistics.CommandsRun)
                .WithColor(Config.Color());
            }

            var channel = await Context.User.CreateDMChannelAsync();
            await channel.SendMessageAsync(string.Empty, embed: builder);

            await ReplyAsync($"You have been DMed with all the statistics!");
        }
    }
}

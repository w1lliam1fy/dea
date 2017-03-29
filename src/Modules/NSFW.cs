using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Xml;

namespace DEA.Modules
{
    public class NSFW : ModuleBase<SocketCommandContext>
    {
        [Command("ChangeNSFWSettings")]
        [RequireAdmin]
        [Summary("Enables/disables NSFW commands in your server.")]
        [Remarks("ChangeNSFWSettings")]
        public async Task ChangeNSFWSettings()
        {
            using (var db = new DbContext())
            {
                
                switch ((await GuildRepository.FetchGuildAsync(Context.Guild.Id)).NSFW)
                {
                    case true:
                        await GuildRepository.ModifyAsync(x => { x.NSFW = false; return Task.CompletedTask; }, Context.Guild.Id);
                        await ReplyAsync($"{Context.User.Mention}, You have successfully disabled NSFW commands!");
                        break;
                    case false:
                        await GuildRepository.ModifyAsync(x => { x.NSFW = true; return Task.CompletedTask; }, Context.Guild.Id);
                        await ReplyAsync($"{Context.User.Mention}, You have successfully enabled NSFW commands!");
                        break;
                }
            }
        }

        [Command("SetNSFWChannel")]
        [RequireAdmin]
        [Summary("Sets a specific channel for all NSFW commands.")]
        [Remarks("SetNSFWChannel <#NSFWChannel>")]
        public async Task SetNSFWChannel(ITextChannel nsfwChannel)
        {
            using (var db = new DbContext())
            {
                
                await GuildRepository.ModifyAsync(x => { x.NSFWChannelId = nsfwChannel.Id; return Task.CompletedTask; }, Context.Guild.Id);
                var nsfwRole = Context.Guild.GetRole((await GuildRepository.FetchGuildAsync(Context.Guild.Id)).NSFWRoleId);
                if (nsfwRole != null && Context.Guild.CurrentUser.GuildPermissions.Administrator)
                {
                    await nsfwChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions().Modify(null, null, null, PermValue.Deny));
                    await nsfwChannel.AddPermissionOverwriteAsync(nsfwRole, new OverwritePermissions().Modify(null, null, null, PermValue.Allow));
                }
                await ReplyAsync($"{Context.User.Mention}, You have successfully set the NSFW channel to {nsfwChannel.Mention}.");
            }
        }

        [Command("SetNSFWRole")]
        [RequireAdmin]
        [Summary("Only allow users with a specific role to use NSFW commands.")]
        [Remarks("SetNSFWRole <@NSFWRole>")]
        public async Task SetNSFWRole(IRole nsfwRole)
        {
            if (nsfwRole.Position > Context.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                throw new Exception("You may not set the NSFW role to a role that is higher in hierarchy than DEA's highest role.");
            using (var db = new DbContext())
            {
                
                await GuildRepository.ModifyAsync(x => { x.NSFWRoleId = nsfwRole.Id; return Task.CompletedTask; }, Context.Guild.Id);
                var nsfwChannel = Context.Guild.GetChannel((await GuildRepository.FetchGuildAsync(Context.Guild.Id)).NSFWChannelId);
                if (nsfwChannel != null && Context.Guild.CurrentUser.GuildPermissions.Administrator)
                {
                    await nsfwChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions().Modify(null, null, null, PermValue.Deny));
                    await nsfwChannel.AddPermissionOverwriteAsync(nsfwRole, new OverwritePermissions().Modify(null, null, null, PermValue.Allow));
                }
                await ReplyAsync($"{Context.User.Mention}, You have successfully set the NSFW role to {nsfwRole.Mention}.");
            }
        }

        [Command("NSFW")]
        [Alias("EnableNSFW", "DisableNSFW")]
        [Summary("Enables/disables the user's ability to use NSFW commands.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Remarks("NSFW")]
        public async Task JoinNSFW()
        {
            using (var db = new DbContext())
            {
                
                var guild = await GuildRepository.FetchGuildAsync(Context.Guild.Id);
                var NSFWRole = Context.Guild.GetRole(guild.NSFWRoleId);
                if (NSFWRole == null) throw new Exception("Everyone will always be able to use NSFW commands since there has been no NSFW role that has been set.\n" + 
                                                         $"In order to change this, an administrator may use the `{guild.Prefix}SetNSFWRole` command.");
                if ((Context.User as IGuildUser).RoleIds.Any(x => x == guild.NSFWRoleId))
                {
                    await (Context.User as IGuildUser).RemoveRoleAsync(NSFWRole);
                    await ReplyAsync($"{Context.User.Mention}, You have successfully disabled your ability to use NSFW commands.");
                }
                else
                {
                    await (Context.User as IGuildUser).AddRoleAsync(NSFWRole);
                    await ReplyAsync($"{Context.User.Mention}, You have successfully enabled your ability to use NSFW commands.");
                }
            }
        }

        [Command("Tits", RunMode = RunMode.Async)]
        [Alias("titties", "tities", "boobs", "boob")]
        [RequireNSFW]
        [Summary("Motorboat that shit.")]
        [Remarks("Tits")]
        public async Task Tits()
        {
            using (var http = new HttpClient())
            {
                var obj = JArray.Parse(await http.GetStringAsync($"http://api.oboobs.ru/boobs/{new Random().Next(0, 10330)}").ConfigureAwait(false))[0];
                await Context.Channel.SendMessageAsync($"http://media.oboobs.ru/{obj["preview"]}").ConfigureAwait(false);
            }
        }

        [Command("Ass", RunMode = RunMode.Async)]
        [Alias("butt", "butts", "booty")]
        [RequireNSFW]
        [Summary("Sauce me some booty how about that.")]
        [Remarks("Ass")]
        public async Task Ass()
        {
            using (var http = new HttpClient())
            {
                var obj = JArray.Parse(await http.GetStringAsync($"http://api.obutts.ru/butts/{new Random().Next(0, 4335)}").ConfigureAwait(false))[0];
                await Context.Channel.SendMessageAsync($"http://media.obutts.ru/{obj["preview"]}").ConfigureAwait(false);
            }
        }

        [Command("Hentai", RunMode = RunMode.Async)]
        [RequireNSFW]
        [Summary("The real shit goes down with custom hentai tags.")]
        [Remarks("Hentai [tag]")]
        public async Task Gelbooru([Remainder] string tag = "")
        {
            tag = tag?.Replace(" ", "_");
            using (var http = new HttpClient())
            {
                var data = await http.GetStreamAsync($"http://gelbooru.com/index.php?page=dapi&s=post&q=index&limit=100&tags={tag}").ConfigureAwait(false);
                var doc = new XmlDocument();
                doc.Load(data);

                var node = doc.LastChild.ChildNodes[new Random().Next(0, doc.LastChild.ChildNodes.Count)];
                if (node == null) throw new Exception("No result found.");

                var url = node.Attributes["file_url"].Value;

                if (!url.StartsWith("http"))
                    url = "https:" + url;
                await ReplyAsync(url).ConfigureAwait(false);
            }
        }
    }
}

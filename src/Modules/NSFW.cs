using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DEA.Database.Repositories;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Xml;
using DEA.Common;
using DEA.Common.Preconditions;

namespace DEA.Modules
{
    public class NSFW : DEAModule
    {
        private readonly GuildRepository _guildRepo;

        public NSFW(GuildRepository guildRepo)
        {
            _guildRepo = guildRepo;
        }

        [Command("ChangeNSFWSettings")]
        [Require(Attributes.Admin)]
        [Summary("Enables/disables NSFW commands in your server.")]
        public async Task ChangeNSFWSettings()
        {
            switch (Context.DbGuild.Nsfw)
            {
                case true:
                    await _guildRepo.ModifyAsync(Context.Guild.Id, x => x.Nsfw, false);
                    await ReplyAsync($"You have successfully disabled NSFW commands!");
                    break;
                case false:
                    await _guildRepo.ModifyAsync(Context.Guild.Id, x => x.Nsfw, true);
                    await ReplyAsync($"You have successfully enabled NSFW commands!");
                    break;
            }
        }

        [Command("SetNSFWChannel")]
        [Require(Attributes.Admin)]
        [Summary("Sets a specific channel for all NSFW commands.")]
        public async Task SetNSFWChannel([Remainder] ITextChannel nsfwChannel)
        {
            await _guildRepo.ModifyAsync(Context.Guild.Id, x => x.NsfwId, nsfwChannel.Id);

            var nsfwRole = Context.Guild.GetRole(Context.DbGuild.NsfwRoleId);
            if (nsfwRole != null && Context.Guild.CurrentUser.GuildPermissions.Administrator)
            {
                await nsfwChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions().Modify(null, null, null, PermValue.Deny));
                await nsfwChannel.AddPermissionOverwriteAsync(nsfwRole, new OverwritePermissions().Modify(null, null, null, PermValue.Allow));
            }

            await ReplyAsync($"You have successfully set the NSFW channel to {nsfwChannel.Mention}.");
        }

        [Command("SetNSFWRole")]
        [Require(Attributes.Admin)]
        [Summary("Only allow users with a specific role to use NSFW commands.")]
        public async Task SetNSFWRole([Remainder] IRole nsfwRole)
        {
            if (nsfwRole.Position > Context.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                await ErrorAsync("You may not set the NSFW role to a role that is higher in hierarchy than DEA's highest role.");

            await _guildRepo.ModifyAsync(Context.Guild.Id, x => x.NsfwRoleId, nsfwRole.Id);

            var nsfwChannel = Context.Guild.GetChannel(Context.DbGuild.NsfwId);
            if (nsfwChannel != null && Context.Guild.CurrentUser.GuildPermissions.Administrator)
            {
                await nsfwChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions().Modify(null, null, null, PermValue.Deny));
                await nsfwChannel.AddPermissionOverwriteAsync(nsfwRole, new OverwritePermissions().Modify(null, null, null, PermValue.Allow));
            }

            await ReplyAsync($"You have successfully set the NSFW role to {nsfwRole.Mention}.");
        }

        [Command("NSFW")]
        [Alias("EnableNSFW", "DisableNSFW")]
        [Summary("Enables/disables the user's ability to use NSFW commands.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task JoinNSFW()
        {
            var NsfwRole = Context.Guild.GetRole(Context.DbGuild.NsfwRoleId);
            if (NsfwRole == null)
                await ErrorAsync("Everyone will always be able to use NSFW commands since there has been no NSFW role that has been set.\n" +
                                 $"In order to change this, an administrator may use the `{Context.Prefix}SetNSFWRole` command.");

            if ((Context.User as IGuildUser).RoleIds.Any(x => x == Context.DbGuild.NsfwRoleId))
            {
                await (Context.User as IGuildUser).RemoveRoleAsync(NsfwRole);
                await ReplyAsync($"You have successfully disabled your ability to use NSFW commands.");
            }
            else
            {
                await (Context.User as IGuildUser).AddRoleAsync(NsfwRole);
                await ReplyAsync($"You have successfully enabled your ability to use NSFW commands.");
            }
        }

        [Command("Tits")]
        [Alias("titties", "tities", "boobs", "boob")]
        [Require(Attributes.Nsfw)]
        [Summary("Motorboat that shit.")]
        public async Task Tits()
        {
            using (var http = new HttpClient())
            {
                var obj = JArray.Parse(await http.GetStringAsync($"http://api.oboobs.ru/boobs/{new Random().Next(0, 10330)}"))[0];
                await Context.Channel.SendMessageAsync($"http://media.oboobs.ru/{obj["preview"]}");
            }
        }

        [Command("Ass")]
        [Alias("butt", "butts", "booty")]
        [Require(Attributes.Nsfw)]
        [Summary("Sauce me some booty how about that.")]
        public async Task Ass()
        {
            using (var http = new HttpClient())
            {
                var obj = JArray.Parse(await http.GetStringAsync($"http://api.obutts.ru/butts/{new Random().Next(0, 4335)}"))[0];
                await Context.Channel.SendMessageAsync($"http://media.obutts.ru/{obj["preview"]}");
            }
        }

        [Command("Hentai")]
        [Require(Attributes.Nsfw)]
        [Summary("The real shit goes down with custom hentai tags.")]
        public async Task Gelbooru([Remainder] string tag = "")
        {
            tag = tag?.Replace(" ", "_");

            using (var http = new HttpClient())
            {
                var data = await http.GetStreamAsync($"http://gelbooru.com/index.php?page=dapi&s=post&q=index&limit=100&tags={tag}");
                var doc = new XmlDocument();
                doc.Load(data);

                var node = doc.LastChild.ChildNodes[new Random().Next(0, doc.LastChild.ChildNodes.Count)];
                if (node == null) await ErrorAsync("No result found.");

                var url = node.Attributes["file_url"].Value;

                if (!url.StartsWith("http"))
                    url = "https:" + url;
                await Context.Channel.SendMessageAsync(url);
            }
        }
    }
}

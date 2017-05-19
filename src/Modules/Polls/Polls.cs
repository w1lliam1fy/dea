using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;
using System.Collections.Generic;
using DEA.Common.Extensions.DiscordExtensions;

namespace DEA.Modules.Polls
{
    public partial class Polls
    {
        [Command("Polls")]
        [Alias("Indexes", "Index")]
        [Summary("Sends you a list of all polls in progress.")]
        public async Task Indexes()
        {
            var polls = await (await _pollRepo.Collection.FindAsync(y => y.GuildId == Context.Guild.Id)).ToListAsync();

            polls = polls.OrderBy(x => x.CreatedAt).ToList();

            if (polls.Count == 0)
            {
                ReplyError("There are no polls in progress.");
            }

            List<string> elements = new List<string>();

            if (polls.Any(x => x.CreatedByMod))
            {
                elements.Add("Polls created by moderators:\n");
            }

            for (int i = 0; i < polls.Count; i++)
            {
                if (polls[i].CreatedByMod)
                {
                    elements.Add($"{i + 1}. {polls[i].Name}\n");
                }
            }

            if (polls.Any(x => !x.CreatedByMod))
            {
                elements.Add("User polls:\n");
            }

            for (int i = 0; i < polls.Count; i++)
            {
                if (!polls[i].CreatedByMod)
                {
                    elements.Add($"{i + 1}. {polls[i].Name}\n");
                }
            }

            var channel = await Context.User.CreateDMChannelAsync();
            await channel.SendCodeAsync(elements, "Poll Indexes");

            await ReplyAsync("You have been DMed with all polls in progress.");
        }
    }
}

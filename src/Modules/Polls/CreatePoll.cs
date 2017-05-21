using Discord.Commands;
using System;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;

namespace DEA.Modules.Polls
{
    public partial class Polls
    {
        [Command("CreatePoll")]
        [Remarks("CreatePoll \"Are there any discord bots better than DEA?\" \"No~No way hoe zay~Not a chance buddy\" 1 false false")]
        [Summary("Creates a poll.")]
        public async Task AddPoll(string poll, string choices, double daysToLast = 1, bool elderOnly = false, bool modOnly = false)
        {
            var isMod = _moderationService.GetPermLevel(Context.DbGuild, Context.GUser) > 0;

            if (modOnly && !isMod)
            {
                ReplyError("Only moderators may create mod only polls.");
            }

            var choicesArray = choices.Split('~');

            if (choicesArray.Distinct().Count() != choicesArray.Length)
            {
                ReplyError("You may not have multiple choices that are identicle.");
            }

            await _pollRepo.CreatePollAsync(Context, poll, choicesArray, TimeSpan.FromDays(daysToLast), elderOnly, modOnly, isMod);

            await ReplyAsync($"You have successfully created poll #{await _pollRepo.Collection.CountAsync(y => y.GuildId == Context.Guild.Id)}.");
        }
    }
}

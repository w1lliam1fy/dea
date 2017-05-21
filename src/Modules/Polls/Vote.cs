using Discord.Commands;
using System;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;
using DEA.Common.Data;

namespace DEA.Modules.Polls
{
    public partial class Polls
    {
        [Command("Vote")]
        [Remarks("12 1")]
        [Summary("Vote on any poll.")]
        public async Task Vote(int pollIndex, int choiceIndex)
        {
            var poll = await _pollRepo.GetPollAsync(pollIndex, Context.Guild.Id);

            if (poll.VotesDocument.Any(x => x.Name == Context.User.Id.ToString()))
            {
                ReplyError("You have already voted on this poll.");
            }
            else if (poll.ElderOnly && DateTime.UtcNow.Subtract((Context.GUser).JoinedAt.Value.UtcDateTime).TotalMilliseconds <
                Config.ELDER_TIME_REQUIRED.TotalMilliseconds)
            {
                ReplyError($"You must have been in this server for more than {Config.ELDER_TIME_REQUIRED.TotalDays} days to vote on this poll.");
            }
            else if (poll.ModOnly && _moderationService.GetPermLevel(Context.DbGuild, Context.GUser) == 0)
            {
                ReplyError("Only a moderator may vote on this poll.");
            }

            string choice = null;
            try
            {
                choice = poll.Choices[choiceIndex - 1];
            }
            catch (IndexOutOfRangeException)
            {
                ReplyError("This poll choice index does not exist.");
            }

            await _pollRepo.ModifyAsync(poll, x => x.VotesDocument.Add(Context.User.Id.ToString(), choice));

            await ReplyAsync($"You have successfully voted on the \"{poll.Name}\" poll.");
        }
    }
}

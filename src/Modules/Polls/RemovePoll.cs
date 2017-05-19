using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Preconditions;

namespace DEA.Modules.Polls
{
    public partial class Polls
    {
        [Command("RemovePoll")]
        [Summary("Removes a poll.")]
        [Require(Attributes.Moderator)]
        public async Task RemovePoll(int index)
        {
            var poll = await _pollRepo.GetPollAsync(index, Context.Guild.Id);
            await _pollRepo.RemovePollAsync(index, Context.Guild.Id);

            await ReplyAsync($"You have successfully removed the \"{poll.Name}\" poll!");
        }
    }
}

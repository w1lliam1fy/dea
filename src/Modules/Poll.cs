using DEA.Common;
using DEA.Database.Repositories;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules
{
    public class Poll : DEAModule
    {
        private readonly PollRepository _pollRepo;

        public Poll(PollRepository pollRepo)
        {
            _pollRepo = pollRepo;
        }

        /// <param name="choices">Option 1~Option 2~Option 3...</param>
        [Command("AddPoll")]
        [Summary("Adds a poll to your servers list of polls.")]
        public async Task AddPoll(string poll, [Remainder] string choices)
        {
            
        }
    }
}

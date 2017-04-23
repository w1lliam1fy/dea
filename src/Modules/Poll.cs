using DEA.Common;
using DEA.Database.Repositories;
using DEA.Database.Models;
using MongoDB.Driver;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules
{
    public class Poll : DEAModule
    {
        private readonly PollRepository _pollRepo;
        private readonly IMongoCollection<Poll> _polls;

        public Poll(PollRepository pollRepo, IMongoCollection<Poll> polls)
        {
            _pollRepo = pollRepo;
            _polls = polls;
        }

        /// <param name="choices">Option 1~Option 2~Option 3...</param>
        [Command("AddPoll")]
        [Summary("Adds a poll to your servers list of polls.")]
        public async Task AddPoll(string name, double length, int permissionLevel, [Remainder] string choices)
        {
            string[] pollChoices = choices.Split(' ');
            var newPoll = await _pollrepo(Context, name, pollChoices, length);
            switch (permissionLevel)
            {
                case 1:
                    newPoll.ElderOnly = true;
                    break;
                case 2:
                    newPoll.ModOnly = true;
                    break;
                default:
                    break;
            }
            await ReplyAsync("Successfully created poll.");
        }
        [Command("RemovePoll")]
        [Summary("Removes a poll.")]
        [Require(Attributes.Moderator)]
        public async Task RemovePoll(int index)
        {
            await _pollrepo.RemovePollAsync(index, Context.Guild.Id);
            await ReplyAsync("Successfully removed poll.");
        }
        [Command("RemovePoll")]
        [Summary("Removes a poll.")]
        [Require(Attributes.Moderator)]
        public async Task RemovePoll(string name)
        {
            await _pollrepo.RemovePollAsync(name, Context.Guild.Id);
            await ReplyAsync("Successfully removed poll.");
        }
        [Command("Polls")]
        [Summary("Sends you a list of polls in progress.")]
        public async Task Polls()
        {
            var polls = await (await _polls.FindAsync(y => y.GuildId == guildId)).ToListAsync();
            if (polls.Count == 0) await ErrorAsync("There are no polls in progress.");
            List<string> elements = new List<string>();
            
            for (int i = 0; i < polls.Count; i++)
            {
                elements.Add($"{i}. {polls[i].Name}\n");
            }

            var channel = await Context.User.CreateDMChannelAsync();
            await channel.SendCodeAsync(elements, "Polls");
            await ReplyAsync("You have been DMed with all the polls in progress.");
        }
        [Command("Poll")]
        [Summary("Check the choices of any poll.")]
        public async Task PollInfo(int index)
        {
            var poll = await _pollrepo.FetchePollAsync(index, Context.Guild.Id);
            string choices = string.Empty;
            int position = 1;
            foreach(var choice in poll.Choices)
            {
                choices += $"{position}. {choice}\n";
                position++;
            }
            await SendAsync(choices, poll.Name);
        }
    }
}

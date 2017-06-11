using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace DEA.Modules.Polls
{
    public partial class Polls
    {
        [Command("Poll")]
        [Remarks("13")]
        [Summary("View the information of any poll.")]
        public async Task PollInfo(int index)
        {
            var poll = await _pollRepo.GetPollAsync(index, Context.Guild.Id);
            string description = string.Empty;

            var votes = poll.Votes();
            for (int i = 0; i < poll.Choices.Length; i++)
            {
                var choice = poll.Choices[i];
                var percentage = (votes[choice] / (double)poll.VotesDocument.ElementCount);
                if (double.IsNaN(percentage))
                {
                    percentage = 0;
                }

                description += $"{i + 1}. **{choice}:** {votes[choice]} Vote(s) ({percentage.ToString("P")})\n";
            }

            var timeRemaining = TimeSpan.FromMilliseconds(poll.Length).Subtract(DateTime.UtcNow.Subtract(poll.CreatedAt));
            if (timeRemaining.Ticks > 0)
            {
                description += $"\n**Ending:** Days: {timeRemaining.Days}, Hours: {timeRemaining.Hours}, Minutes: {timeRemaining.Minutes}, Seconds: {timeRemaining.Seconds}";
            }
            else
            {
                description += $"This poll will end very soon!";
            }

            if (poll.ModOnly)
            {
                description += "\n\n**Only moderators may vote on this poll.**";
            }
            else if (poll.ElderOnly)
            {
                description += $"\n\n**Only users that have been in this server for at least {Config.ElderTimeRequired.TotalHours} hours may vote on this poll.**";
            }

            var creator = await (Context.Guild as IGuild).GetUserAsync(poll.CreatorId);

            description += $"\n\n**Creator:** {creator}";

            await SendAsync(description, poll.Name);
        }
    }
}

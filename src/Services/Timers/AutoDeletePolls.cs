using DEA.Common.Extensions;
using DEA.Database.Models;
using DEA.Database.Repositories;
using DEA.Services.Static;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Services.Timers
{
    class AutoDeletePolls
    {
        private readonly IDependencyMap _map;
        private readonly DiscordSocketClient _client;
        private readonly IMongoCollection<Poll> _polls;

        private readonly Timer _timer;

        public AutoDeletePolls(IDependencyMap map)
        {
            _map = map;
            _polls = _map.Get<IMongoCollection<Poll>>();
            _client = _map.Get<DiscordSocketClient>();

            ObjectState StateObj = new ObjectState();

            TimerCallback TimerDelegate = new TimerCallback(DeletePolls);

            _timer = new Timer(TimerDelegate, StateObj, TimeSpan.FromMilliseconds(500), Config.AUTO_DELETE_POLLS_COOLDOWN);

            StateObj.TimerReference = _timer;
        }

        private void DeletePolls(object stateObj) =>
            Task.Run(async () =>
            {
                Logger.Log(LogSeverity.Debug, $"Timers", "Auto Delete Polls");
                var builder = Builders<Poll>.Filter;

                foreach (var poll in await (await _polls.FindAsync(builder.Empty)).ToListAsync())
                    if (TimeSpan.FromMilliseconds(poll.Length).Subtract(DateTime.UtcNow.Subtract(poll.CreatedAt)).TotalMilliseconds < 0)
                    {
                        var description = string.Empty;
                        var votes = poll.Votes();

                        for (int j = 0; j < poll.Choices.Length; j++)
                        {
                            var choice = poll.Choices[j];
                            var percentage = (votes[choice] / (double)poll.VotesDocument.ElementCount);
                            if (double.IsNaN(percentage)) percentage = 0;
                            description += $"{j + 1}. {choice}: {votes[choice]} Votes ({percentage.ToString("P")})\n";
                        }

                        await poll.CreatorId.DMAsync(_client, description, $"Final Poll Results of \"{poll.Name}\" Poll");

                        await _polls.DeleteOneAsync(y => y.Id == poll.Id);
                    }
            });

    }
}
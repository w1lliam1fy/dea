using DEA.Database.Models;
using MongoDB.Driver;
using System;
using System.Threading;

namespace DEA.Services.Timers
{
    class AutoTrivia
    {
        private Timer _timer;

        public AutoTrivia()
        {
            ObjectState StateObj = new ObjectState();

            TimerCallback TimerDelegate = new TimerCallback(TriviaTask);

            _timer = new Timer(TimerDelegate, StateObj, 0, Config.AUTO_TRIVIA_COOLDOWN);

            StateObj.TimerReference = _timer;
        }

        private async void TriviaTask(object stateObj)
        {
            //TODO: AUTOTRIVIA
            return;
            var builder = Builders<Guild>.Filter;
            foreach (var dbGuild in await (await DEABot.Guilds.FindAsync(builder.Empty)).ToListAsync())
            {
                if (dbGuild.AutoTrivia)
                {
                    var guild = DEABot.Client.GetGuild(dbGuild.Id);
                    if (guild != null)
                    {
                        int roll = new Random().Next(0, dbGuild.Trivia.ElementCount);
                        var element = dbGuild.Trivia.GetElement(roll);
                        await guild.DefaultChannel.SendMessageAsync("__**TRIVIA:**__ " + element.Name);
                    }
                }
            }
        }
    }
}
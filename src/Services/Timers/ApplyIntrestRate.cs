using DEA.Database.Models;
using DEA.Services.Static;
using Discord;
using Discord.Commands;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Services.Timers
{
    class ApplyIntrestRate
    {
        private readonly IDependencyMap _map;
        private readonly IMongoCollection<Gang> _gangs;

        private readonly Timer _timer;

        public ApplyIntrestRate(IDependencyMap map)
        {
            _map = map;
            _gangs = _map.Get<IMongoCollection<Gang>>();

            ObjectState StateObj = new ObjectState();

            TimerCallback TimerDelegate = new TimerCallback(InterestRate);

            _timer = new Timer(TimerDelegate, StateObj, TimeSpan.FromMilliseconds(250), Config.INTEREST_RATE_COOLDOWN);

            StateObj.TimerReference = _timer;
        }

        private void InterestRate(object stateObj) =>
            Task.Run(async () =>
            {
                Logger.Log(LogSeverity.Debug, $"Timers", "Interest Rate");
                var builder = Builders<Gang>.Filter;
                var updateBuilder = Builders<Gang>.Update;

                foreach (var gang in await (await _gangs.FindAsync(builder.Empty)).ToListAsync())
                    if (gang.Wealth < 10000000000000000000000m)
                        await _gangs.UpdateOneAsync(y => y.Id == gang.Id,
                            updateBuilder.Set(x => x.Wealth, Static.InterestRate.Calculate(gang.Wealth) * gang.Wealth + gang.Wealth));
            });
    }
}
using DEA.Database.Models;
using DEA.Services.Static;
using Discord.Commands;
using MongoDB.Driver;
using System.Threading;

namespace DEA.Services.Timers
{
    class ApplyIntrestRate
    {
        private IDependencyMap _map;
        private IMongoCollection<Gang> _gangs;

        private Timer _timer;

        public ApplyIntrestRate(IDependencyMap map)
        {
            _map = map;
            _gangs = _map.Get<IMongoCollection<Gang>>();

            ObjectState StateObj = new ObjectState();

            TimerCallback TimerDelegate = new TimerCallback(TimerTask);

            _timer = new Timer(TimerDelegate, StateObj, 0, Config.INTEREST_RATE_COOLDOWN);

            StateObj.TimerReference = _timer;
        }

        private async void TimerTask(object stateObj)
        {
            var builder = Builders<Gang>.Filter;
            var updateBuilder = Builders<Gang>.Update;
            foreach (var gang in await(await _gangs.FindAsync(builder.Empty)).ToListAsync())
                if (gang.Wealth < 10000000000000000000000m)
                    await _gangs.UpdateOneAsync(y => y.Id == gang.Id,
                        updateBuilder.Set(x => x.Wealth, InterestRate.Calculate(gang.Wealth) * gang.Wealth + gang.Wealth));
        }
    }
}
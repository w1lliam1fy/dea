using DEA.Database.Models;
using Discord.Commands;
using MongoDB.Driver;
using System.Threading;

namespace DEA.Services.Timers
{
    class ResetTempMultiplier
    {
        private IDependencyMap _map;
        private IMongoCollection<User> _users;

        private Timer _timer;

        public ResetTempMultiplier(IDependencyMap map)
        {
            _map = map;
            _users = _map.Get<IMongoCollection<User>>();

            ObjectState StateObj = new ObjectState();

            TimerCallback TimerDelegate = new TimerCallback(TimerTask);

            _timer = new Timer(TimerDelegate, StateObj, 0, Config.TEMP_MULTIPLIER_RESET_COOLDOWN);

            StateObj.TimerReference = _timer;
        }

        private async void TimerTask(object stateObj)
        {
            var builder = Builders<User>.Filter;
            var updateBuilder = Builders<User>.Update;
            await _users.UpdateManyAsync(builder.Empty, updateBuilder.Set(x => x.TemporaryMultiplier, 1));
        }
    }
}
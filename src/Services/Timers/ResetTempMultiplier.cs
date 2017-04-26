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
    class ResetTempMultiplier
    {
        private readonly IDependencyMap _map;
        private readonly IMongoCollection<User> _users;

        private readonly Timer _timer;

        public ResetTempMultiplier(IDependencyMap map)
        {
            _map = map;
            _users = _map.Get<IMongoCollection<User>>();

            ObjectState StateObj = new ObjectState();

            TimerCallback TimerDelegate = new TimerCallback(ResetTempMult);

            _timer = new Timer(TimerDelegate, StateObj, TimeSpan.FromMilliseconds(1250), Config.TEMP_MULTIPLIER_RESET_COOLDOWN);

            StateObj.TimerReference = _timer;
        }

        private void ResetTempMult(object stateObj) =>
            Task.Run(async () =>
            {
                Logger.Log(LogSeverity.Debug, $"Timers", "Reset Temporary Multiplier");
                var builder = Builders<User>.Filter;
                var updateBuilder = Builders<User>.Update;
                await _users.UpdateManyAsync(builder.Empty, updateBuilder.Set(x => x.TemporaryMultiplier, 1));
            });

    }
}
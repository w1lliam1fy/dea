using DEA.Common.Data;
using DEA.Database.Models;
using DEA.Database.Repositories;
using DEA.Services.Static;
using Discord;
using Discord.Commands;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Services.Timers
{
    /// <summary>
    /// Periodically resets the temporary multiplier for all users.
    /// </summary>
    class ResetTempMultiplier
    {
        private readonly IDependencyMap _map;
        private readonly UserRepository _userRepo;

        private readonly Timer _timer;

        public ResetTempMultiplier(IDependencyMap map)
        {
            _map = map;
            _userRepo = _map.Get<UserRepository>();

            ObjectState StateObj = new ObjectState();

            TimerCallback TimerDelegate = new TimerCallback(ResetTempMult);

            _timer = new Timer(TimerDelegate, StateObj, TimeSpan.FromMilliseconds(1250), Config.TEMP_MULTIPLIER_RESET_COOLDOWN);

            StateObj.TimerReference = _timer;
        }

        private void ResetTempMult(object stateObj)
        {
            Task.Run(async () =>
            {
                Logger.Log(LogSeverity.Debug, $"Timers", "Reset Temporary Multiplier");
                var builder = Builders<User>.Filter;
                var updateBuilder = Builders<User>.Update;
                await _userRepo.Collection.UpdateManyAsync(builder.Empty, updateBuilder.Set(x => x.TemporaryMultiplier, 1));
            });
        }
    }
}
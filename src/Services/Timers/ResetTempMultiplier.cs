using DEA.Database.Models;
using MongoDB.Driver;
using System.Threading;

namespace DEA.Services.Timers
{
    class ResetTempMultiplier
    {
        private Timer _timer;

        public ResetTempMultiplier()
        {
            ObjectState StateObj = new ObjectState();

            TimerCallback TimerDelegate = new TimerCallback(TimerTask);

            _timer = new Timer(TimerDelegate, StateObj, 0, Config.TEMP_MULTIPLIER_RESET_COOLDOWN);

            StateObj.TimerReference = _timer;
        }

        private async void TimerTask(object stateObj)
        {
            var builder = Builders<User>.Filter;
            await DEABot.Users.UpdateManyAsync(builder.Empty, DEABot.UserUpdateBuilder.Set(x => x.TemporaryMultiplier, 1));
        }
    }
}
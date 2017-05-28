using DEA.Database.Models;
using DEA.Database.Repositories;
using DEA.Services.Static;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Services.Timers
{
    internal sealed class AutoIntrestRate
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly GangRepository _gangRepo;
        private readonly Timer _timer;

        public AutoIntrestRate(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _gangRepo = _serviceProvider.GetService<GangRepository>();

            ObjectState StateObj = new ObjectState();

            TimerCallback TimerDelegate = new TimerCallback(ApplyInterestRate);

            _timer = new Timer(TimerDelegate, StateObj, TimeSpan.FromMilliseconds(250), Config.INTEREST_RATE_COOLDOWN);

            StateObj.TimerReference = _timer;
        }

        private void ApplyInterestRate(object stateObj)
        {
            Task.Run(async () =>
            {
                Logger.Log(LogSeverity.Debug, $"Timers", "Interest Rate");
                var updateBuilder = Builders<Gang>.Update;

                foreach (var gang in await _gangRepo.AllAsync())
                {
                    try
                    {
                        await _gangRepo.ModifyAsync(gang, x => x.Wealth += InterestRate.Calculate(x.Wealth) * x.Wealth);
                    }
                    catch (OverflowException)
                    {
                        //Ignored.
                    }
                }
            });
        }
    }
}
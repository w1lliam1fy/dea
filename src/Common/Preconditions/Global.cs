using System;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using DEA.Services;
using DEA.Common.Utilities;

namespace DEA.Common.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    internal sealed class Global : PreconditionAttribute
    {
        private IServiceProvider _serviceProvider;
        private RateLimitService _rateLimitService;
        private Statistics _statistics;
        private TimeSpan _cooldown;

        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider serviceProvider)
        {
            return Task.Run(() =>
            {
                _serviceProvider = serviceProvider;
                _rateLimitService = _serviceProvider.GetService<RateLimitService>();
                _statistics = _serviceProvider.GetService<Statistics>();

                if (!_rateLimitService.TryGet(x => x.UserId == context.User.Id && x.Global, out _cooldown))
                {
                    _rateLimitService.TryAdd(new RateLimit(context.User.Id, true, Config.UserRateLimit));

                    _statistics.CommandUsage.AddOrUpdate(command.Name, 0, (key, value) => value + 1);

                    (context as Context).Command = command;

                    return Task.FromResult(PreconditionResult.FromSuccess());
                }
                else
                {
                    return Task.FromResult(PreconditionResult.FromError(string.Empty));
                }
            });
        }
    }
}
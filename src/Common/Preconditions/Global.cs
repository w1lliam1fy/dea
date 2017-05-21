using System;
using System.Threading.Tasks;
using Discord.Commands;
using DEA.Common.Data;
using Microsoft.Extensions.DependencyInjection;
using DEA.Services;

namespace DEA.Common.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class GlobalAttribute : PreconditionAttribute
    {
        private IServiceProvider _serviceProvider;
        private RateLimitService _rateLimitService;
        private Statistics _statistics;
        private TimeSpan _cooldown;

        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _rateLimitService = _serviceProvider.GetService<RateLimitService>();
            _statistics = _serviceProvider.GetService<Statistics>();

            if (!_rateLimitService.TryGetEntry(context.User.Id, 0, "Global", out _cooldown))
            {
                _rateLimitService.Add(context.User.Id, 0, "Global", Config.USER_RATE_LIMIT);

                _statistics.CommandUsage.AddOrUpdate(command.Name, 0, (key, value) => value + 1);

                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            else
            {
                return Task.FromResult(PreconditionResult.FromError(string.Empty));
            }
        }
    }
}
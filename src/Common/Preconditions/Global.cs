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

        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider serviceProvider)
        {
            return Task.Run(() =>
            {
                _serviceProvider = serviceProvider;
                _rateLimitService = _serviceProvider.GetService<RateLimitService>();
                _statistics = _serviceProvider.GetService<Statistics>();

                if (_rateLimitService.TryGet(context.User.Id))
                {
                    return Task.FromResult(PreconditionResult.FromError(string.Empty));
                }
                else
                {
                    _rateLimitService.TryAdd(context.User.Id, new RateLimit(Config.UserRateLimit));

                    _statistics.CommandUsage.AddOrUpdate(command.Name, 0, (key, value) => value + 1);
                }

                if (_rateLimitService.TryGet(context.Channel.Id, out var pair))
                {
                    if (pair.Item1.Uses >= Config.MaxChannelRateLimitUses)
                    {
                        return Task.FromResult(PreconditionResult.FromError(string.Empty));
                    }
                    else
                    {
                        _rateLimitService.Update(context.Channel.Id, (k, v) =>
                        {
                            v.Item1.Uses++;
                            return v;
                        });
                    }
                }
                else
                {
                    _rateLimitService.TryAdd(context.Channel.Id, new RateLimit(Config.ChannelRateLimit));
                }

                (context as Context).Command = command;

                return Task.FromResult(PreconditionResult.FromSuccess());
            });
        }
    }
}
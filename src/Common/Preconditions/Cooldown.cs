using System;
using System.Threading.Tasks;
using Discord.Commands;
using DEA.Common.Extensions.DiscordExtensions;
using DEA.Common.Extensions;
using DEA.Common.Data;
using Microsoft.Extensions.DependencyInjection;
using DEA.Services;

namespace DEA.Common.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class CooldownAttribute : PreconditionAttribute
    {
        private IServiceProvider _serviceProvider;
        private RateLimitService _rateLimitService;
        private TimeSpan _cooldown;

        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _rateLimitService = _serviceProvider.GetService<RateLimitService>();

            if (!_rateLimitService.TryGetEntry(context.User.Id, context.Guild.Id, command.Name, out _cooldown))
            {
                return PreconditionResult.FromSuccess();
            }
            else
            {
                await context.Channel.SendAsync($"Hours: {_cooldown.Hours}\nMinutes: {_cooldown.Minutes}\nSeconds: {_cooldown.Seconds}", $"{command.Name.UpperFirstChar()} cooldown for {context.User}", Config.ERROR_COLOR);
                return PreconditionResult.FromError(string.Empty);
            }
        }
    }
}
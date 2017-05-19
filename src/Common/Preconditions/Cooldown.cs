using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using DEA.Common.Extensions.DiscordExtensions;
using DEA.Common.Extensions;
using DEA.Common.Data;
using DEA.Common.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace DEA.Common.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class CooldownAttribute : PreconditionAttribute
    {
        private readonly TimeSpan _cooldown;
        private List<CommandTimeout> _commandTimeouts;

        public CooldownAttribute(double period, TimeScale scale)
        {
            switch (scale)
            {
                case TimeScale.Days:
                    _cooldown = TimeSpan.FromDays(period);
                    break;
                case TimeScale.Hours:
                    _cooldown = TimeSpan.FromHours(period);
                    break;
                case TimeScale.Minutes:
                    _cooldown = TimeSpan.FromMinutes(period);
                    break;
                case TimeScale.Seconds:
                    _cooldown = TimeSpan.FromSeconds(period);
                    break;
            }
        }

        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider map)
        {
            _commandTimeouts = map.GetService<List<CommandTimeout>>();
            var commandTimeout = _commandTimeouts.FirstOrDefault(x => x.UserId == context.User.Id && x.GuildId == context.Guild.Id && x.CommandName == command.Name);

            if (commandTimeout == null)
            {
                return PreconditionResult.FromSuccess();
            }
            else
            {
                var timeSpan = _cooldown.Subtract(DateTime.UtcNow.Subtract(commandTimeout.CommandUse));

                if (timeSpan.TotalMilliseconds < 0)
                {
                    _commandTimeouts.Remove(commandTimeout);
                    return PreconditionResult.FromSuccess();
                }
                else
                {
                    await context.Channel.SendAsync($"Hours: {timeSpan.Hours}\nMinutes: {timeSpan.Minutes}\nSeconds: {timeSpan.Seconds}", $"{command.Name.UpperFirstChar()} cooldown for {context.User}", Config.ERROR_COLOR);
                    return PreconditionResult.FromError(string.Empty);
                }
            }
        }
    }

    public enum TimeScale
    {
        Days, Hours, Minutes, Seconds
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using DEA.Common.Extensions.DiscordExtensions;
using DEA.Common.Extensions;
using DEA.Common.Data;

namespace DEA.Common.Preconditions
{
    /// <summary> Sets how often a user is allowed to use this command. </summary>
    /// <remarks>This is backed by an in-memory collection
    /// and will not persist with restarts.</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class CooldownAttribute : PreconditionAttribute
    {
        private readonly uint _invokeLimit;
        private readonly bool _noLimitInDMs;
        private readonly TimeSpan _invokeLimitPeriod;
        private readonly Dictionary<ulong, CommandTimeout> _invokeTracker = new Dictionary<ulong, CommandTimeout>();

        /// <summary> Sets how often a user is allowed to use this command. </summary>
        /// <param name="times">The number of times a user may use the command within a certain period.</param>
        /// <param name="period">The amount of time since first invoke a user has until the limit is lifted.</param>
        /// <param name="measure">The scale in which the <paramref name="period"/> parameter should be measured.</param>
        /// <param name="noLimitInDMs">Set whether or not there is no limit to the command in DMs. Defaults to false.</param>
        public CooldownAttribute(uint times, double period, Scale measure, bool noLimitInDMs = false)
        {
            _invokeLimit = times;
            _noLimitInDMs = noLimitInDMs;

            //TODO: C# 7 candidate switch expression
            switch (measure)
            {
                case Scale.Days:
                    _invokeLimitPeriod = TimeSpan.FromDays(period);
                    break;
                case Scale.Hours:
                    _invokeLimitPeriod = TimeSpan.FromHours(period);
                    break;
                case Scale.Minutes:
                    _invokeLimitPeriod = TimeSpan.FromMinutes(period);
                    break;
                case Scale.Seconds:
                    _invokeLimitPeriod = TimeSpan.FromSeconds(period);
                    break;
            }
        }

        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider map)
        {
            if (context.Channel is IPrivateChannel && _noLimitInDMs)
            {
                return PreconditionResult.FromSuccess();
            }

            var now = DateTime.UtcNow;
            var timeout = (_invokeTracker.TryGetValue(context.User.Id, out var t)
                && ((now - t.FirstInvoke) < _invokeLimitPeriod))
                    ? t : new CommandTimeout(now);

            timeout.TimesInvoked++;

            if (timeout.TimesInvoked <= _invokeLimit)
            {
                _invokeTracker[context.User.Id] = timeout;
                return PreconditionResult.FromSuccess();
            }
            else
            {
                var timeSpan = _invokeLimitPeriod.Subtract(DateTime.UtcNow.Subtract(timeout.FirstInvoke));
                await context.Channel.SendAsync($"Hours: {timeSpan.Hours}\nMinutes: {timeSpan.Minutes}\nSeconds: {timeSpan.Seconds}", $"{command.Name.UpperFirstChar()} cooldown for {context.User}", Config.ERROR_COLOR);
                return PreconditionResult.FromError(string.Empty);
            }
        }

        private class CommandTimeout
        {
            public uint TimesInvoked { get; set; }
            public DateTime FirstInvoke { get; }

            public CommandTimeout(DateTime timeStarted)
            {
                FirstInvoke = timeStarted;
            }
        }
    }

    /// <summary> Sets the scale of the period parameter. </summary>
    public enum Scale
    {
        /// <summary> Period is measured in days. </summary>
        Days,

        /// <summary> Period is measured in hours. </summary>
        Hours,

        /// <summary> Period is measured in minutes. </summary>
        Minutes,

        /// <summary> Period is measured in seconds</summary>
        Seconds
    }
}
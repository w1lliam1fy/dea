using DEA.Common.Utilities;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace DEA.Services
{
    public sealed class CooldownService
    {
        private readonly ConcurrentDictionary<CommandCooldown, Timer> _cooldowns = new ConcurrentDictionary<CommandCooldown, Timer>();

        public CommandCooldown[] Cooldowns => _cooldowns.Keys.ToArray();

        public bool TryAdd(CommandCooldown Cooldown)
        {
            var timer = new Timer(Expire, Cooldown, Cooldown.Length, Cooldown.Length);

            return _cooldowns.TryAdd(Cooldown, timer);
        }

        public bool TryGet(Func<CommandCooldown, bool> filter, out TimeSpan remaining)
        {
            remaining = TimeSpan.Zero;

            var entry = _cooldowns.Keys.FirstOrDefault(filter);
            if (entry == null)
            {
                return false;
            }
            
            remaining = entry.ExpiresAt.Subtract(DateTime.UtcNow);
            return true;
        }

        private void Expire(object state)
        {
            var entry = (CommandCooldown)state;

            var timer = _cooldowns[entry];

            timer.Dispose();

            _cooldowns.TryRemove(entry, out Timer value);
        }
    }
}

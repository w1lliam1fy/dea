using DEA.Common.Utilities;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace DEA.Services
{
    public sealed class RateLimitService
    {
        private readonly ConcurrentDictionary<RateLimit, Timer> _rateLimits = new ConcurrentDictionary<RateLimit, Timer>();

        public RateLimit[] RateLimits => _rateLimits.Keys.ToArray();

        public bool TryAdd(RateLimit rateLimit)
        {
            var timer = new Timer(Expire, rateLimit, rateLimit.Cooldown, rateLimit.Cooldown);

            return _rateLimits.TryAdd(rateLimit, timer);
        }

        public bool TryGet(Func<RateLimit, bool> filter, out TimeSpan remaining)
        {
            remaining = TimeSpan.Zero;

            var entry = _rateLimits.Keys.FirstOrDefault(filter);
            if (entry == null)
            {
                return false;
            }
            
            remaining = entry.ExpiresAt.Subtract(DateTime.UtcNow);
            return true;
        }

        private void Expire(object state)
        {
            var entry = (RateLimit)state;

            var timer = _rateLimits[entry];

            timer.Dispose();

            _rateLimits.TryRemove(entry, out Timer value);
        }
    }
}

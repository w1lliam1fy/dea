using DEA.Common.Utilities;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace DEA.Services
{
    public sealed class RateLimitService
    {
        private readonly ConcurrentDictionary<ulong, Tuple<RateLimit, Timer>> _rateLimits = new ConcurrentDictionary<ulong, Tuple<RateLimit, Timer>>();

        public bool TryAdd(ulong id, RateLimit rateLimit)
        {
            var timer = new Timer(Expire, id, rateLimit.Length, rateLimit.Length);

            return _rateLimits.TryAdd(id, new Tuple<RateLimit, Timer>(rateLimit, timer));
        }

        public void Update(ulong id, Func<ulong, Tuple<RateLimit, Timer>, Tuple<RateLimit, Timer>> updateValueFactory)
        {
            _rateLimits.AddOrUpdate(id, new Tuple<RateLimit, Timer>(null, null), updateValueFactory);
        }

        public bool TryGet(ulong id)
        {
            return _rateLimits.TryGetValue(id, out Tuple<RateLimit,Timer> ignored);
        }

        public bool TryGet(ulong id, out Tuple<RateLimit, Timer> pair)
        {
            return _rateLimits.TryGetValue(id, out pair);
        }

        private void Expire(object state)
        {
            var id = (ulong)state;

            var timer = _rateLimits[id];

            timer.Item2.Dispose();

            _rateLimits.TryRemove(id, out Tuple<RateLimit, Timer> ignored);
        }
    }
}

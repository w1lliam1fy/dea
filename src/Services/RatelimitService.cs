using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace DEA.Services
{
    public class RateLimitService
    {
        private readonly SemaphoreSlim _sema = new SemaphoreSlim(1, 1);
        private readonly List<RateLimitEntry> _rateLimits = new List<RateLimitEntry>();
        private readonly Dictionary<RateLimitEntry, Timer> _timers = new Dictionary<RateLimitEntry, Timer>();

        public IReadOnlyCollection<RateLimitEntry> RateLimits => _rateLimits.ToImmutableList();

        public void Add(ulong userId, ulong guildId, string commandId, TimeSpan expiresAfter)
        {
            _sema.WaitAsync();

            try
            {
                var entry = new RateLimitEntry(userId, guildId, commandId, expiresAfter);
                var timer = new Timer(Expire, entry, expiresAfter, expiresAfter);

                _rateLimits.Add(entry);
                _timers.Add(entry, timer);
            }
            finally
            {
                _sema.Release();
            }
        }

        public bool TryGetEntry(ulong userId, ulong guildId, string commandId, out TimeSpan remaining)
        {
            _sema.Wait();

            try
            {
                remaining = TimeSpan.Zero;

                var entry = _rateLimits.FirstOrDefault(x => x.UserId == userId && x.GuildId == guildId && x.CommandId == commandId);
                if (entry == null)
                    return false;

                remaining = entry.ExpiresAt.Subtract(DateTime.UtcNow);
                return true;
            }
            finally
            {
                _sema.Release();
            }
        }

        private void Expire(object state)
        {
            var entry = (RateLimitEntry)state;

            _sema.Wait();

            try
            {
                _rateLimits.Remove(entry);
                var timer = _timers[entry];

                _timers.Remove(entry);

                timer.Dispose();
            }
            finally
            {
                _sema.Release();
            }
        }
    }

    public class RateLimitEntry
    {
        public ulong UserId { get; }
        public ulong GuildId { get; }
        public string CommandId { get; }
        public DateTime ExpiresAt { get; }

        public RateLimitEntry(ulong userId, ulong guildId, string commandId, TimeSpan duration)
        {
            UserId = userId;
            GuildId = guildId;
            CommandId = commandId;
            ExpiresAt = DateTime.UtcNow.Add(duration);
        }
    }
}

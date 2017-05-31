using System;

namespace DEA.Common.Utilities
{
    public sealed class RateLimit
    {
        public RateLimit(ulong userId, bool global, TimeSpan cooldown)
        {
            UserId = userId;
            Global = global;
            Cooldown = cooldown;
            ExpiresAt = DateTime.UtcNow.Add(cooldown);
        }

        public RateLimit(ulong userId, ulong guildId, string commandId, TimeSpan cooldown)
        {
            UserId = userId;
            GuildId = guildId;
            CommandId = commandId;
            Cooldown = cooldown;
            ExpiresAt = DateTime.UtcNow.Add(cooldown);
        }

        public DateTime ExpiresAt { get; }

        public TimeSpan Cooldown { get; }

        public ulong UserId { get; set; }

        public ulong GuildId { get; set; }

        public string CommandId { get; set; }

        public bool Global { get; set; } = false;
    }
}

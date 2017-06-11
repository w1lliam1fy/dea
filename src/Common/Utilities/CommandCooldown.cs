using System;

namespace DEA.Common.Utilities
{
    public sealed class CommandCooldown
    {
        public CommandCooldown(ulong userId, ulong guildId, string commandId, TimeSpan length)
        {
            UserId = userId;
            GuildId = guildId;
            CommandId = commandId;
            Length = length;
            ExpiresAt = DateTime.UtcNow.Add(length);
        }

        public DateTime ExpiresAt { get; }

        public TimeSpan Length { get; }

        public ulong UserId { get; }

        public ulong GuildId { get; }

        public string CommandId { get; }
    }
}

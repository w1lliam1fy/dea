using System;

namespace DEA.Common.Utilities
{
    public class CommandTimeout
    {
        public CommandTimeout(ulong userId, ulong guildId, string commandName)
        {
            UserId = userId;
            GuildId = guildId;
            CommandName = commandName;
        }

        public ulong UserId { get; }

        public ulong GuildId { get;  }
        
        public string CommandName { get; }

        public DateTime CommandUse { get; } = DateTime.UtcNow;
    }
}

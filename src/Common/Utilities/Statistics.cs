using System.Collections.Concurrent;

namespace DEA.Common.Utilities
{
    public sealed class Statistics
    {
        public uint MessagesRecieved { get; set; } = 0;

        public uint CommandsRun { get; set; } = 0;

        public ConcurrentDictionary<string, uint> CommandUsage { get; set; } = new ConcurrentDictionary<string, uint>();
    }
}

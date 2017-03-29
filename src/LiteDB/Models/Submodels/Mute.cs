using System;

namespace DEA.SQLite.Models.Submodels
{
    public class Mute
    {
        public ulong Id { get; set; }

        public ulong UserId { get; set; }

        public TimeSpan MuteLength { get; set; } = TimeSpan.FromDays(1);

        public DateTimeOffset MutedAt { get; set; } = DateTimeOffset.Now;
    }
}
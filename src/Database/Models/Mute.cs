using System;

namespace DEA.Database.Models
{
    public partial class Mute
    {
        public int Id { get; set; }

        public TimeSpan MuteLength { get; set; }

        public DateTimeOffset MutedAt { get; set; }

        public decimal UserId { get; set; }

        public virtual Guild Guild { get; set; }
    }
}
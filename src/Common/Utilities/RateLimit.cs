using System;

namespace DEA.Common.Utilities
{
    public sealed class RateLimit
    {
        public RateLimit(TimeSpan length)
        {
            Length = length;
            ExpiresAt = DateTime.UtcNow.Add(length);
        }

        public int Uses { get; set; }

        public DateTime ExpiresAt { get; }

        public TimeSpan Length { get; }
    }
}

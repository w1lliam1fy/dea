using System;

namespace DEA.Database.Models
{
    public partial class User
    {
        public int Id { get; set; }

        public decimal UserId { get; set; }

        public double Cash { get; set; } = 0.0;

        public double InvestmentMultiplier { get; set; } = 1.0;

        public double TemporaryMultiplier { get; set; }

        //Cooldowns

        public DateTimeOffset Whore { get; set; } = DateTimeOffset.Now.AddYears(-1);

        public DateTimeOffset Withdraw { get; set; } = DateTimeOffset.Now.AddYears(-1);

        public DateTimeOffset Jump { get; set; } = DateTimeOffset.Now.AddYears(-1);

        public DateTimeOffset Message { get; set; } = DateTimeOffset.Now.AddYears(-1);

        public TimeSpan MessageCooldown { get; set; } = TimeSpan.FromSeconds(30);

        public DateTimeOffset Rob { get; set; } = DateTimeOffset.Now.AddYears(-1);

        public DateTimeOffset Steal { get; set; } = DateTimeOffset.Now.AddYears(-1);

        public virtual Guild Guild { get; set; }
    }
}

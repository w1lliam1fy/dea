using System;

namespace DEA.SQLite.Models.Submodels
{
    public class Cooldowns
    {
        public DateTimeOffset Message { get; set; } = DateTimeOffset.Now.AddYears(-1);

        public DateTimeOffset Whore { get; set; } = DateTimeOffset.Now.AddYears(-1);

        public DateTimeOffset Jump { get; set; } = DateTimeOffset.Now.AddYears(-1);

        public DateTimeOffset Steal { get; set; } = DateTimeOffset.Now.AddYears(-1);

        public DateTimeOffset Rob { get; set; } = DateTimeOffset.Now.AddYears(-1);

        public DateTimeOffset Withdraw { get; set; } = DateTimeOffset.Now.AddYears(-1);
    }
}

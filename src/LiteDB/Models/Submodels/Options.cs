namespace DEA.SQLite.Models.Submodels
{
    public class Options
    {
        public bool Nsfw { get; set; } = false;

        public double JumpRequirement { get; set; } = 500.0;

        public double StealRequirement { get; set; } = 2500.0;

        public double RobRequirement { get; set; } = 5000.0;

        public double BullyRequirement { get; set; } = 10000.0;

        public double FiftyX2Requirement { get; set; } = 25000.0;
    }
}

namespace DEA.Common.Items
{
    public abstract partial class Weapon : CrateItem
    {
        public abstract int Damage { get; set; }

        public abstract int Accuracy { get; set; }

        public override int CrateOdds { get; set; }
    }
}

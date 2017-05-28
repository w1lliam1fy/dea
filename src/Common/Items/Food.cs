namespace DEA.Common.Items
{
    public abstract partial class Food : Item
    {
        public abstract int Health { get; set; }

        public abstract int AcquireOdds { get; set; }
    }
}

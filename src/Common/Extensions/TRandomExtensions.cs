using Troschuetz.Random;

namespace DEA.Common.Extensions
{
    public static class TRandomExtensions
    {
        public static decimal NextDecimal(this TRandom random, decimal minValue, decimal maxValue)
        {
            return (decimal)random.NextDouble((double)minValue, (double)maxValue);
        }

        public static int Roll(this TRandom random)
        {
            return random.Next(1, 101);
        }
    }
}

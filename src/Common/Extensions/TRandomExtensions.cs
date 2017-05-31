using Troschuetz.Random;

namespace DEA.Common.Extensions
{
    public static class TRandomExtensions
    {
        public static decimal NextDecimal(this TRandom random, double minValue, double maxValue)
        {
            return (decimal)random.NextDouble(minValue, maxValue);
        }

        public static int Roll(this TRandom random)
        {
            return random.Next(1, 101);
        }
    }
}

using System;
using System.Security.Cryptography;

namespace DEA.Services.Static
{
    internal static class CryptoRandom
    {
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public static void GetBytes(byte[] buffer)
        {
            _rng.GetBytes(buffer);
        }

        public static double NextDouble()
        {
            byte[] b = new byte[4];
            GetBytes(b);
            return (double)BitConverter.ToUInt32(b, 0) / UInt32.MaxValue;
        }

        public static decimal NextDecimal(decimal minValue, decimal maxValue)
        {
            decimal range = maxValue - minValue;
            return (decimal)NextDouble() * range + minValue;
        }

        public static int Next(int minValue, int maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException("maxValue", "The maximum value of the random number must be larger than the minimum value.");
            }

            if (minValue == maxValue)
            {
                return minValue;
            }

            uint scale = uint.MaxValue;
            while (scale == uint.MaxValue)
            {
                byte[] bytes = new byte[4];
                GetBytes(bytes);

                scale = BitConverter.ToUInt32(bytes, 0);
            }

            return (int)(minValue + (1 + maxValue - minValue) * (scale / (double)uint.MaxValue));
        }

        public static int Next(int maxValue)
        {
            return Next(1, maxValue);
        }

        public static int Next()
        {
            return Next(1, Int32.MaxValue);
        }
    }
}
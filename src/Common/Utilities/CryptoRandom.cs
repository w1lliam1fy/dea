using System;
using System.Security.Cryptography;

namespace DEA.Common.Utilities
{
    ///<summary>
    /// Represents a pseudo-random number generator, a device that produces random data.
    ///</summary>
    internal sealed class CryptoRandom : RandomNumberGenerator
    {
        private static readonly RandomNumberGenerator _rng = Create();

        ///<summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        ///</summary>
        ///<param name=”buffer”>An array of bytes to contain random numbers.</param>
        public override void GetBytes(byte[] buffer)
        {
            _rng.GetBytes(buffer);
        }

        ///<summary>
        /// Returns a random double between 0 and 1.
        ///</summary>
        public double NextDouble()
        {
            byte[] b = new byte[4];
            GetBytes(b);
            var result = (double)BitConverter.ToUInt32(b, 0) / UInt32.MaxValue;
            return result == 0 ? result + Double.Epsilon : result == 1 ? result - Double.Epsilon : result;
        }

        ///<summary>
        /// Returns a random number within the specified range.
        ///</summary>
        ///<param name=”minValue”>The inclusive lower bound of the random number returned.</param>
        ///<param name=”maxValue”>The inclusive upper bound of the random number returned.</param>
        public int Next(int minValue, int maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException("maxValue", "The maximum value of the random number must be smaller than the minimum value.");
            }

            double range = 1 + maxValue - minValue;
            return (int)Math.Floor(NextDouble() * range) + minValue;
        }

        ///<summary>
        /// Returns a random positive integer.
        ///</summary>
        public int Next()
        {
            return Next(0, Int32.MaxValue);
        }

        ///<summary>
        /// Returns a random positive integer between 0 and <paramref name="maxValue"/>.
        ///</summary>
        ///<param name=”maxValue”>The inclusive upper bound of the random number returned. <paramref name="maxValue"/> must be greater than or equal 0</param>
        public int Next(int maxValue)
        {
            return Next(0, maxValue);
        }
    }
}
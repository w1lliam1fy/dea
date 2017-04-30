using System.Globalization;

namespace DEA.Common.Extensions
{
    public static class DecimalExtension
    {
        /// <summary>
        /// Stringifies the decimal formatted to a currency compatible with negative numbers.
        /// </summary>
        /// <returns>Formatted currency string.</returns>
        public static string USD(this decimal cash)
        {
            return cash.ToString("C", new CultureInfo("en-CA"));
        }
    }
}

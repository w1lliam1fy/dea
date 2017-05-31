using System.Globalization;

namespace DEA.Common.Extensions
{
    public static class NumericalExtensions
    {
        private static readonly CultureInfo _cultureInfo = new CultureInfo("en-CA");

        public static string USD(this decimal cash)
        {
            return cash.ToString("C", _cultureInfo);
        }

        public static string USD(this int cash)
        {
            return cash.ToString("C", _cultureInfo);
        }
    }
}

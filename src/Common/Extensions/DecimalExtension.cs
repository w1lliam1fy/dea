using System.Globalization;

namespace DEA.Common.Extensions
{
    public static class DecimalExtension
    {
        public static string USD(this decimal cash)
            => cash.ToString("C", new CultureInfo("en-CA"));
    }
}

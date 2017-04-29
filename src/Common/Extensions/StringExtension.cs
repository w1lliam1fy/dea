namespace DEA.Common.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Sets the first character of the string to uppercase while setting the rest to lowercase.
        /// </summary>
        /// <returns>String with the first character capitalized.</returns>
        public static string UpperFirstChar(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;

            char[] a = s.ToLower().ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }
}

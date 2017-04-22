namespace DEA.Common.Extensions
{
    public static class StringExtension
    {
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

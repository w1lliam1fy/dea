using System.Text.RegularExpressions;

namespace DEA.Common.Extensions
{
    public static class StringExtension
    {
        public static string UpperFirstChar(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }

            char[] a = s.ToLower().ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static bool SimilarTo(this string s, string s2, int stringLength1 = 0, int stringLength2 = 5, int stringLength3 = 15)
        {
            s = s.ToLower();
            s2 = s2.ToLower();

            if (s == s2)
            {
                return true;
            }

            var distance = LevenshteinDistance.Compute(s, s2);

            if (s.Length >= stringLength1 && s.Length < stringLength2)
            {
                if (distance <= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (s.Length >= stringLength2 && s.Length < stringLength3)
            {
                if (distance <= 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (s.Length >= stringLength3)
            {
                if (distance <= 3)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public static string SplitCamelCase(this string s)
        {
            return Regex.Replace(s, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }
    }
}

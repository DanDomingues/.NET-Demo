using System.Globalization;

namespace Demo.Utility
{
    public static class GenericUtility
    {
        public static string FormatAsUSD(this double amount)
        {
            return amount.ToString("c", CultureInfo.GetCultureInfo("en-US"));
        }

        public static bool EqualsAny(this string s, params string[] values)
        {
            return values.Any(s.Equals);
        }
    }
}
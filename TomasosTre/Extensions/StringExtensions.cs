using System;

namespace TomasosTre.Extensions
{
    public static class StringExtensions
    {
        public static DateTime ToDateTime(this string str)
        {
            var year = str.Substring(0, str.IndexOf('-'));
            var month = str.Substring(str.IndexOf('-') + 1);
            return new DateTime(Int32.Parse(year), Int32.Parse(month) + 1, 1);
        }
    }
}

using System;
using System.Text.RegularExpressions;

namespace TomasosTre.Extensions
{
    public static class StringExtensions
    {
        public static DateTime ToExpiryDate(this string str)
        {
            // Regex check for Chrome, to work on browsers not supporting Month input
            // Maybe change Month input back to two number inputs?
            var isChrome = Regex.IsMatch(str, @"^\d{4}\-\d{2}$");
            string year, month;
            if (isChrome)
            {
                year = str.Substring(0, str.IndexOf('-'));
                month = str.Substring(str.IndexOf('-') + 1);
            }
            else
            {
                month = str.Substring(0, str.IndexOf(' '));
                year = str.Substring(str.IndexOf(' ') + 1);
            }

            // Cards expire at the end of provided month, so add 1 to the month number
            // This is only called from card expiry check, so doing two things in this method is acceptable (sort of)
            if (Int32.Parse(month) == 12)
            {
                month = "1";
                year = (Int32.Parse(year) + 1).ToString();
            }
            else
            {
                month = (Int32.Parse(month) + 1).ToString();
            }

            return new DateTime(Int32.Parse(year), Int32.Parse(month), 1);
        }

        public static DateTime ToDate(this string str)
        {
            // Regex check for Chrome, to work on browsers not supporting Month input
            // Maybe change Month input back to two number inputs?
            var isChrome = Regex.IsMatch(str, @"^\d{4}\-\d{2}$");
            string year, month;
            if (isChrome)
            {
                year = str.Substring(0, str.IndexOf('-'));
                month = str.Substring(str.IndexOf('-') + 1);
            }
            else
            {
                month = str.Substring(0, str.IndexOf(' '));
                year = str.Substring(str.IndexOf(' ') + 1);
            }

            return new DateTime(Int32.Parse(year), Int32.Parse(month), 1);
        }
    }
}

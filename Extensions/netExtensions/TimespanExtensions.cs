using System;
using System.Text;

namespace Extensions.netExtensions
{
    public static class TimespanExtensions
    {
        public static string ToNiceTimespanString(this TimeSpan ts, int significantDigits = 2, string zeroValue = "Now")
        {
            if (significantDigits == 0)
            {
                return ts.ToString();
            }

            StringBuilder sb = new StringBuilder();
            if (significantDigits > 0 && ts.Days > 0)
            {
                sb.Append($"{ts.Days} days ");
                significantDigits--;
            }
            if (significantDigits > 0 && ts.Hours > 0)
            {
                sb.Append($"{ts.Hours} hours ");
                significantDigits--;
            }
            if (significantDigits > 0 && ts.Minutes > 0)
            {
                sb.Append($"{ts.Minutes} minutes ");
                significantDigits--;
            }
            if (significantDigits > 0 && ts.Seconds > 0)
            {
                sb.Append($"{ts.Seconds} seconds");
            }
            else if (significantDigits > 0 && ts.TotalSeconds < 1)
            {
                sb.Append(zeroValue);
            }
            return sb.ToString();
        }

    }
}

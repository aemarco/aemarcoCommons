using System;

namespace Extensions.NumberExtensions
{
    public static class RangeExtensions
    {
        public static double LimitToRange(this double value, double inclusiveMinimum, double inclusiveMaximum)
        {
            return Math.Min(inclusiveMaximum, Math.Max(inclusiveMinimum, value));
        }

        public static int LimitToRange(this int value, int inclusiveMinimum, int inclusiveMaximum)
        {
            return Math.Min(inclusiveMaximum, Math.Max(inclusiveMinimum, value));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace aemarcoCommons.Extensions.TimeExtensions
{
    public static class CalculationExtensions
    {
        public static TimeSpan Sum<T>(this IEnumerable<T> entries, Func<T, TimeSpan> selector)
        {
            return entries.Select(selector).Sum();
        }
        public static TimeSpan Sum(this IEnumerable<TimeSpan> entries)
        {
            return entries.Aggregate(TimeSpan.Zero, (t1, t2) => t1 + t2);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace aemarcoCommons.Extensions.TimeExtensions;

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



    public static int ToAgeYears(this DateTimeOffset value, DateTimeOffset? target = null)
    {
        target = target ?? DateTimeOffset.Now;

        int age = target.Value.Year - value.Year;
        if (value.AddYears(age) > target.Value)
            age--;
        return age;
    }





}
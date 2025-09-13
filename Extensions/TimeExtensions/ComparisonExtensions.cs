using System;

namespace aemarcoCommons.Extensions.TimeExtensions;

public static class ComparisonExtensions
{
    /// <summary>
    /// Checks if given timeStamp lies in the future
    /// </summary>
    /// <param name="timeStamp">timeStamp to check</param>
    /// <returns>true if it´s in the future</returns>
    public static bool IsFuture(this DateTimeOffset timeStamp)
    {
        return DateTimeOffset.Now < timeStamp;
    }

    /// <summary>
    /// Checks if given timeStamp lies in the past
    /// </summary>
    /// <param name="timeStamp">timeStamp to check</param>
    /// <returns>true if it´s in the past</returns>
    public static bool IsPast(this DateTimeOffset timeStamp)
    {
        return timeStamp < DateTimeOffset.Now;
    }

    /// <summary>
    /// Checks if given timeStamp lies more than given distance in the future
    /// </summary>
    /// <param name="timeStamp">timeStamp to check</param>
    /// <param name="minimumDistance">distance to check</param>
    /// <returns>true if it´s at least given distance in the future</returns>
    public static bool StillIsFuture(this DateTimeOffset timeStamp, TimeSpan minimumDistance)
    {
        return timeStamp.Subtract(minimumDistance).IsFuture();
    }

    /// <summary>
    /// Supposed to be used on timestamps in the past. Checks if given timeStamp falls inside given distance
    /// </summary>
    /// <param name="timeStamp">timeStamp to check</param>
    /// <param name="timeSpan"> distance to check</param>
    /// <returns>true if younger than given distance</returns>
    public static bool IsYoungerThan(this DateTimeOffset timeStamp, TimeSpan timeSpan)
    {
        return timeStamp.Add(timeSpan).IsFuture();
    }

    /// <summary>
    /// Supposed to be used on timestamps in the past. Checks if given timeStamp was at least given distance in the past 
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <param name="timeSpan"></param>
    /// <returns>true if older than given distance</returns>
    public static bool IsOlderThan(this DateTimeOffset timeStamp, TimeSpan timeSpan)
    {
        return timeStamp.Add(timeSpan).IsPast();
    }


}
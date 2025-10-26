using aemarcoCommons.Extensions.TimeExtensions;

namespace aemarcoCommons.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Converts a <see cref="DateTime"/> to a <see cref="DateTimeOffset"/>.
    /// <para>
    /// - If <see cref="DateTime.Kind"/> is <see cref="DateTimeKind.Utc"/>, the offset will be zero (UTC).  
    /// - If <see cref="DateTime.Kind"/> is <see cref="DateTimeKind.Local"/>, the offset will be the system local offset.  
    /// - If <see cref="DateTime.Kind"/> is <see cref="DateTimeKind.Unspecified"/>, it is assumed to be local time and the offset will match the system local offset.
    /// </para>
    /// This ensures that UTC, local, and unspecified <see cref="DateTime"/> values are converted correctly to <see cref="DateTimeOffset"/>.
    /// </summary>
    public static DateTimeOffset ToDateTimeOffset(this DateTime timeStamp)
    {
        var result = timeStamp.Kind switch
        {
            DateTimeKind.Utc => new DateTimeOffset(timeStamp, TimeSpan.Zero),
            DateTimeKind.Local or DateTimeKind.Unspecified => new DateTimeOffset(timeStamp, TimeZoneInfo.Local.GetUtcOffset(timeStamp)),
            _ => throw new ArgumentOutOfRangeException(nameof(timeStamp))
        };
        return result;
    }

    /// <summary>
    /// Determines whether the specified <see cref="DateTime"/> lies in the future relative to now.
    /// </summary>
    /// <param name="timeStamp">The <see cref="DateTime"/> to check.</param>
    /// <returns>True if the timestamp is in the future; otherwise, false.</returns>
    public static bool IsFuture(this DateTime timeStamp)
    {
        return timeStamp.ToDateTimeOffset().IsFuture();
    }

    /// <summary>
    /// Determines whether the specified <see cref="DateTime"/> lies in the past relative to now.
    /// </summary>
    /// <param name="timeStamp">The <see cref="DateTime"/> to check.</param>
    /// <returns>True if the timestamp is in the past; otherwise, false.</returns>
    public static bool IsPast(this DateTime timeStamp)
    {
        return timeStamp.ToDateTimeOffset().IsPast();
    }

    /// <summary>
    /// Determines whether the specified <see cref="DateTime"/> lies at least a given <paramref name="minimumDistance"/> in the future.
    /// </summary>
    /// <param name="timeStamp">The <see cref="DateTime"/> to check.</param>
    /// <param name="minimumDistance">The minimum <see cref="TimeSpan"/> into the future.</param>
    /// <returns>True if the timestamp is at least the specified distance in the future; otherwise, false.</returns>
    public static bool StillIsFuture(this DateTime timeStamp, TimeSpan minimumDistance)
    {
        return timeStamp.ToDateTimeOffset().StillIsFuture(minimumDistance);
    }

    /// <summary>
    /// Determines whether the specified <see cref="DateTime"/> in the past is less than a given <paramref name="timeSpan"/> old.
    /// </summary>
    /// <param name="timeStamp">The <see cref="DateTime"/> to check.</param>
    /// <param name="timeSpan">The <see cref="TimeSpan"/> threshold.</param>
    /// <returns>True if the timestamp is younger than the specified distance; otherwise, false.</returns>
    public static bool IsYoungerThan(this DateTime timeStamp, TimeSpan timeSpan)
    {
        return timeStamp.ToDateTimeOffset().IsYoungerThan(timeSpan);
    }

    /// <summary>
    /// Determines whether the specified <see cref="DateTime"/> in the past is at least a given <paramref name="timeSpan"/> old.
    /// </summary>
    /// <param name="timeStamp">The <see cref="DateTime"/> to check.</param>
    /// <param name="timeSpan">The <see cref="TimeSpan"/> threshold.</param>
    /// <returns>True if the timestamp is older than the specified distance; otherwise, false.</returns>
    public static bool IsOlderThan(this DateTime timeStamp, TimeSpan timeSpan)
    {
        return timeStamp.ToDateTimeOffset().IsOlderThan(timeSpan);
    }


}

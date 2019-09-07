using System;

namespace Extensions.netExtensions
{
    public static class DateTimeOffsetExtensions
    {
        public static bool IsYoungerThan(this DateTimeOffset timeStamp, TimeSpan duration)
        {
            return timeStamp.Add(duration) > DateTimeOffset.Now;
        }

        public static bool IsOlderThan(this DateTimeOffset timeOffset, TimeSpan duration)
        {
            return !timeOffset.IsYoungerThan(duration);
        }

    }
}

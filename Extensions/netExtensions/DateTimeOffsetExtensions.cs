using System;

namespace Extensions.netExtensions
{
    public static class DateTimeOffsetExtensions
    {
        public static bool IsYoungerThan(this DateTimeOffset timeStamp, TimeSpan timeSpan)
        {
            return timeStamp.Add(timeSpan) > DateTimeOffset.Now;
        }

        public static bool IsOlderThan(this DateTimeOffset timeStamp, TimeSpan timeSpan)
        {
            return timeStamp.Add(timeSpan) < DateTimeOffset.Now;
        }

    }
}

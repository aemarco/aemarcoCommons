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

        public static bool IsInFuture(this DateTimeOffset timeStamp)
        {
            return DateTimeOffset.Now < timeStamp;

        }

        public static bool IsInFuture(this DateTimeOffset timeStamp, TimeSpan minimumDistance)
        {
            return timeStamp.IsInFuture() && timeStamp.Add(-minimumDistance).IsInFuture();

        }




    }
}

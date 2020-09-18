using System;

namespace aemarcoCommons.Extensions.TimeExtensions
{
    public static class ComparisonExtensions
    {

        public static bool IsFuture(this DateTimeOffset timeStamp)
        {
            return DateTimeOffset.Now < timeStamp;
        }
        public static bool IsPast(this DateTimeOffset timeStamp)
        {
            return timeStamp < DateTimeOffset.Now;
        }


        public static bool StillIsFuture(this DateTimeOffset timeStamp, TimeSpan minimumDistance)
        {
            return timeStamp.Subtract(minimumDistance).IsFuture();
        }


        



        public static bool IsYoungerThan(this DateTimeOffset timeStamp, TimeSpan timeSpan)
        {
            return timeStamp.Add(timeSpan).IsFuture();
        }

        public static bool IsOlderThan(this DateTimeOffset timeStamp, TimeSpan timeSpan)
        {
            return timeStamp.Add(timeSpan).IsPast();
        }


    }
}

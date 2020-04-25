using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Extensions.netExtensions
{
    public static class DateTimeOffsetExtensions
    {
        [Obsolete("Use TimeExtensions instead")]
        public static bool IsYoungerThan(this DateTimeOffset timeStamp, TimeSpan timeSpan)
        {
            return timeStamp.Add(timeSpan) > DateTimeOffset.Now;
        }

        [Obsolete("Use TimeExtensions instead")]
        public static bool IsOlderThan(this DateTimeOffset timeStamp, TimeSpan timeSpan)
        {
            return timeStamp.Add(timeSpan) < DateTimeOffset.Now;
        }





        [Obsolete("Use TimeExtensions instead")]
        public static bool IsInFuture(this DateTimeOffset timeStamp)
        {
            return DateTimeOffset.Now < timeStamp;

        }

        [Obsolete("Use TimeExtensions instead")]
        public static bool IsInFuture(this DateTimeOffset timeStamp, TimeSpan minimumDistance)
        {
            return timeStamp.IsInFuture() && timeStamp.Add(-minimumDistance).IsInFuture();

        }




        [Obsolete("Use TimeExtensions instead")]
        public static async Task WaitTill(this DateTimeOffset target, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (target.IsInFuture())
            {
                var toWait = target - DateTimeOffset.Now;
                try { await Task.Delay(toWait, token); }
                catch (TaskCanceledException) { }
            }
            token.ThrowIfCancellationRequested();
        }

    }
}

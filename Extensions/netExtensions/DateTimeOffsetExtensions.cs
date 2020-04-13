using System;
using System.Threading;
using System.Threading.Tasks;

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

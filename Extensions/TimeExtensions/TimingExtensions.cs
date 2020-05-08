using System;
using System.Threading;
using System.Threading.Tasks;

namespace Extensions.TimeExtensions
{
    public static class TimingExtensions
    {
        public static async Task WaitTill(this DateTimeOffset target, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (target.IsFuture())
            {
                var toWait = target - DateTimeOffset.Now;
                try { await Task.Delay(toWait, token); }
                catch (TaskCanceledException) { }
            }
            token.ThrowIfCancellationRequested();
        }




    }
}

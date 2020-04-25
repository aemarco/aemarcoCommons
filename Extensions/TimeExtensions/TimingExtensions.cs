using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Extensions.netExtensions;

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

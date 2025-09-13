using System;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.Extensions.TimeExtensions;

public static class TimingExtensions
{
    public static async Task WaitTill(this DateTimeOffset target, CancellationToken token)
    {
        if (target.IsFuture())
        {
            var toWait = target - DateTimeOffset.Now;
            if (toWait.TotalMilliseconds > 0)
            {
                await Task.Delay(toWait, token)
                    .ConfigureAwait(false);
            }
        }
    }




}
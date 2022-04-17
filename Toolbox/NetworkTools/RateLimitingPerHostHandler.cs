using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.Toolbox.NetworkTools
{
    public class RateLimitingPerHostHandler : DelegatingHandler
    {
        private readonly ConcurrentDictionary<string, int> _rateLimits = new ConcurrentDictionary<string, int>();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var host = request.RequestUri.Host;
            var delay = 0;
            if (_rateLimits.ContainsKey(host) &&
                _rateLimits.TryGetValue(host, out delay))
            {
                await Task.Delay(delay, cancellationToken)
                    .ConfigureAwait(false);
            }


            HttpResponseMessage response = await base.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);
            if ((int)response.StatusCode == 429)
            {
                _rateLimits.AddOrUpdate(
                    host,
                    100,
                    (key, val) => Math.Min(delay + 100, 1000));

                response.EnsureSuccessStatusCode();
            }
            return response;
        }



    }
}

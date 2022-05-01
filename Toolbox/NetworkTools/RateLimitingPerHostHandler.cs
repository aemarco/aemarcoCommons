using aemarcoCommons.Extensions.TimeExtensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.Toolbox.NetworkTools
{
    public class RateLimitingPerHostHandler : DelegatingHandler
    {
        private readonly ILogger<RateLimitingPerHostHandler> _logger;
        public RateLimitingPerHostHandler(ILogger<RateLimitingPerHostHandler> logger)
        {
            _logger = logger;
        }



        //private readonly ConcurrentDictionary<string, int> _rateLimits = new ConcurrentDictionary<string, int>();
        //protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        //{
        //    var host = request.RequestUri.Host;
        //    var delay = 0;
        //    if (_rateLimits.ContainsKey(host) &&
        //        _rateLimits.TryGetValue(host, out delay))
        //    {
        //        await Task.Delay(delay, cancellationToken)
        //            .ConfigureAwait(false);
        //    }


        //    HttpResponseMessage response = await base.SendAsync(request, cancellationToken)
        //        .ConfigureAwait(false);

        //    //we skip all but 429
        //    if ((int)response.StatusCode != 429)
        //        return response;

        //    _rateLimits.AddOrUpdate(
        //        host,
        //        100,
        //        (key, val) => Math.Min(delay + 100, 1000));
        //    if (_rateLimits.TryGetValue(host, out delay))
        //    {
        //        _logger.LogWarning("Rate limiting {host} with new delay {delay}", host, delay);
        //    }

        //    return await SendAsync(request, cancellationToken)
        //        .ConfigureAwait(false);
        //}



        private readonly ConcurrentDictionary<string, RateLimitingInfo> _rateLimits = new ConcurrentDictionary<string, RateLimitingInfo>();
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var host = request.RequestUri.Host;
            RateLimitingInfo info = null;
            if (_rateLimits.ContainsKey(host) &&
                _rateLimits.TryGetValue(host, out info))
            {
                var earliest = info.LastRequest.AddMilliseconds(info.Delay);
                await earliest.WaitTill(cancellationToken)
                    .ConfigureAwait(false);
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            //we remember timestamp for rate limited hosts
            if (info != null)
                info.LastRequest = DateTimeOffset.Now;

            //we skip all but 429
            if ((int)response.StatusCode != 429)
                return response;

            if (info == null)
                info = new RateLimitingInfo();

            info.Delay = Math.Min(info.Delay + 100, 1000);
            info.LastRequest = DateTimeOffset.Now;

            _rateLimits.AddOrUpdate(host, info, (key, val) => info);
            _logger.LogWarning("Rate limiting {info}", info);


            return await SendAsync(request, cancellationToken)
                .ConfigureAwait(false);
        }

    }

    public class RateLimitingInfo
    {
        public int Delay { get; set; }
        public DateTimeOffset LastRequest { get; set; }

    }

}

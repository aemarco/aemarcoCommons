using aemarcoCommons.Extensions.TimeExtensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.Toolbox.NetworkTools;

public class RateLimitingPerHostHandler : DelegatingHandler
{
    private readonly ILogger<RateLimitingPerHostHandler> _logger;
    public RateLimitingPerHostHandler(ILogger<RateLimitingPerHostHandler> logger)
    {
        _logger = logger;
    }


    private static readonly ConcurrentDictionary<string, RateLimitingInfo> RateLimits = new ConcurrentDictionary<string, RateLimitingInfo>();
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri is not null)
        {
            var host = request.RequestUri.Host;
            if (RateLimits.TryGetValue(host, out RateLimitingInfo info))
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

            info ??= new RateLimitingInfo();

            info.Delay = Math.Min(info.Delay + 100, 1000);
            info.LastRequest = DateTimeOffset.Now;

            RateLimits.AddOrUpdate(host, info, (_, _) => info);
            _logger.LogWarning("Rate limiting {info}", info);
        }

        return await base.SendAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }

}

public class RateLimitingInfo
{
    public int Delay { get; set; }
    public DateTimeOffset LastRequest { get; set; }

}
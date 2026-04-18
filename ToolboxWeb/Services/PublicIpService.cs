namespace aemarcoCommons.ToolboxWeb.Services;

public static class PublicIpServiceExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPublicIpService()
        {
            services.AddHttpClient();
            services.TryAddSingleton<IPublicIpService, PublicIpService>();
            return services;
        }
    }
}

public interface IPublicIpService
{
    Task<IPAddress?> Resolve(TimeSpan? maxAge = null);
    Task<IPAddress?> ResolveFresh();
}

public class PublicIpService : IPublicIpService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PublicIpService> _logger;
    public PublicIpService(
        IHttpClientFactory httpClientFactory,
        ILogger<PublicIpService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private IPAddress? _cachedIp;
    private DateTimeOffset _cachedAt = DateTimeOffset.MinValue;
    private DateTimeOffset _retryAfter = DateTimeOffset.MinValue;
    public async Task<IPAddress?> Resolve(TimeSpan? maxAge = null)
    {
        if (_cachedIp is not null &&
            (maxAge is null || DateTimeOffset.UtcNow - _cachedAt <= maxAge))
            return _cachedIp;

        if (DateTimeOffset.UtcNow < _retryAfter)
            return null;

        await _semaphore.WaitAsync();
        try
        {
            if (_cachedIp is not null &&
                (maxAge is null || DateTimeOffset.UtcNow - _cachedAt <= maxAge))
                return _cachedIp;

            if (DateTimeOffset.UtcNow < _retryAfter)
                return null;

            using var client = _httpClientFactory.CreateClient();
            var ip = await ResolveFromSourcesAsync(client);
            if (ip is null)
                _retryAfter = DateTimeOffset.UtcNow.AddMinutes(1);
            else
            {
                _cachedIp = ip;
                _cachedAt = DateTimeOffset.UtcNow;
            }
            return ip;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    public async Task<IPAddress?> ResolveFresh() => await Resolve(TimeSpan.Zero);

    private static readonly (string Url, Func<string, string?> Extract)[] Sources =
    [
        ("https://api.ipify.org",                raw => raw.Trim()),
        ("https://api64.ipify.org/?format=json", ExtractIpFromJson),
        ("https://checkip.amazonaws.com/",       raw => raw.Trim()),
        ("https://ifconfig.me/ip",               raw => raw.Trim()),
    ];
    private async Task<IPAddress?> ResolveFromSourcesAsync(HttpClient httpClient)
    {
        foreach (var (url, extract) in Sources)
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var raw = await httpClient.GetStringAsync(url, cts.Token);
                var ipString = extract(raw);
                if (IPAddress.TryParse(ipString, out var ip))
                {
                    _logger.LogDebug("Resolved own public IP {Ip} via {Url}", ip, url);
                    return ip;
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Public IP source {Url} failed, trying next", url);
            }
        }
        _logger.LogWarning("All public IP sources failed; own-IP hairpin check disabled until next retry");
        return null;
    }

    private static string? ExtractIpFromJson(string raw)
    {
        using var doc = JsonDocument.Parse(raw);
        return doc.RootElement.GetProperty("ip").GetString()?.Trim();
    }
}

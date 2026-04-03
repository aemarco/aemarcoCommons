namespace aemarcoCommons.ToolboxWeb.Authorization;

internal static class PublicIpResolver
{
    private static readonly (string Url, Func<string, string?> Extract)[] Sources =
    [
        ("https://api.ipify.org",                  raw => raw.Trim()),
        ("https://api64.ipify.org/?format=json",    ExtractIpFromJson),
        ("https://checkip.amazonaws.com/",          raw => raw.Trim()),
        ("https://ifconfig.me/ip",                  raw => raw.Trim()),
    ];


    internal static async Task<IPAddress?> ResolveAsync(HttpClient httpClient, ILogger logger)
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
                    logger.LogDebug("Resolved own public IP {Ip} via {Url}", ip, url);
                    return ip;
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Public IP source {Url} failed, trying next", url);
            }
        }
        logger.LogWarning("All public IP sources failed; own-IP hairpin check disabled until next retry");
        return null;
    }

    private static string? ExtractIpFromJson(string raw)
    {
        using var doc = JsonDocument.Parse(raw);
        return doc.RootElement.GetProperty("ip").GetString()?.Trim();
    }

}

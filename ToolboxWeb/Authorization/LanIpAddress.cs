namespace aemarcoCommons.ToolboxWeb.Authorization;

/// <summary>
/// Defines which IP ranges are considered local/trusted for a given LAN policy.
/// Loopback addresses (127.0.0.1, ::1) are always allowed.
/// </summary>
public class LanIpAddressOptions
{
    /// <summary>
    /// CIDR subnets that are considered local, e.g. "192.168.20.0/24" or "10.0.0.0/8".
    /// Defaults to all RFC-1918 private address ranges.
    /// </summary>
    public List<string> LocalSubnets { get; set; } =
    [
        "10.0.0.0/8",
        "172.16.0.0/12",
        "192.168.0.0/16"
    ];

    /// <summary>
    /// When true (default), requests arriving from the server's own public IP are allowed,
    /// covering NAT hairpinning scenarios where a LAN device reaches the server via its
    /// public address. Set to false if the public IP is dynamic or hairpin traffic is unwanted.
    /// </summary>
    public bool AllowOwnPublicIp { get; set; } = true;
}

public static class LanIpAddressExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddLanIpAddressPolicy(string name)
            => services.AddLanIpAddressPolicyCore(null, name, null);

        public IServiceCollection AddLanIpAddressPolicy(string name, Action<LanIpAddressOptions> configure)
            => services.AddLanIpAddressPolicyCore(null, name, (_, opts) => configure(opts));

        public IServiceCollection AddLanIpAddressPolicy(string name, Action<IServiceProvider, LanIpAddressOptions> configure)
            => services.AddLanIpAddressPolicyCore(null, name, configure);

        public IServiceCollection AddLanIpAddressPolicy(string scheme, string name)
            => services.AddLanIpAddressPolicyCore(scheme, name, null);

        public IServiceCollection AddLanIpAddressPolicy(string scheme, string name, Action<LanIpAddressOptions> configure)
            => services.AddLanIpAddressPolicyCore(scheme, name, (_, opts) => configure(opts));

        public IServiceCollection AddLanIpAddressPolicy(string scheme, string name, Action<IServiceProvider, LanIpAddressOptions> configure)
            => services.AddLanIpAddressPolicyCore(scheme, name, configure);

        private IServiceCollection AddLanIpAddressPolicyCore(string? scheme, string name, Action<IServiceProvider, LanIpAddressOptions>? configure)
        {
            services
                .AddAuthorizationBuilder()
                .AddPolicy(name, policy =>
                {
                    if (scheme is not null)
                        policy.AddAuthenticationSchemes(scheme);
                    policy.AddRequirements(new LanIpAddressRequirement(name));
                });
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            var optBuilder = services.AddOptions<LanIpAddressOptions>(name);
            if (configure is not null)
                optBuilder.Configure<IServiceProvider>((opts, sp) => configure(sp, opts));
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IAuthorizationHandler, LanIpAddressHandler>());
            return services;
        }
    }
}

public class LanIpAddressRequirement(string policyName) : IAuthorizationRequirement
{
    public string PolicyName { get; } = policyName;
}

public class LanIpAddressHandler : AuthorizationHandler<LanIpAddressRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptionsMonitor<LanIpAddressOptions> _optionsMonitor;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LanIpAddressHandler> _logger;
    public LanIpAddressHandler(
        IHttpContextAccessor httpContextAccessor,
        IOptionsMonitor<LanIpAddressOptions> optionsMonitor,
        IHttpClientFactory httpClientFactory,
        ILogger<LanIpAddressHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _optionsMonitor = optionsMonitor;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }



    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, LanIpAddressRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            _logger.LogWarning("Could not get HttpContext");
            context.Fail();
            return;
        }

        var remoteIp = httpContext.Connection.RemoteIpAddress;
        if (remoteIp is null)
        {
            _logger.LogWarning("Could not get remote IP address");
            context.Fail();
            return;
        }

        // Normalize IPv4-mapped IPv6 addresses (::ffff:x.x.x.x → x.x.x.x)
        if (remoteIp.IsIPv4MappedToIPv6)
            remoteIp = remoteIp.MapToIPv4();

        var subnets = await GetSubnetsAsync(requirement.PolicyName);

        if (!IPAddress.IsLoopback(remoteIp) && !IsInSubnet(remoteIp, subnets))
        {
            _logger.LogWarning("Access denied to {RemoteIp}", remoteIp);
            context.Fail();
            return;
        }

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("Access granted to {RemoteIp}", remoteIp);
        context.Succeed(requirement);
    }

    private readonly ConcurrentDictionary<string, List<(IPAddress Network, int PrefixLength)>> _subnetCache = [];
    private async Task<List<(IPAddress Network, int PrefixLength)>> GetSubnetsAsync(string policyName)
    {
        if (_subnetCache.TryGetValue(policyName, out var cached))
            return cached;

        var options = _optionsMonitor.Get(policyName);
        var subnets = ParseSubnets(options.LocalSubnets);

        if (options.AllowOwnPublicIp)
        {
            var ownIp = await GetOwnPublicIpAsync();
            if (ownIp is null)
                return subnets; // resolution failed — don't cache, retry next request

            if (ownIp.IsIPv4MappedToIPv6)
                ownIp = ownIp.MapToIPv4();

            var prefixLength = ownIp.GetAddressBytes().Length * 8;
            subnets.Add((ownIp, prefixLength));
        }

        return _subnetCache.GetOrAdd(policyName, subnets);
    }
    internal static List<(IPAddress Network, int PrefixLength)> ParseSubnets(List<string> subnets)
    {
        var result = new List<(IPAddress Network, int PrefixLength)>();
        foreach (var subnet in subnets)
        {
            var slash = subnet.IndexOf('/');
            if (slash > 0 &&
                IPAddress.TryParse(subnet.AsSpan(0, slash), out var network) &&
                int.TryParse(subnet.AsSpan(slash + 1), out var prefix))
            {
                var maxPrefix = network.GetAddressBytes().Length * 8;
                if (prefix >= 0 && prefix <= maxPrefix)
                    result.Add((network, prefix));
            }
        }
        return result;
    }
    internal static bool IsInSubnet(IPAddress address, List<(IPAddress Network, int PrefixLength)> subnets)
    {
        foreach (var (network, prefixLength) in subnets)
        {
            if (IsInSubnet(address, network, prefixLength))
                return true;
        }
        return false;
    }
    internal static bool IsInSubnet(IPAddress address, IPAddress network, int prefixLength)
    {
        var addressBytes = address.GetAddressBytes();
        var networkBytes = network.GetAddressBytes();

        if (addressBytes.Length != networkBytes.Length)
            return false;

        var fullBytes = prefixLength / 8;
        var remainingBits = prefixLength % 8;

        for (var i = 0; i < fullBytes; i++)
        {
            if (addressBytes[i] != networkBytes[i])
                return false;
        }

        if (remainingBits > 0)
        {
            var mask = (byte)(0xFF << (8 - remainingBits));
            if ((addressBytes[fullBytes] & mask) != (networkBytes[fullBytes] & mask))
                return false;
        }

        return true;
    }




    private readonly SemaphoreSlim _publicIpSemaphore = new(1, 1);
    private IPAddress? _ownPublicIp;
    private DateTimeOffset _retryAfter = DateTimeOffset.MinValue;
    private async Task<IPAddress?> GetOwnPublicIpAsync()
    {
        if (_ownPublicIp is not null)
            return _ownPublicIp;

        if (DateTimeOffset.UtcNow < _retryAfter)
            return null;

        await _publicIpSemaphore.WaitAsync();
        try
        {
            // double-check after acquiring
            if (_ownPublicIp is not null)
                return _ownPublicIp;

            if (DateTimeOffset.UtcNow < _retryAfter)
                return null;

            using var client = _httpClientFactory.CreateClient();
            var ip = await PublicIpResolver.ResolveAsync(client, _logger);
            if (ip is null)
                _retryAfter = DateTimeOffset.UtcNow.AddMinutes(1);
            else
                _ownPublicIp = ip;
            return ip;
        }
        finally
        {
            _publicIpSemaphore.Release();
        }
    }

}

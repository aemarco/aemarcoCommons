using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace aemarcoCommons.WebTools.Authorization;

public static class LanIpAddressExtensions
{
    public static IServiceCollection AddLanIpAddressPolicy(this IServiceCollection services, params string[] names)
    {
        services.AddAuthorization(config =>
        {
            foreach (var name in names)
            {
                config.AddPolicy(name, builder =>
                {
                    builder.AddRequirements(new LanIpAddressRequirement());
                });
            }
        });
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddSingleton<IAuthorizationHandler, LanIpAddressHandler>();
        return services;
    }
}
public class LanIpAddressRequirement : IAuthorizationRequirement { }
public class LanIpAddressHandler : AuthorizationHandler<LanIpAddressRequirement>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<LanIpAddressHandler> _logger;

    public LanIpAddressHandler(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        ILogger<LanIpAddressHandler> logger)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }


    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, LanIpAddressRequirement requirement)
    {
        var wanAddress = await GetWanIp()
            .ConfigureAwait(false);
        HttpContext? httpContext = _httpContextAccessor.HttpContext;
        if (string.IsNullOrWhiteSpace(wanAddress) || // if unknown, assume not local
            httpContext is null) //must have a http context
        {
            _logger.LogWarning(httpContext is null ? "Could not get httpContext" : "Could not get remote ip");
            context.Fail();
            return;
        }


        var remoteAddress = httpContext.Connection.RemoteIpAddress?.ToString();
        var localAddress = httpContext.Connection.LocalIpAddress?.ToString();
        if (string.IsNullOrWhiteSpace(remoteAddress) ||
            string.IsNullOrWhiteSpace(localAddress))
        {
            _logger.LogWarning(string.IsNullOrWhiteSpace(remoteAddress) ? "Could not get remoteAddress" : "Could not get localAddress");
            context.Fail();
            return;
        }

        var whitelist = new[]
        {
            "127.0.0.1", // check if localhost
            "::1", // check if localhost
            localAddress, // compare with local address
            wanAddress // check if from own ip 
        };
        if (!whitelist.Contains(remoteAddress))
        {
            _logger.LogWarning("Access denied to {remoteAddress}", remoteAddress);
            context.Fail();
            return;
        }

        _logger.LogDebug("Access granted to {remoteAddress}", remoteAddress);
        context.Succeed(requirement);
    }


    //TODO better way to get a wan ip ?
    private string? _wanIp;
    private async Task<string> GetWanIp()
    {
        if (!string.IsNullOrWhiteSpace(_wanIp))
            return _wanIp;


        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://api.ipify.org/")
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var ipText = await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        if (IPAddress.TryParse(ipText, out IPAddress? test))
        {
            _wanIp = test.ToString();
            return _wanIp;
        }

        throw new Exception("Could not determine WAN Ip address");
    }

}
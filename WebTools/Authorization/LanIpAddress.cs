using aemarcoCommons.Toolbox.GeoTools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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
public class LanIpAddressRequirement : IAuthorizationRequirement;
public class LanIpAddressHandler : AuthorizationHandler<LanIpAddressRequirement>
{
    private readonly GeoService _geoService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<LanIpAddressHandler> _logger;

    public LanIpAddressHandler(
        GeoService geoService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<LanIpAddressHandler> logger)
    {
        _geoService = geoService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }


    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, LanIpAddressRequirement requirement)
    {
        var wanAddress = await _geoService.GetIpInfo(
            maximumInfoAge: TimeSpan.FromMinutes(15));
        if (string.IsNullOrWhiteSpace(wanAddress))
        {
            _logger.LogWarning("Could not get wan ip address");
            context.Fail();
            return;
        }

        HttpContext? httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null) //must have a http context
        {
            _logger.LogWarning("Could not get httpContext");
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
}
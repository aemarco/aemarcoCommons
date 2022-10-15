using Microsoft.Extensions.DependencyInjection;

namespace aemarcoCommons.WebTools.Authorization;

public static class AllowedClientsExtensions
{
    public static IServiceCollection AddAllowedClientsPolicy(
        this IServiceCollection services,
        string policyName,
        params string[] clientIds)
    {
        services.AddAuthorization(config =>
        {
            config.AddPolicy(policyName, builder =>
            {
                builder
                    .RequireClaim("client_id", clientIds);
            });
        });
        return services;
    }
}
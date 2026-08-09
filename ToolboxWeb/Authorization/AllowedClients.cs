namespace aemarcoCommons.ToolboxWeb.Authorization;

public static class AllowedClientsExtensions
{
    /// <summary>
    /// Adds an authorization policy that requires the caller's 'client_id' claim to match one of the given client IDs.
    /// </summary>
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
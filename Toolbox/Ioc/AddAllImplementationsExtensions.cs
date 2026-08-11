using System.Reflection;

namespace aemarcoCommons.Toolbox.Ioc;

public static class AddAllImplementationsExtensions
{
    public static IServiceCollection AddTransientImplementations<TInterface>(
        this IServiceCollection services,
        params Assembly[] assemblies) =>
        services.AddImplementations<TInterface>(ServiceLifetime.Transient, assemblies);

    public static IServiceCollection AddScopedImplementations<TInterface>(
        this IServiceCollection services,
        params Assembly[] assemblies) =>
        services.AddImplementations<TInterface>(ServiceLifetime.Scoped, assemblies);

    public static IServiceCollection AddSingletonImplementations<TInterface>(
        this IServiceCollection services,
        params Assembly[] assemblies) =>
        services.AddImplementations<TInterface>(ServiceLifetime.Singleton, assemblies);

    private static IServiceCollection AddImplementations<TInterface>(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        Assembly[] assemblies)
    {
        var types = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                typeof(TInterface).IsAssignableFrom(t) &&
                t is { IsAbstract: false, IsInterface: false });
        foreach (var type in types)
            services.Add(ServiceDescriptor.Describe(typeof(TInterface), type, lifetime));
        return services;
    }
}
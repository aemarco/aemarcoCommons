using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace aemarcoCommons.Toolbox.Ioc;

public interface IAssMarker
{
    void Config(IConfigurationBuilder configBuilder, IHostEnvironment environment, string? prefixToStrip = null)
    {
        var assFile = Path
            .GetFileNameWithoutExtension(GetType().Assembly.Location)
            .ToLower();
        if (string.IsNullOrWhiteSpace(assFile))
            return;

        if (prefixToStrip is { Length: > 0 } prefix &&
            assFile.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            assFile = assFile[prefix.Length..];

        configBuilder
            .AddJsonFile($"appsettings.{assFile}.json", true, true)
            .AddJsonFile($"appsettings.{assFile}.{environment.EnvironmentName}.json", true, true);
    }
    void Setup(IServiceCollection services, IConfiguration config, IHostEnvironment env);
}

public static class AssMarkerExtensions
{
    public static IConfigurationBuilder ConfigAssMarkers(
        this IConfigurationBuilder configBuilder,
        Type[] assMarkers,
        IHostEnvironment environment,
        string? prefixToStrip = null)
    {
        assMarkers.PerformOnAssMarkers(x =>
            x.Config(configBuilder, environment, prefixToStrip));

        return configBuilder;
    }

    public static IServiceCollection SetupAssMarkers(
        this IServiceCollection services,
        Type[] assMarkers,
        IConfiguration config,
        IHostEnvironment env)
    {
        assMarkers.PerformOnAssMarkers(x =>
            x.Setup(services, config, env));

        return services;
    }

    private static void PerformOnAssMarkers(
        this IEnumerable<Type> assMarkers,
        Action<IAssMarker> action)
    {
        foreach (var marker in assMarkers)
        {
            if (!typeof(IAssMarker).IsAssignableFrom(marker) ||
                !marker.IsClass ||
                marker.IsAbstract)
                continue;
            //we only deal with assembly markers we intend to

            if (Activator.CreateInstance(marker) as IAssMarker
                is not { } instance)
                throw new UnreachableException($"Could not activate Assembly {marker.Name}");
            //we need an instance to call

            action(instance);
        }
    }
}

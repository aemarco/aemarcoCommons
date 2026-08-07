using System.Reflection;

namespace aemarcoCommons.Toolbox.Ioc;

public static class LuckyPennyLicenseWarningExtensions
{
    /// <summary>Must be called after AddMediatR, which resets this flag to false.</summary>
    public static IServiceCollection SuppressMediatRLicenseWarning(this IServiceCollection services)
    {
        AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "MediatR")?
            .GetType("Microsoft.Extensions.DependencyInjection.MediatRServiceCollectionExtensions")?
            .GetProperty("LicenseChecked", BindingFlags.Static | BindingFlags.NonPublic)?
            .SetValue(null, true);
        return services;
    }
}

#pragma warning disable IDE0130
namespace aemarcoCommons.ToolboxMediatR;


public static class LuckyPennyLicenseWarningExtensions
{
    /// <summary>Must be called after AddMediatR, which resets this flag to false.</summary>
    public static IServiceCollection SuppressMediatRLicenseWarning(this IServiceCollection services)
    {
        typeof(MediatRServiceCollectionExtensions)
            .GetProperty("LicenseChecked", BindingFlags.Static | BindingFlags.NonPublic)?
            .SetValue(null, true);
        return services;
    }
}

#pragma warning disable IDE0130
namespace aemarcoCommons.ToolboxAppOptions.Transformations;

public static class ConfigurationExtensions
{
    [Obsolete("Use GetResolved instead.")]
    public static string? GetResolvedText(this IConfiguration config, string path) =>
        config.GetResolved<string?>(path);
}
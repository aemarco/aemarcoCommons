using System.Collections.Generic;
using System.Linq;
// ReSharper disable MemberCanBePrivate.Global

namespace aemarcoCommons.ToolboxAppOptions;

public record ConfigurationOptions
{
    public List<StringTransformerBase> StringTransformations { get; } = [];
    public List<Assembly> Assemblies { get; } = [];
    public bool EnableValidationOnStartup { get; internal set; } = true;

    internal IReadOnlyList<Type> ConfigurationTypes { get; set; } = [];
    internal IReadOnlyList<Assembly> ConfigurationAssemblies { get; set; } = [];
}

public class ConfigurationOptionsBuilder
{

    private readonly ConfigurationOptions _result = new();

    public ConfigurationOptionsBuilder AddStringTransformation(StringTransformerBase stringTransformer)
    {
        _result.StringTransformations.Add(stringTransformer);
        return this;
    }
    public ConfigurationOptionsBuilder AddAssemblyMarker(params Type[] types)
    {
        AddAssemblies([.. types.Select(x => x.Assembly)]);
        return this;
    }
    public ConfigurationOptionsBuilder AddAssemblies(params Assembly[] assemblies)
    {
        _result.Assemblies.AddRangeDistinct(
            assemblies,
            x => x.FullName);
        return this;
    }
    /// <summary>
    /// Enables, that during host startup all the config objects will be validated
    /// </summary>
    /// <param name="enableValidationOnStartup"></param>
    /// <returns></returns>
    public ConfigurationOptionsBuilder EnableValidationOnStartup(bool enableValidationOnStartup = true)
    {
        _result.EnableValidationOnStartup = enableValidationOnStartup;
        return this;
    }


    internal ConfigurationOptions Build()
    {
        if (Assembly.GetEntryAssembly() is { } entry)
            _result.Assemblies.AddDistinct(entry, x => x.FullName);

        _result.ConfigurationTypes = _result.Assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x =>
                x.IsAssignableTo(typeof(ISettingsBase)) &&
                x is { IsAbstract: false, IsInterface: false })
            .ToList();

        _result.ConfigurationAssemblies = _result.ConfigurationTypes
            .Select(x => x.Assembly)
            .Distinct()
            .ToList();

        return _result;
    }

}
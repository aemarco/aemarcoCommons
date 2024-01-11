using System.Collections.Generic;
using System.Linq;

namespace aemarcoCommons.ToolboxAppOptions;

public record ConfigurationOptions
{
    public readonly List<StringTransformerBase> StringTransformations = [];
    public bool EnableValidationOnStartup { get; internal set; } = true;
    public readonly List<Assembly> Assemblies = [];


    internal readonly List<Type> ConfigurationTypes = [];
    internal readonly List<Assembly> ConfigurationAssemblies = [];
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
        foreach (var type in types)
        {
            var assembly = Assembly.GetAssembly(type)
                ?? throw new Exception($"Assembly can not be loaded: {type}");
            _result.Assemblies.AddDistinct(assembly, x => x.FullName);
        }
        return this;
    }
    public ConfigurationOptionsBuilder AddAssemblies(params Assembly[] assemblies)
    {
        _result.Assemblies.AddRangeDistinct(assemblies, x => x.FullName);
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


    internal ConfigurationOptionsBuilder AddConfigurationTypes()
    {
        var entryAsm = Assembly.GetEntryAssembly()
            ?? throw new Exception("Entry assembly can not be loaded");
        _result.Assemblies.AddDistinct(entryAsm, x => x.FullName);

        var settingsTypes = _result.Assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsSubclassOf(typeof(SettingsBase)));
        foreach (var type in settingsTypes)
        {
            _result.ConfigurationTypes.AddDistinct(type, x => x.FullName);
            _result.ConfigurationAssemblies.AddDistinct(type.Assembly, x => x.FullName);
        }
        return this;
    }

    public ConfigurationOptions Build()
    {
        return _result;
    }

}
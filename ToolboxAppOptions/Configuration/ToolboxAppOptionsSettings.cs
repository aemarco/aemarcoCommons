// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ToolboxAppOptions;

public record ToolboxAppOptionsSettings
{
    public List<StringTransformerBase> StringTransformations { get; } = [];
    public List<Assembly> Assemblies { get; } = [];
    public bool EnableValidationOnStartup { get; internal set; } = true;


    /// <summary>
    /// resolved types implementing ISettingsBase
    /// </summary>
    internal IReadOnlyList<Type> ConfigurationTypes { get; set; } = [];
    /// <summary>
    /// assemblies containing ISettingsBase types
    /// </summary>
    internal IReadOnlyList<Assembly> ConfigurationAssemblies { get; set; } = [];
}

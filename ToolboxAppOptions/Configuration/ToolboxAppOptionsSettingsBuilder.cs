// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ToolboxAppOptions;

public class ToolboxAppOptionsSettingsBuilder
{

    private readonly ToolboxAppOptionsSettings _result = new();

    public ToolboxAppOptionsSettingsBuilder AddStringTransformation(StringTransformerBase stringTransformer)
    {
        _result.StringTransformations.Add(stringTransformer);
        return this;
    }
    public ToolboxAppOptionsSettingsBuilder AddAssemblyMarker(params Type[] types)
    {
        AddAssemblies([.. types.Select(x => x.Assembly)]);
        return this;
    }
    public ToolboxAppOptionsSettingsBuilder AddAssemblies(params Assembly[] assemblies)
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
    public ToolboxAppOptionsSettingsBuilder EnableValidationOnStartup(bool enableValidationOnStartup = true)
    {
        _result.EnableValidationOnStartup = enableValidationOnStartup;
        return this;
    }


    internal ToolboxAppOptionsSettings Build()
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

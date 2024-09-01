namespace aemarcoCommons.ToolboxAppOptions;

/// <summary>
/// Custom Path in IConfiguration
/// ex. empty string means root,
/// ex. "Abc:Def"
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SettingsPathAttribute(string path) : Attribute
{
    public readonly string Path = path;
}


/// <summary>
/// Skips registration of IOptions, itself etc...
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SkipRegistrationAttribute : Attribute;


public interface ISettingsBase;
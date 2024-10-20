namespace aemarcoCommons.ToolboxAppOptions;

/// <summary>
/// This interface is to mark setting classes, so they get recognized as such
/// </summary>
public interface ISettingsBase;

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
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

public abstract class SettingsBase { }
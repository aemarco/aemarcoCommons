using System;

namespace aemarcoCommons.ToolboxAppOptions
{

    /// <summary>
    /// Custom Path in IConfiguration
    /// ex. empty string means root,
    /// ex. "Abc:Def"
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SettingsPathAttribute : Attribute
    {
        public SettingsPathAttribute(string path)
        {
            Path = path;
        }
        public string Path { get; }
    }

    public abstract class SettingsBase
    {

    }

}
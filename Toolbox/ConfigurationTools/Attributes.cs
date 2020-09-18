using System;

namespace aemarcoCommons.Toolbox.ConfigurationTools
{
    /// <summary>
    /// Custom Path in IConfiguration
    /// ex. empty string means root,
    /// ex. "Abc:Def"
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SettingPathAttribute : Attribute
    {
        public string Path { get; }
        public SettingPathAttribute(string path)
        {
            Path = path;
        }
    }


    /// <summary>
    /// Use this to En-/Decrypt string properties while Saving and Loading
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ProtectedAttribute : Attribute { }
}
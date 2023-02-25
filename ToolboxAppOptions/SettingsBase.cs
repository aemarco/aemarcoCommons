using aemarcoCommons.ToolboxAppOptions.Transformations;
using System;
using System.Collections.Generic;

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

    public class ConfigurationOptions
    {
        public List<StringTransformerBase> StringTransformations { get; } = new List<StringTransformerBase>();
    }
    public class ConfigurationOptionsBuilder
    {
        private readonly ConfigurationOptions _result = new ConfigurationOptions();
        public ConfigurationOptionsBuilder AddStringTransformation(StringTransformerBase stringTransformer)
        {
            _result.StringTransformations.Add(stringTransformer);
            return this;
        }
        public ConfigurationOptions Build()
        {
            return _result;
        }
    }
}
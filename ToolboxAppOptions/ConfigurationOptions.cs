using aemarcoCommons.ToolboxAppOptions.Transformations;
using System;
using System.Collections.Generic;

namespace aemarcoCommons.ToolboxAppOptions
{
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

        public ConfigurationOptionsBuilder AddAssemblyMarker(Type type)
        {
            _ = type;
            return this;
        }

        public ConfigurationOptions Build()
        {
            return _result;
        }

    }
}
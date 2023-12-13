using aemarcoCommons.Extensions;
using aemarcoCommons.ToolboxAppOptions.Transformations;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace aemarcoCommons.ToolboxAppOptions
{
    public class ConfigurationOptions
    {
        public List<StringTransformerBase> StringTransformations { get; } = new List<StringTransformerBase>();
        public bool EnableValidationOnStartup { get; internal set; } = true;
        public List<Assembly> Assemblies { get; } = new List<Assembly>();


        internal List<Type> ConfigurationTypes { get; } = new List<Type>();
        internal List<Assembly> ConfigurationAssemblies { get; } = new List<Assembly>();
    }

    public class ConfigurationOptionsBuilder
    {

        private readonly ConfigurationOptions _result = new ConfigurationOptions();
        public ConfigurationOptionsBuilder AddStringTransformation(StringTransformerBase stringTransformer)
        {
            _result.StringTransformations.Add(stringTransformer);
            return this;
        }

        public ConfigurationOptionsBuilder AddAssemblyMarker(params Type[] type)
        {
            _ = type;
            return this;
        }
        public ConfigurationOptionsBuilder AddAssemblies(params Assembly[] assemblies)
        {
            //TODO AddRangeDistinct with selector
            foreach (var assembly in assemblies)
                _result.Assemblies.AddDistinct(assembly, x => x.FullName);
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


        internal ConfigurationOptionsBuilder AddConfigurationTypes(params Type[] types)
        {
            foreach (var type in types)
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
}
using aemarcoCommons.ToolboxAppOptions.Transformations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace aemarcoCommons.ToolboxAppOptions.Services
{
    public class AppOptionFactory<TOptions> :
        IConfigureOptions<TOptions>,
        IPostConfigureOptions<TOptions>,
        IValidateOptions<TOptions>
        where TOptions : class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfigurationRoot _config;
        public AppOptionFactory(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _config = serviceProvider.GetRequiredService<IConfigurationRoot>();
        }
        public void Configure(TOptions options)
        {
            var type = typeof(TOptions);
            var path = Attribute.GetCustomAttribute(type, typeof(SettingsPathAttribute)) is SettingsPathAttribute pathAttribute
                ? pathAttribute.Path
                : type.Name;
            if (string.IsNullOrWhiteSpace(path))
                _config.Bind(options);
            else
                _config.GetSection(path).Bind(options);
        }

        public void PostConfigure(string name, TOptions options)
        {
            var toolOptions = _serviceProvider.GetRequiredService<ConfigurationOptions>();
            //ApplyReadTransformations
            foreach (var transformation in toolOptions.StringTransformations)
            {
                StringTransformerBase.TransformObject(options, _config, transformation.PerformReadTransformation);
            }
        }


        public ValidateOptionsResult Validate(string name, TOptions options)
        {

            return ValidateOptionsResult.Success;
        }
    }
}
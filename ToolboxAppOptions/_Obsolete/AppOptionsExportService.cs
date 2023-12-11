using aemarcoCommons.ToolboxAppOptions.Transformations;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ToolboxAppOptions.Services
{
    [Obsolete("Use AppOptionsTransformationService instead")]
    public class AppOptionsExportService
    {

        private readonly ConfigurationOptions _options;
        private readonly IConfigurationRoot _config;

        public AppOptionsExportService(
            ConfigurationOptions options,
            IConfigurationRoot config)
        {
            _options = options;
            _config = config;
        }

        public void PerformWriteTransformation(SettingsBase options)
        {
            var transformations = _options.StringTransformations.ToList();
            transformations.Reverse();

            //ApplyReadTransformations
            foreach (var transformation in transformations)
            {
                StringTransformerBase.TransformObject(options, _config, transformation.PerformWriteTransformation);
            }
        }

    }
}

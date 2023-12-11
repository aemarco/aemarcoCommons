using aemarcoCommons.ToolboxAppOptions.Transformations;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace aemarcoCommons.ToolboxAppOptions.Services
{
    public class AppOptionsTransformationService
    {

        private readonly ConfigurationOptions _options;
        private readonly IConfigurationRoot _config;
        public AppOptionsTransformationService(
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

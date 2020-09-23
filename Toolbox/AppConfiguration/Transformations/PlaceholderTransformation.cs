using System.Reflection;
using aemarcoCommons.Extensions.TextExtensions;
using Microsoft.Extensions.Configuration;

namespace aemarcoCommons.Toolbox.AppConfiguration.Transformations
{
    public class PlaceholderTransformation: StringTransformerBase
    {
        public override string PerformReadTransformation(string currentValue, PropertyInfo propertyInfo, IConfigurationRoot configRoot)
        {
            var resolved = currentValue.ResolvePlaceholders(configRoot);
            return resolved;
        }

        public override string PerformWriteTransformation(string currentValue, PropertyInfo propertyInfo, IConfigurationRoot configRoot) => currentValue;
    }
}

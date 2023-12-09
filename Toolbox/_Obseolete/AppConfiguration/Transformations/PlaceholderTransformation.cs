using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace aemarcoCommons.Toolbox.AppConfiguration.Transformations
{
    /// <summary>
    /// This Transformer replaces placeholders in strings with values defined for the key in the placeholder.
    /// The entire IConfiguration will be searched for defined key.
    /// To define a placeholder, just wrap the key in triple curly braces like {{{myKey}}}.
    /// Ensure to define a value for the defined key somewhere in the config.
    /// To use this, add it in the StringTransformations List, inside the AddConfigurationUtils options.
    /// ATTENTION: Better not use this for parts of the config which you save, because on write, resolved values will be written. 
    /// </summary>
    public class PlaceholderTransformation : StringTransformerBase
    {
        public override string PerformReadTransformation(string currentValue, PropertyInfo propertyInfo, IConfigurationRoot configRoot)
        {
            var resolved = configRoot.ResolvePlaceholders(currentValue);
            return resolved;
        }

        public override string PerformWriteTransformation(string currentValue, PropertyInfo propertyInfo, IConfigurationRoot configRoot) =>
            currentValue;
    }
}

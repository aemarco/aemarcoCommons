using System.Text.RegularExpressions;

namespace aemarcoCommons.ToolboxAppOptions.Transformations;

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

public static class ConfigurationExtensions
{
    public static string GetResolvedText(this IConfiguration config, string path)
    {
        var unresolved = config.GetValue<string>(path);
        var result = ((IConfigurationRoot)config)
            .ResolvePlaceholders(unresolved
                ?? throw new NullReferenceException(path));
        return result;
    }
}

internal static class PlaceholderTransformationExtensions
{
    /// <summary>
    /// Searches for {{{placeholder}}}, and replaces that with resolved value
    /// </summary>
    /// <param name="root">configuration root to search in</param>
    /// <param name="currentValue">value in which placeholders should be resolved</param>
    /// <returns>given value with it´s placeholder´s resolved</returns>
    internal static string ResolvePlaceholders(this IConfigurationRoot root, string currentValue)
    {
        if (string.IsNullOrWhiteSpace(currentValue)) return currentValue;

        const string pattern = @"{{3}([^|\n]+?)}{3}";
        while (Regex.IsMatch(currentValue, pattern))
        {
            foreach (Match match in Regex.Matches(currentValue, pattern))
            {
                var search = match.Groups[1].Value;
                var newValue = root.SearchValueInTree(search);
                if (newValue == null) throw new Exception($"Cant resolve placeholder {search}");

                currentValue = currentValue.Replace(match.Value, newValue);
            }
        }

        return currentValue;
    }


    /// <summary>
    /// Searches given config section for given key, and returns it´s value, which can´t be a placeholder to itself
    /// </summary>
    /// <param name="section">section to search</param>
    /// <param name="key">key to search</param>
    /// <returns>the value for given key</returns>
    private static string SearchValueInTree(this IConfiguration section, string key)
    {
        //search in section
        var result = section[key];
        if (result == $"{{{{{{{key}}}}}}}") result = null;

        //search in sub sections
        if (result != null) return result;
        foreach (var child in section.GetChildren())
        {
            result = child.SearchValueInTree(key);
            if (result == $"{{{{{{{key}}}}}}}") result = null;
            if (result != null) break;
        }
        return result;
    }

}

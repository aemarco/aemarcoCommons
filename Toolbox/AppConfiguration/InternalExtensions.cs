using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace aemarcoCommons.Toolbox.AppConfiguration
{
    internal static class InternalExtensions
    {

        /// <summary>
        /// Resolves the section path in IConfigurationRoot
        /// </summary>
        /// <param name="type">type for which to resole</param>
        /// <returns>defined path</returns>
        internal static string GetSectionPath(this Type type)
        {
            //use path defined in attribute if specified
            if (Attribute.GetCustomAttribute(type, typeof(SettingPathAttribute)) is SettingPathAttribute pathAttribute)
                return pathAttribute.Path;
            //use typename if nothing is specified
            else
                return type.Name;
        }

        /// <summary>
        /// Gather absolute file Path which the type get saved to
        /// </summary>
        /// <param name="type">type for which the file Path should be returned</param>
        /// <param name="options">options</param>
        /// <returns>absolute file path to the saved configuration file</returns>
        internal static string GetSavePathForSetting(this Type type, ConfigurationOptions options)
        {
            var getFileName = options.GetFileNameForSettingsClass
                              ?? throw new ApplicationException("Saving disabled. To enable don´t set GetFileNameForSettingsClass to null");
            var saveDirectory = options.SettingsSaveDirectory
                                ?? throw new ApplicationException("Saving disabled. To enable don´t set SettingsSaveDirectory to null");
            if (!Directory.Exists(saveDirectory)) Directory.CreateDirectory(saveDirectory);

            return Path.Combine(saveDirectory, getFileName(type));
        }



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
        internal static string SearchValueInTree(this IConfiguration section, string key)
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
}
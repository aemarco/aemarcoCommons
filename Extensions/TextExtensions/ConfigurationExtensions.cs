using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace Extensions.TextExtensions
{
    public static class ConfigurationExtensions
    {
        private const string Pattern = @"{{3}([^|\n]+?)}{3}";

        public static string ResolvePlaceholders(this string currentValue,  IConfiguration root)
        {
            if (string.IsNullOrWhiteSpace(currentValue)) return currentValue;

            while (Regex.IsMatch(currentValue, Pattern))
            {
                foreach (Match match in Regex.Matches(currentValue, Pattern))
                {
                    var search = match.Groups[1].Value;
                    var newValue = root.SearchValue(search, false, root);
                    if (newValue == null) throw new Exception($"Cant resolve placeholder {search}");

                    currentValue = currentValue.Replace(match.Value, newValue);
                }
            }

            return currentValue;
        }


        public static string SearchValue(this IConfiguration root, string key, bool resolvePlaceholders = true)
        {
            return root.SearchValue(key, resolvePlaceholders, root);
        }


        private static string SearchValue(this IConfiguration section, string key, bool resolvePlaceholders, IConfiguration root)
        {
            //search in section
            var result = section[key];
            if (result == $"{{{{{{{key}}}}}}}") result = null;
            
            //search in sub sections
            if (result == null)
            {
                foreach (var child in section.GetChildren())
                {
                    result = child.SearchValue(key, resolvePlaceholders, root);
                    if (result == $"{{{{{{{key}}}}}}}") result = null;
                    if (result != null) break;
                }
            }

            if (resolvePlaceholders)
            {
                result = result.ResolvePlaceholders(root);
            }

            return result;
        }

        



    }
}

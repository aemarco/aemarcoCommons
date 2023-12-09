using Microsoft.Extensions.Configuration;

namespace aemarcoCommons.Toolbox.AppConfiguration
{
    public static class PublicExtensions
    {
        public static string SearchValue(this IConfiguration root, string key)
        {
            var result = root.SearchValueInTree(key);
            return result;
        }

        public static string SearchResolvedValue(this IConfigurationRoot root, string key)
        {
            var result = root.ResolvePlaceholders(root.SearchValueInTree(key));
            return result;
        }
    }
}

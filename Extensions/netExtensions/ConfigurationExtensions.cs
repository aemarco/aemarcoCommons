using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Extensions.netExtensions
{
    public static class ConfigurationExtensions
    {
        public static string SearchValue(this IConfiguration configuration, string key)
        {
            var result = configuration[key];
            if (result != null) return result;

            foreach (var child in configuration.GetChildren())
            {
                result = child.SearchValue(key);
                if (result != null) break;
            }
            return result;
        }

    }
}

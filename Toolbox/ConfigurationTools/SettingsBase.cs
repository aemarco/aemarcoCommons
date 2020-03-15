using System.Linq;
using System.Text.RegularExpressions;
using Extensions.netExtensions;
using Microsoft.Extensions.Configuration;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Toolbox.ConfigurationTools
{
    
    public abstract class SettingsBase
    {
        protected SettingsBase(IConfiguration root)
            :this(root, root)
        { }

        protected SettingsBase(IConfiguration root, IConfiguration section)
        {
            section.GetSection(this.GetType().Name).Bind(this);
            HandleStringPlaceholders(root);
        }


        private void HandleStringPlaceholders(IConfiguration root)
        {
            foreach (var propInfo in this.GetType().GetProperties()
                .Where(x => x.PropertyType == typeof(string))
                .Where(x => x.CanRead && x.CanWrite))
            {
                var currentValue = (string)propInfo.GetValue(this);
                if (currentValue == null) continue;

                foreach (Match match in Regex.Matches(currentValue, @"{{3}([^|\n]+?)}{3}"))
                {
                    var search = match.Groups[1].Value;
                    var newValue = root.SearchValue(search);
                    if (newValue == null) continue;

                    currentValue = currentValue.Replace(match.Value, newValue);
                    propInfo.SetValue(this, currentValue);
                }
            }
        }


        







    }
}

using System.Linq;
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
            // ReSharper disable once VirtualMemberCallInConstructor
            HandleStringPlaceholders(root);
        }


        protected virtual void HandleStringPlaceholders(IConfiguration root)
        { 
            foreach (var propInfo in this.GetType().GetProperties()
                .Where(x => x.PropertyType == typeof(string))
                .Where(x => x.CanRead && x.CanWrite))
            {
                var currentValue = (string)propInfo.GetValue(this);
                var resolved = currentValue.ResolvePlaceholders(root);
                if (currentValue != resolved)
                {
                    propInfo.SetValue(this, resolved);
                }
            }
        }
    }
}

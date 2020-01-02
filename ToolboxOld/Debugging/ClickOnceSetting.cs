using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolboxOld.Debugging
{
    public static class ClickOnceSetting
    {
        public static void EnsureMaintainUserSettings(this ApplicationSettingsBase settings, string keyName)
        {
            var keyValue = settings[keyName];
            var defaultValue = settings.Properties[keyName].DefaultValue;
            if (keyValue == defaultValue)
            {
                foreach (SettingsProperty prop in settings.Properties)
                {
                    if (prop.Attributes.ContainsKey(typeof(UserScopedSettingAttribute)))
                    {
                        object result = settings.GetPreviousVersion(prop.Name);
                        settings[prop.Name] = result;
                    }
                }
                settings.Save();
            }
        }
    }
}

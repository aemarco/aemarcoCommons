using System.Configuration;

namespace Toolbox.SyncTools
{
    public static class ClickOnceSetting
    {
        public static void EnsureMaintainUserSettings(this ApplicationSettingsBase settings, string keyName)
        {
            if (settings == null) return;
            
            var keyValue = settings[keyName];
            var defaultValue = settings.Properties[keyName]?.DefaultValue;

            if (keyValue != defaultValue) return;
            

            foreach (SettingsProperty prop in settings.Properties)
            {
                if (!prop.Attributes.ContainsKey(typeof(UserScopedSettingAttribute))) continue;


                var result = settings.GetPreviousVersion(prop.Name);
                settings[prop.Name] = result;
            }
            settings.Save();
        }
    }
}

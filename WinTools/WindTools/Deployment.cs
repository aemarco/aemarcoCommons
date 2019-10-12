using Newtonsoft.Json;
using System.Configuration;

namespace WinTools.WindTools
{

    public static class Deployment
    {
        public static string Publisher
        {
            get
            {
                return GetSettings().PublisherName;
            }
            set
            {
                var settings = GetSettings();
                settings.PublisherName = value;
                SetSettings(settings);
            }
        }
        public static string Product
        {
            get
            {
                return GetSettings().ProductName;
            }
            set
            {
                var settings = GetSettings();
                settings.ProductName = value;
                SetSettings(settings);
            }
        }

        private static DeploymentSettings GetSettings()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            var result = JsonConvert.DeserializeObject<DeploymentSettings>(settings[nameof(DeploymentSettings)].Value.ToString());
            return result;
        }

        private static void SetSettings(DeploymentSettings newSettings)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            if (settings[nameof(DeploymentSettings)] == null)
            {
                settings.Add(nameof(DeploymentSettings), JsonConvert.SerializeObject(newSettings));
            }
            else
            {
                settings[nameof(DeploymentSettings)].Value = JsonConvert.SerializeObject(newSettings);
            }
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }

    }


    public class DeploymentSettings
    {
        //Json Stuff for saving
        public string PublisherName { get; set; }
        public string ProductName { get; set; }
    }
}

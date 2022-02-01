using aemarcoCommons.Toolbox.AppConfiguration.Transformations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;

namespace aemarcoCommons.Toolbox.AppConfiguration
{
    /// <summary>
    /// Custom Path in IConfiguration
    /// ex. empty string means root,
    /// ex. "Abc:Def"
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SettingPathAttribute : Attribute
    {
        public string Path { get; }
        public SettingPathAttribute(string path)
        {
            Path = path;
        }
    }

    /// <summary>
    /// Inherit this class in your setting classes
    /// </summary>
    public abstract class SettingsBase
    {
        internal static IConfigurationRoot ConfigurationRoot { get; set; }
        internal static ConfigurationOptions ConfigurationOptions { get; set; }

        protected SettingsBase()
        {
            if (ConfigurationRoot == null || ConfigurationOptions == null) return;

            Init();
            if (ConfigurationOptions.WatchSavedFiles)
                ChangeToken.OnChange(ConfigurationRoot.GetReloadToken, Init);
        }

        private void Init()
        {

            //TODO: Bin merges collections, so set defaults before re init

            var sectionPath = GetType().GetSectionPath();
            //read current values from IConfiguration
            if (string.IsNullOrWhiteSpace(sectionPath))
                ConfigurationRoot.Bind(this);
            else
                ConfigurationRoot.GetSection(sectionPath).Bind(this);

            //ApplyReadTransformations
            foreach (var transformation in ConfigurationOptions.StringTransformations)
            {
                StringTransformerBase.TransformObject(this, ConfigurationRoot, transformation.PerformReadTransformation);
            }
        }
        //TODO: Save Overload where directory and/or filename can be specified
        /// <summary>
        /// Save this Configuration.
        /// SettingsSaveDirectory should be defined if no filePath is given.
        /// </summary>
        public string Save()
        {
            var type = GetType();
            var copyForSave = MemberwiseClone();

            //decide for a filePath
            var filePath = type.GetSavePathForSetting(ConfigurationOptions);
            var sectionPath = type.GetSectionPath();

            //ApplyWriteTransformations in reverse order
            var transformations = ConfigurationOptions.StringTransformations.ToList();
            transformations.Reverse();
            foreach (var transformation in transformations)
            {
                StringTransformerBase.TransformObject(copyForSave, ConfigurationRoot, transformation.PerformWriteTransformation);
            }

            //save the stuff
            var obj = JObject.FromObject(copyForSave);
            //use defined path if not root
            if (!string.IsNullOrWhiteSpace(sectionPath))
            {
                obj = sectionPath.Split(':')
                    .Reverse()
                    .Aggregate(obj, (current, section) => new JObject { { section, current } });
            }

            File.WriteAllText(filePath, obj.ToString());
            
            return filePath;
        }
    }
}

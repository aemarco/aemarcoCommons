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

            //TODO: Bind merges collections, grrrr
            //https://github.com/dotnet/runtime/issues/46988

            //problems:
            //a: if multiple providers provide values for a collection, the values get added
            //b: when having Config inside Config, Bind will be executed twice
            //   Bind binds recursive, so the subConfig Constructor calls Init (bind#1) and then binds again below (bind#2)
            //c: On the init call from Change token, existing element will be bound again

            //idea for workaround:
            // construct sub configs before binding (has correct values) and replace here after binding.
            

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
        

       
        /// <summary>
        /// Save this Configuration. 
        /// SettingsSaveDirectory should be defined when using this.
        /// Settings saved in this manner, will be automatically loaded on next start of the app
        /// </summary>
        /// <returns>the path were the config was saved</returns>
        public string Save()
        {
            var type = GetType();
            var filePath = type.GetSavePathForSetting(ConfigurationOptions);
            Save(filePath);
            return filePath;
        }

        
        /// <summary>
        /// Export this configuration based on root level to a given file path
        /// </summary>
        /// <param name="filePath">file path to export to</param>
        public void Save(string filePath)
        {
            var type = GetType();
            var copyForSave = MemberwiseClone();

            //decide for a SectionPath
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
        }
    }
}

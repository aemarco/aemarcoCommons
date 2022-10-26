using aemarcoCommons.Extensions.AttributeExtensions;
using aemarcoCommons.Toolbox.AppConfiguration.Transformations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

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
    /// Use this to automatically save on PropertyChanged event
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoSaveOnPropertyChangedAttribute : Attribute
    {
        public AutoSaveOnPropertyChangedAttribute(params string[] ignoredProperties)
        {
            IgnoredProperties = ignoredProperties;
        }
        public string[] IgnoredProperties { get; }
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

            this.HandleAutoSaveOnPropertyChanged();
        }

        private void Init()
        {

            //problems:
            //a: if multiple providers provide values for a collection, the values get added
            //doubleMerge: when having Config inside Config, Bind will be executed twice
            //   Bind binds recursive, so the subConfig Constructor calls Init (bind#1) and then binds again below (bind#2)
            //c: On the init call from Change token, existing element will be bound again
            //d: setting classes will be registered as singletons, but when using sub settings is used,
            //   you will end up with separate instances despite registrations. To handle this correctly,
            //   sub settings would need to be resolved from ioc, and set in this instance.


            #region doubleMerge

            //create sub Settings by us here, so they will not get double bound here
            //after binding below, the constructor of the sub class will bind, and then the binding here will bind it again
            //so with merge behaviour of binding, values of array will be doubled
            //https://github.com/dotnet/runtime/issues/46988

            //idea for doubleMerge workaround:
            // construct sub configs before binding (has correct values) and replace here after binding.

            var subSettings = new Dictionary<PropertyInfo, object>();
            foreach (var propertyInfo in GetType().GetProperties()
                         .Where(x => x.CanRead && x.CanWrite)
                         .Where(x => typeof(SettingsBase).IsAssignableFrom(x.PropertyType)))
            {
                var subSetting = Activator.CreateInstance(propertyInfo.PropertyType);
                subSettings.Add(propertyInfo, subSetting);
            }

            #endregion


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

            #region doubleMerge

            //set back sub settings
            foreach (var pair in subSettings)
            {
                pair.Key.SetValue(this, pair.Value);
            }

            #endregion
        }



        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedParameter.Local
        private void AutoSaveOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var attr = this.GetAttribute<AutoSaveOnPropertyChangedAttribute>();
            if (attr.IgnoredProperties.Contains(e.PropertyName))
                return;

            Save();
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
        /// given folder must already exist
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

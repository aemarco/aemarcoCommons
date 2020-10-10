using System;
using System.Collections.Generic;
using aemarcoCommons.Toolbox.AppConfiguration.Transformations;

namespace aemarcoCommons.Toolbox.AppConfiguration
{
    public class ConfigurationOptions
    {
        public ConfigurationOptions()
        {
            SettingsSaveDirectory = AppDomain.CurrentDomain.BaseDirectory;
            GetFileNameForSettingsClass = type =>
            {
                var fileMiddleName = type.Name;
                //use path defined in attribute if specified
                if (Attribute.GetCustomAttribute(type, typeof(SettingPathAttribute)) is SettingPathAttribute pathAttribute &&
                    !string.IsNullOrWhiteSpace(pathAttribute.Path))
                    fileMiddleName = pathAttribute.Path.Replace(':', '_');
                return $"savedSettings.{fileMiddleName}.json";
            };
        }

        /// <summary>
        /// If Save() is being used on any Settings-Class, define a directory where stuff gets saved
        /// - defaults to app directory
        /// - set null to disable saving stuff
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public string SettingsSaveDirectory { get; set; }

        /// <summary>
        /// If set, that function will be called to determine the filename for given settings type
        /// </summary>
        public Func<Type, string> GetFileNameForSettingsClass { get; set; }

        /// <summary>
        /// provide a list of transformations which will be called
        /// - in order during read
        /// - in reverse order during write
        /// </summary>
        public List<StringTransformerBase> StringTransformations { get; } = new List<StringTransformerBase>();

        /// <summary>
        /// Add Setting types which won´t get found by reflection (basically the ones throwing exceptions when try to resolve)
        /// By even enlisting them here, necessary assemblies get loaded, so they appear then
        /// </summary>
        // ReSharper disable once CollectionNeverQueried.Global
        // ReSharper disable once UnusedMember.Global
        public List<Type> AdditionalRegistrations { get; } = new List<Type>(); 

    }
}
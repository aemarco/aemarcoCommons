using System;
using System.Collections.Generic;
using aemarcoCommons.Toolbox.Autofac.AppConfiguration.Transformations;

namespace aemarcoCommons.Toolbox.Autofac.AppConfiguration
{
    public class ConfigurationOptions
    {
        public ConfigurationOptions()
        {
            SettingsSaveDirectory = AppDomain.CurrentDomain.BaseDirectory;
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
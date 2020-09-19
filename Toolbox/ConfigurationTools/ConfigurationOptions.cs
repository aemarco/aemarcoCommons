using System;
using System.Collections.Generic;

namespace aemarcoCommons.Toolbox.ConfigurationTools
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
        public string SettingsSaveDirectory { get; set; }

        /// <summary>
        /// If ProtectedAttribute is used, define Iv and Key for Cryptography
        /// </summary>
        public string Iv { get; set; }
        /// <summary>
        /// If ProtectedAttribute is used, define Iv and Key for Cryptography
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// If ProtectedAttribute is used, define Password instead of Key and Iv
        /// </summary>
        public string Password { get; set; }


        /// <summary>
        /// If set, Decryption in Skipped during initialization of Setting-Classes
        /// </summary>
        public bool SkipDecrypt { get; set; }

        /// <summary>
        /// Add types you miss for injection
        /// </summary>
        // ReSharper disable once CollectionNeverQueried.Global
        public List<Type> AdditionalRegistrations { get; } = new List<Type>(); 

    }
}
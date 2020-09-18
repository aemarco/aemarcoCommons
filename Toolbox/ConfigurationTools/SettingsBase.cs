using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace aemarcoCommons.Toolbox.ConfigurationTools
{
    public abstract class SettingsBase
    {
        internal static IConfigurationRoot ConfigRoot { get; set; }
        internal static ConfigurationOptions Options { get; set; }
       
        protected SettingsBase()
        {
            if (ConfigRoot == null || Options == null) return;

            Init();
            ChangeToken.OnChange(ConfigRoot.GetReloadToken, Init);
        }

        private string _sectionPath;
        private void Init()
        {
            var type = GetType();
            //use path defined in attribute if specified
            if (Attribute.GetCustomAttribute(type, typeof(SettingPathAttribute)) is SettingPathAttribute pathAttribute)
                _sectionPath = pathAttribute.Path;
            //use typename if nothing is specified
            else
                _sectionPath = type.Name;


            //read current values from IConfiguration
            if (string.IsNullOrWhiteSpace(_sectionPath))
                ConfigRoot.Bind(this);
            else
                ConfigRoot.GetSection(_sectionPath).Bind(this);


            //decrypt all Protected Properties
            if (Options != null &&
                !Options.SkipDecrypt &&
                !string.IsNullOrWhiteSpace(Options.Iv) &&
                !string.IsNullOrWhiteSpace(Options.Key))
            {
                this.ProtectedTransformObject(x => 
                    CryptoStuff.Decrypt(x, Options.Iv, Options.Key));
            }
        }


        /// <summary>
        /// Save this Configuration.
        /// SettingsSaveDirectory should be defined if no filePath is given.
        /// </summary>
        /// <param name="filePath">optional file Path to save to</param>
        public void Save(string filePath = null)
        {
            //decide for a filePath
            var type = GetType();
            filePath ??= type.GetSavePathForSetting(Options.SettingsSaveDirectory);

            //encrypt all Protected Properties
            if (Options != null && 
                !string.IsNullOrWhiteSpace(Options.Iv) &&
                !string.IsNullOrWhiteSpace(Options.Key))
            {
                this.ProtectedTransformObject(x => 
                    CryptoStuff.Encrypt(x, Options.Iv, Options.Key));
            }

            //save the stuff
            var obj = JObject.FromObject(this);
            //use defined path if not root
            if (!string.IsNullOrWhiteSpace(_sectionPath))
            {
                obj = _sectionPath.Split(':')
                    .Reverse()
                    .Aggregate(obj, (current, section) => new JObject {{section, current}});
            }

            File.WriteAllText(filePath,obj.ToString());
        }
    }
}

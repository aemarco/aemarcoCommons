using System;
using System.IO;
using System.Linq;
using aemarcoCommons.Extensions.TextExtensions;
using aemarcoCommons.Toolbox.CryptoTools;
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

            ApplyReadTransformations();
        }
        /// <summary>
        /// Save this Configuration.
        /// SettingsSaveDirectory should be defined if no filePath is given.
        /// </summary>
        public string Save()
        {
            //decide for a filePath
            var filePath = GetType().GetSavePathForSetting(Options.SettingsSaveDirectory);


            ApplyWriteTransformations();
            
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
            return filePath;
        }





        private void ApplyReadTransformations()
        {
            //decrypt all Protected Properties
            if (Options != null && !Options.SkipDecrypt)
            {
                if (!string.IsNullOrWhiteSpace(Options.Password)) 
                    ProtectedTransformObject(this, x=> TextCipher.Decrypt(x, Options.Password));
                else if (!string.IsNullOrWhiteSpace(Options.Key) && !string.IsNullOrWhiteSpace(Options.Iv))
                    ProtectedTransformObject(this,x=> CryptoStuff.Decrypt(x, Options.Iv, Options.Key));
            }


            PlaceholdersTransformObject(this);
        }

        private void ApplyWriteTransformations()
        {
            //encrypt all Protected Properties
            if (Options != null)
            {
                if (!string.IsNullOrWhiteSpace(Options.Password))
                    ProtectedTransformObject(this, x => TextCipher.Encrypt(x, Options.Password));
                else if (!string.IsNullOrWhiteSpace(Options.Key) && !string.IsNullOrWhiteSpace(Options.Iv))
                    ProtectedTransformObject(this, x => CryptoStuff.Encrypt(x, Options.Iv, Options.Key));
            }

        }



        /// <summary>
        /// Performs string transformations on given objects string properties with ProtectedAttribute defined
        /// </summary>
        /// <param name="obj">object which needs transformation</param>
        /// <param name="transform">transformation which should be performed</param>
        private static void ProtectedTransformObject(object obj, Func<string, string> transform)
        {
            //handle crypto
            foreach (var propInfo in obj.GetType().GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(ProtectedAttribute)))
                .Where(x => x.PropertyType == typeof(string))
                .Where(x => x.CanRead && x.CanWrite))
            {
                propInfo.SetValue(obj, transform((string)propInfo.GetValue(obj)));
            }

            //recurse
            foreach (var propInfo in obj.GetType().GetProperties()
                .Where(x => typeof(SettingsBase).IsAssignableFrom(x.PropertyType))
                .Where(x => x.GetValue(obj) != null))
            {
                ProtectedTransformObject(propInfo.GetValue(obj), transform);
            }
        }


        /// <summary>
        /// Resolves string placeholders on given objects string properties
        /// </summary>
        /// <param name="obj"></param>
        private static void PlaceholdersTransformObject(object obj)
        {
            //handle string placeholders
            foreach (var propInfo in obj.GetType().GetProperties()
                .Where(x => x.PropertyType == typeof(string))
                .Where(x => x.CanRead && x.CanWrite))
            {
                var currentValue = (string)propInfo.GetValue(obj);
                var resolved = currentValue.ResolvePlaceholders(ConfigRoot);
                if (currentValue != resolved)
                {
                    propInfo.SetValue(obj, resolved);
                }
            }

            //recurse
            foreach (var propInfo in obj.GetType().GetProperties()
                .Where(x => typeof(SettingsBase).IsAssignableFrom(x.PropertyType))
                .Where(x => x.GetValue(obj) != null))
            {
                PlaceholdersTransformObject(propInfo.GetValue(obj));
            }
        }





    }
}

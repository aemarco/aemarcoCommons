using System;
using System.IO;
using System.Linq;
using aemarcoCommons.Toolbox.AppConfiguration.Transformations;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

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
        internal static ILifetimeScope RootScope { get; set; }

       

        private readonly IConfigurationRoot _configRoot;
        private readonly ConfigurationOptions _options;
        private readonly Type _type;
        
        protected SettingsBase()
        {
            _type = GetType();

            if (RootScope == null) return;

            _configRoot = RootScope.Resolve<IConfigurationRoot>();
            _options = RootScope.Resolve<ConfigurationOptions>();
            

            Init();
            ChangeToken.OnChange(_configRoot.GetReloadToken, Init);
        }

        private string _sectionPath;
        private void Init()
        {
            //use path defined in attribute if specified
            if (Attribute.GetCustomAttribute(_type, typeof(SettingPathAttribute)) is SettingPathAttribute pathAttribute)
                _sectionPath = pathAttribute.Path;
            //use typename if nothing is specified
            else
                _sectionPath = _type.Name;


            //read current values from IConfiguration
            if (string.IsNullOrWhiteSpace(_sectionPath))
                _configRoot.Bind(this);
            else
                _configRoot.GetSection(_sectionPath).Bind(this);



            //ApplyReadTransformations
            foreach (var transformation in _options.StringTransformations)
            {
                StringTransformerBase.TransformObject(this, _configRoot, transformation.PerformReadTransformation);
            }
        }


        /// <summary>
        /// Save this Configuration.
        /// SettingsSaveDirectory should be defined if no filePath is given.
        /// </summary>
        public string Save()
        {
            //decide for a filePath
            var filePath = _type.GetSavePathForSetting(_options.SettingsSaveDirectory);


            //ApplyWriteTransformations in reverse order
            var transformations = _options.StringTransformations;
            transformations.Reverse();
            foreach (var transformation in transformations)
            {
                StringTransformerBase.TransformObject(this, _configRoot, transformation.PerformWriteTransformation);
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
            return filePath;
        }
    }
}

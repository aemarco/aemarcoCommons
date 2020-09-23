using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace aemarcoCommons.Toolbox.AppConfiguration.Transformations
{
    public abstract class StringTransformerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentValue">value which comes from previous transformation or from IConfiguration</param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public abstract string PerformReadTransformation(string currentValue, PropertyInfo propertyInfo, IConfigurationRoot configRoot);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentValue">value which comes from previous transformation or from SettingsObject</param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public abstract string PerformWriteTransformation(string currentValue, PropertyInfo propertyInfo, IConfigurationRoot configRoot);


        /// <summary>
        /// Performs string transformations on given objects string properties 
        /// </summary>
        /// <param name="obj">object which needs transformation</param>
        /// <param name="configRoot"></param>
        /// <param name="transform">transformation which should be performed</param>
        internal static void TransformObject(object obj, IConfigurationRoot configRoot, Func<string, PropertyInfo, IConfigurationRoot, string> transform)
        {
            //handle
            foreach (var propInfo in obj.GetType().GetProperties()
                .Where(x => x.PropertyType == typeof(string))
                .Where(x => x.CanRead && x.CanWrite))
            {
                propInfo.SetValue(obj, transform((string)propInfo.GetValue(obj), propInfo, configRoot));
            }

            //recurse
            foreach (var propInfo in obj.GetType().GetProperties()
                .Where(x => typeof(SettingsBase).IsAssignableFrom(x.PropertyType))
                .Where(x => x.GetValue(obj) != null))
            {
                TransformObject(propInfo.GetValue(obj), configRoot, transform);
            }
        }
    }
}
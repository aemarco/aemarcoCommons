using System.Collections;
using System.Linq;

namespace aemarcoCommons.ToolboxAppOptions.Transformations;

public abstract class StringTransformerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentValue">value which comes from previous transformation or from IConfiguration</param>
    /// <param name="propertyInfo"></param>
    /// <param name="configRoot"></param>
    /// <returns></returns>
    public abstract string PerformReadTransformation(string currentValue, PropertyInfo propertyInfo, IConfigurationRoot configRoot);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentValue">value which comes from previous transformation or from SettingsObject</param>
    /// <param name="propertyInfo"></param>
    /// <param name="configRoot"></param>
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
            .Where(x => x is { CanRead: true, CanWrite: true }))
        {
            if (propInfo.GetValue(obj) is string value)
                propInfo.SetValue(obj, transform(value, propInfo, configRoot));
        }

        //recurse
        foreach (var propInfo in obj.GetType().GetProperties()
            .Where(x => typeof(ISettingsBase).IsAssignableFrom(x.PropertyType))
            .Where(x => x.GetValue(obj) is not null))
        {
            TransformObject(propInfo.GetValue(obj)!, configRoot, transform);
        }


        // Recurse through collections of ISettingsBase
        foreach (var propInfo in obj.GetType().GetProperties())
        {
            var value = propInfo.GetValue(obj);
            if (value is IDictionary dictionary)
            {
                // Handle dictionaries
                var dictType = propInfo.PropertyType;
                var keyType = dictType.GetGenericArguments().FirstOrDefault();
                var valueType = dictType.GetGenericArguments().Skip(1).FirstOrDefault();

                if (keyType != null && typeof(ISettingsBase).IsAssignableFrom(keyType))
                {
                    // Process keys
                    foreach (var key in dictionary.Keys)
                    {
                        if (key is ISettingsBase settingsBaseKey)
                        {
                            TransformObject(settingsBaseKey, configRoot, transform);
                        }
                    }
                }

                if (valueType != null && typeof(ISettingsBase).IsAssignableFrom(valueType))
                {
                    // Process values
                    foreach (var val in dictionary.Values)
                    {
                        if (val is ISettingsBase settingsBaseValue)
                        {
                            TransformObject(settingsBaseValue, configRoot, transform);
                        }
                    }
                }
            }
            else if (value is not string && value is IEnumerable collection)
            {
                Type? genericElementType = null;
                if (propInfo.PropertyType.IsGenericType)
                {
                    // For generic types like List<T>, Array, etc.
                    genericElementType = propInfo.PropertyType.GetGenericArguments().FirstOrDefault();
                }
                else if (propInfo.PropertyType.IsArray)
                {
                    // For arrays
                    genericElementType = propInfo.PropertyType.GetElementType();
                }

                if (genericElementType is not null && typeof(ISettingsBase).IsAssignableFrom(genericElementType))
                {
                    foreach (var item in collection)
                    {
                        if (item is ISettingsBase settingsBase)
                        {
                            TransformObject(settingsBase, configRoot, transform);
                        }
                    }
                }
            }

        }





    }
}
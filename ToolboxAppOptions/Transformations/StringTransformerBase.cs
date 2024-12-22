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
    internal static void TransformObject(object? obj, IConfigurationRoot configRoot, Func<string, PropertyInfo, IConfigurationRoot, string> transform)
    {
        if (obj is null)
            return;

        //handle
        foreach (var propInfo in obj.GetType().GetProperties())
        {
            if (!propInfo.CanWrite)
                continue;

            var value = propInfo.GetValue(obj);

            // Handle strings
            if (value is string unresolved)
            {
                if (propInfo is { CanRead: true, CanWrite: true })
                    propInfo.SetValue(obj, transform(unresolved, propInfo, configRoot));
                continue;
            }

            // Handle dictionaries
            if (value is IDictionary dict)
            {
                var valueType = propInfo.PropertyType.GetGenericArguments().Skip(1).FirstOrDefault();
                if (valueType == typeof(string))
                {
                    var keys = dict.Keys.Cast<object>().ToList();
                    foreach (var key in keys)
                    {
                        if (dict[key] is string val)
                        {
                            dict[key] = transform(val, propInfo, configRoot);
                        }
                    }
                }
                else if (valueType?.IsClass is true)
                {
                    foreach (var val in dict.Values)
                    {
                        TransformObject(val, configRoot, transform);
                    }
                }
                continue;
            }

            // Handle lists
            if (value is IList list)
            {
                var valueType = propInfo.PropertyType.IsGenericType
                    ? propInfo.PropertyType.GetGenericArguments().FirstOrDefault()
                    : propInfo.PropertyType.IsArray
                        ? propInfo.PropertyType.GetElementType()
                        : null;
                if (valueType == typeof(string))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] is string listText)
                        {
                            list[i] = transform(listText, propInfo, configRoot);
                        }
                    }
                }
                else if (valueType?.IsClass is true)
                {
                    foreach (var val in list)
                    {
                        TransformObject(val, configRoot, transform);
                    }
                }
                continue;
            }

            // Handle nested objects
            if (propInfo.PropertyType.IsClass)
            {
                TransformObject(value, configRoot, transform);
                //continue;
            }
        }
    }




}
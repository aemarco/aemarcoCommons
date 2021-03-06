﻿using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace aemarcoCommons.Toolbox.AppConfiguration.Transformations
{
    public class PlaceholderTransformation : StringTransformerBase
    {
        public override string PerformReadTransformation(string currentValue, PropertyInfo propertyInfo, IConfigurationRoot configRoot)
        {
            var resolved = configRoot.ResolvePlaceholders(currentValue);
            return resolved;
        }

        public override string PerformWriteTransformation(string currentValue, PropertyInfo propertyInfo, IConfigurationRoot configRoot) =>
            currentValue;
    }
}

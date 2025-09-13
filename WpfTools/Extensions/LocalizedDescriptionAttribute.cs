using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace aemarcoCommons.WpfTools.Extensions;

//inspired by
//https://youtu.be/T1mhORJCDsY

public class LocalizedDescriptionAttribute : DescriptionAttribute
{
    private readonly string _resourceKey;
    private readonly ResourceManager _resourceManager;
    public LocalizedDescriptionAttribute(string resourceKey, Type resourceType)
    {
        _resourceKey = resourceKey;
        _resourceManager = new ResourceManager(resourceType);
    }


    private static ResourceManager? _cacheResourceManager;
    public LocalizedDescriptionAttribute(string resourceKey)
    {
        _resourceKey = resourceKey;
        if (_cacheResourceManager is null)
        {
            var resourceType = Assembly
                                   .GetEntryAssembly()!
                                   .GetTypes()
                                   .FirstOrDefault(x => x.Name == "Strings")
                               ?? throw new Exception("Could not find Strings Type");
            var propInfo = resourceType.GetProperty("ResourceManager")
                           ?? throw new Exception("Could not find ResourceManager in Strings");
            _cacheResourceManager = (ResourceManager)propInfo.GetValue(null)!;
        }
        _resourceManager = _cacheResourceManager;
    }


    public override string Description
    {
        get
        {
            string? description = _resourceManager.GetString(_resourceKey);
            return string.IsNullOrWhiteSpace(description)
                ? $"[[{_resourceKey}]]"
                : description;
        }
    }



}

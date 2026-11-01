using Newtonsoft.Json;

#pragma warning disable IDE0130

namespace aemarcoCommons.ToolboxHangfire;

[AttributeUsage(AttributeTargets.Parameter)]
public class JsonFingerAttribute : Attribute
{
    private readonly string[] _propertiesToConsider;

    /// <summary>
    /// JsonFinger does define, that the parameter should be fingerprinted by Json
    /// With "propertiesToConsider", it can be defined which properties of this parameter should be considered in the signature
    /// </summary>
    /// <param name="propertiesToConsider">Names of properties to consider for fingerprinting, defaults to all</param>
    public JsonFingerAttribute(params string[] propertiesToConsider)
    {
        _propertiesToConsider = propertiesToConsider;
    }

    public string CreateSignature(object obj)
    {
        if (_propertiesToConsider.Length == 0)
            return JsonConvert.SerializeObject(obj);


        var entries = new List<string>();
        var type = obj.GetType();
        foreach (var propName in _propertiesToConsider)
        {
            entries.Add(propName);
            if (type.GetProperty(propName)?.GetValue(obj)?.ToString() is { } value)
            {
                entries.Add(value);
            }
        }
        return string.Join('_', entries);
    }
}

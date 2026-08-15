#pragma warning disable IDE0130
namespace aemarcoCommons.ToolboxAppOptions;

public static class ConfigurationExtensions
{
    extension(IConfiguration config)
    {
        public TValue? GetResolved<TSettings, TValue>(Expression<Func<TSettings, TValue>> selector)
            where TSettings : ISettingsBase
        {
            if (selector.Body is not MemberExpression { Expression: ParameterExpression, Member.Name: { } propertyName })
                throw new ArgumentException("Selector must be a simple property access.", nameof(selector));

            var type = typeof(TSettings);
            var path = type.GetAttribute<SettingsPathAttribute>() is { } pathAttribute
                ? pathAttribute.Path
                : type.Name;

            var key = string.IsNullOrWhiteSpace(path)
                ? propertyName
                : $"{path}:{propertyName}";

            return config.GetResolved<TValue>(key);
        }

        public TValue? GetResolved<TValue>(string path)
        {
            var result = config.GetSection(path).Get<TValue>();
            if (result is null || typeof(TValue) != typeof(string))
                return result;

            var resolved = ((IConfigurationRoot)config)
                .ResolvePlaceholders((string)(object)result);
            return (TValue)(object)resolved;
        }
    }
}
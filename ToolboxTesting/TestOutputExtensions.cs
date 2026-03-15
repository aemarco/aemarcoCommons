namespace aemarcoCommons.ToolboxTesting;

public static class TestOutputExtensions
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public static T Dump<T>(this T obj)
    {
        if (obj is null)
        {
            TestContext.Out.WriteLine("Passed with: null");
            return default!;
        }

        var typeName = GetTypeName(obj);
        var json = JsonSerializer.Serialize(obj, Options);

        TestContext.Out.WriteLine($"""
                                   Passed with: {typeName}
                                    {json}
                                   """);

        return obj;
    }

    private static string GetTypeName(object obj)
    {
        var type = obj.GetType();
        if (obj is not IEnumerable or string)
            return type.Name;

        var elementType = GetCollectionElementType(type);
        return $"{elementType?.Name ?? "UnknownType"}[]";
    }

    private static Type? GetCollectionElementType(Type collectionType)
    {
        if (collectionType.IsArray)
            return collectionType.GetElementType();

        var genericArguments = collectionType.GetGenericArguments();
        if (genericArguments.Length > 0)
            return genericArguments[0];

        if (typeof(IEnumerable).IsAssignableFrom(collectionType))
        {
            foreach (var interfaceType in collectionType.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return interfaceType.GetGenericArguments()[0];
                }
            }
        }
        return null;
    }
}
